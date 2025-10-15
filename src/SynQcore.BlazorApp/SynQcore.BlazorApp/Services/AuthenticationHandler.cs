using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using SynQcore.BlazorApp.Services.StateManagement;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Handler HTTP para interceptar requisições e automatizar autenticação
/// </summary>
public partial class AuthenticationHandler : DelegatingHandler
{
    private readonly StateManager _stateManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthenticationHandler> _logger;

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Error, "Erro de conectividade HTTP: {Message}")]
    private static partial void LogHttpConnectionError(ILogger logger, string message, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Timeout na requisição HTTP")]
    private static partial void LogHttpTimeout(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Information, "Tentando renovar token de acesso...")]
    private static partial void LogTokenRefreshAttempt(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Falha na renovação do token - forçando logout")]
    private static partial void LogTokenRefreshFailure(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Erro durante tentativa de renovação de token")]
    private static partial void LogTokenRefreshError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Não há refresh token disponível - forçando logout")]
    private static partial void LogNoRefreshToken(ILogger logger);

    public AuthenticationHandler(
        StateManager stateManager,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ILogger<AuthenticationHandler> logger)
    {
        _stateManager = stateManager;
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _logger = logger;
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

        // Fallback final para StateManager
        if (string.IsNullOrEmpty(token))
        {
            token = _stateManager.User.AccessToken;
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
                _stateManager.User.IsAuthenticated)
            {
                // Tenta renovar o token automaticamente
                await HandleUnauthorized();
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            // Atualiza status de conectividade via StateManager
            _stateManager.UI.SetConnectivityStatus(false);
            LogHttpConnectionError(_logger, ex.Message, ex);
            throw;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            // Atualiza status via StateManager
            _stateManager.UI.SetConnectivityStatus(false);
            LogHttpTimeout(_logger, ex);
            throw;
        }
    }

    /// <summary>
    /// Trata erro de autorização tentando renovar o token
    /// </summary>
    private async Task HandleUnauthorized()
    {
        var refreshToken = _stateManager.User.RefreshToken;

        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                LogTokenRefreshAttempt(_logger);

                // Simula tentativa de refresh (em produção seria uma chamada real à API)
                await Task.Delay(1000);

                // Por enquanto, força logout se não conseguir renovar
                LogTokenRefreshFailure(_logger);
                _stateManager.User.Logout();
            }
            catch (Exception ex)
            {
                LogTokenRefreshError(_logger, ex);
                _stateManager.User.Logout();
            }
        }
        else
        {
            // Força logout se não há refresh token
            LogNoRefreshToken(_logger);
            _stateManager.User.Logout();
        }
    }
}