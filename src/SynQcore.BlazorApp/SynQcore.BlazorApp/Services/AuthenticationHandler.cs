using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Fluxor;
using SynQcore.BlazorApp.Store.UI;
using SynQcore.BlazorApp.Store.User;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Handler HTTP para interceptar requisições e automatizar autenticação
/// </summary>
public class AuthenticationHandler : DelegatingHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly IState<UserState> _userState;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationHandler(
        IDispatcher dispatcher,
        IState<UserState> userState,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider)
    {
        _dispatcher = dispatcher;
        _userState = userState;
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Tenta obter token do LocalAuthService primeiro (mais confiável)
        string? token = null;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var localAuthService = scope.ServiceProvider.GetService<ILocalAuthService>();
            if (localAuthService != null)
            {
                token = await localAuthService.GetAccessTokenAsync();
            }
        }
        catch (Exception)
        {
            // Silently continue to fallback options
        }

        // Fallback para cookies ASP.NET Core
        if (string.IsNullOrEmpty(token))
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                token = httpContext.User.FindFirst("AccessToken")?.Value;
            }
        }

        // Fallback final para Fluxor
        if (string.IsNullOrEmpty(token))
        {
            token = _userState.Value.AccessToken;
        }

        // Adiciona token automaticamente se disponível
        if (!string.IsNullOrEmpty(token) && request.Headers.Authorization == null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Adiciona headers corporativos
        if (!request.Headers.Contains("X-SynQcore-Client"))
        {
            request.Headers.Add("X-SynQcore-Client", "Blazor-WebApp");
        }

        if (!request.Headers.Contains("X-SynQcore-Version"))
        {
            request.Headers.Add("X-SynQcore-Version", "1.0.0");
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Verifica se o token expirou
            if (response.StatusCode == HttpStatusCode.Unauthorized &&
                _userState.Value.IsAuthenticated)
            {
                // Tenta renovar o token automaticamente
                await HandleUnauthorized();
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            // Dispara ação de erro de conectividade
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Disconnected));
            _dispatcher.Dispatch(new UIActions.ShowErrorMessageAction(
                "Erro de Conectividade",
                $"Não foi possível conectar ao servidor: {ex.Message}"
            ));
            throw;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            // Dispara ação de timeout
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Error));
            _dispatcher.Dispatch(new UIActions.ShowWarningMessageAction(
                "Timeout",
                "A requisição demorou muito para responder"
            ));
            throw;
        }
    }

    /// <summary>
    /// Trata erro de autorização tentando renovar o token
    /// </summary>
    private async Task HandleUnauthorized()
    {
        var refreshToken = _userState.Value.RefreshToken;

        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                _dispatcher.Dispatch(new UserActions.StartRefreshTokenAction());

                // Simula tentativa de refresh (em produção seria uma chamada real à API)
                await Task.Delay(1000);

                // Por enquanto, força logout se não conseguir renovar
                _dispatcher.Dispatch(new UserActions.RefreshTokenFailureAction("Token expirado"));
                _dispatcher.Dispatch(new UIActions.ShowWarningMessageAction(
                    "Sessão Expirada",
                    "Sua sessão expirou. Por favor, faça login novamente."
                ));
            }
            catch (Exception ex)
            {
                _dispatcher.Dispatch(new UserActions.RefreshTokenFailureAction(ex.Message));
            }
        }
        else
        {
            // Força logout se não há refresh token
            _dispatcher.Dispatch(new UserActions.LogoutAction());
            _dispatcher.Dispatch(new UIActions.ShowWarningMessageAction(
                "Sessão Inválida",
                "Sua sessão é inválida. Por favor, faça login novamente."
            ));
        }
    }
}
