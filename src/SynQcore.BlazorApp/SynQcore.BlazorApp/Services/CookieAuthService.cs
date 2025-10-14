using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Fluxor;
using SynQcore.BlazorApp.Store.User;
using static SynQcore.BlazorApp.Store.User.UserActions;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço de autenticação baseado em cookies para integração com ASP.NET Core Authentication
/// </summary>
public interface ICookieAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync();
    UserInfo? GetCurrentUser();
    bool IsAuthenticated { get; }
}

/// <summary>
/// Implementação do serviço de autenticação com cookies
/// </summary>
public partial class CookieAuthService : ICookieAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApiService _apiService;
    private readonly IDispatcher _dispatcher;
    private readonly ILogger<CookieAuthService> _logger;

        // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Iniciando processo de login para {Email}")]
    private static partial void LogLoginStarted(ILogger logger, string email);

    [LoggerMessage(LogLevel.Information, "Resposta da API recebida - Success: {Success}, User: {UserName}")]
    private static partial void LogApiResponse(ILogger logger, bool success, string? userName);

    [LoggerMessage(LogLevel.Error, "Erro durante login: {ErrorMessage}")]
    private static partial void LogLoginError(ILogger logger, string errorMessage, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Resposta da API nula ou inválida")]
    private static partial void LogInvalidApiResponse(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Falha na autenticação da API para usuário: {Email}. Mensagem: {Message}")]
    private static partial void LogLoginFailure(ILogger logger, string email, string message);

    [LoggerMessage(LogLevel.Information, "Login bem-sucedido para usuário {UserName}")]
    private static partial void LogLoginCompleted(ILogger logger, string userName);

    [LoggerMessage(LogLevel.Information, "Enviando requisição para auth/login")]
    private static partial void LogApiRequest(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Resposta recebida: {ResponseStatus}")]
    private static partial void LogApiResponseReceived(ILogger logger, string responseStatus);

    [LoggerMessage(LogLevel.Error, "Resposta é nula - possível erro de deserialização")]
    private static partial void LogNullResponse(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Response.Success: {Success}, Message: {Message}")]
    private static partial void LogResponseDetails(ILogger logger, bool success, string? message);

    [LoggerMessage(LogLevel.Error, "HttpContext não disponível durante o login")]
    private static partial void LogHttpContextNotAvailable(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Logout realizado com sucesso")]
    private static partial void LogLogoutSuccess(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Erro durante o processo de logout")]
    private static partial void LogLogoutError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Informações de usuário incompletas encontradas nos claims")]
    private static partial void LogIncompleteUserInfo(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Erro ao obter informações do usuário atual")]
    private static partial void LogGetUserError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Resposta HTTP já foi iniciada, não é possível definir cookies")]
    private static partial void LogResponseAlreadyStarted(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Não foi possível definir cookie - resposta já iniciada")]
    private static partial void LogCookieSetFailure(ILogger logger);

    public CookieAuthService(
        IHttpContextAccessor httpContextAccessor,
        IApiService apiService,
        IDispatcher dispatcher,
        ILogger<CookieAuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiService = apiService;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            LogLoginStarted(_logger, email);

            // Verificar se HttpContext está disponível antes de prosseguir
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                LogHttpContextNotAvailable(_logger);
                return false;
            }

            // Verificar se a resposta já foi iniciada
            if (httpContext.Response.HasStarted)
            {
                LogResponseAlreadyStarted(_logger);
                return false;
            }

            // 1. Autenticar com a API SynQcore
            var loginRequest = new
            {
                Email = email,
                Password = password,
                RememberMe = true
            };

            LogApiRequest(_logger);
            var response = await _apiService.PostAsync<ApiLoginResponse>("auth/login", loginRequest);

            LogApiResponseReceived(_logger, response != null ? "Não nula" : "Nula");

            if (response == null)
            {
                LogNullResponse(_logger);
                LogLoginFailure(_logger, email, "Resposta nula da API");
                return false;
            }

            LogResponseDetails(_logger, response.Success, response.Message);

            if (!response.Success)
            {
                LogLoginFailure(_logger, email, response.Message ?? "Falha desconhecida");
                return false;
            }

            // 2. Criar claims para o cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.User.Id),
                new Claim(ClaimTypes.Name, response.User.UserName),
                new Claim(ClaimTypes.Email, response.User.Email),
                new Claim("AccessToken", response.Token),
                new Claim("RefreshToken", response.RefreshToken ?? ""),
                new Claim("TokenExpiry", response.ExpiresAt.ToString("O"))
            };

            // Adicionar roles se disponíveis
            claims.Add(new Claim(ClaimTypes.Role, "Employee"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = response.ExpiresAt,
                AllowRefresh = true
            };

            // 3. Verificar novamente se podemos definir cookies e fazer sign-in
            if (!httpContext.Response.HasStarted)
            {
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                LogLoginCompleted(_logger, response.User.UserName);

                // 4. Atualizar estado do Fluxor
                var userInfo = new UserInfo
                {
                    Id = response.User.Id,
                    Nome = response.User.UserName,
                    Email = response.User.Email,
                    Username = response.User.UserName,
                    IsAtivo = true,
                    Roles = new List<string> { "Employee" }, // TODO: Obter roles reais da API
                    DataCadastro = DateTime.UtcNow,
                    UltimoAcesso = DateTime.UtcNow
                };

                _dispatcher.Dispatch(new LoginSuccessAction(
                    userInfo,
                    response.Token,
                    response.RefreshToken,
                    response.ExpiresAt
                ));

                return true;
            }
            else
            {
                LogCookieSetFailure(_logger);
                return false;
            }
        }
        catch (Exception ex)
        {
            LogLoginError(_logger, email, ex);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                LogLogoutSuccess(_logger);

                // Atualizar estado do Fluxor
                _dispatcher.Dispatch(new LogoutAction());
            }
        }
        catch (Exception ex)
        {
            LogLogoutError(_logger, ex);
        }
    }

    public UserInfo? GetCurrentUser()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var user = httpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = user.FindFirst(ClaimTypes.Name)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email))
            {
                LogIncompleteUserInfo(_logger);
                return null;
            }

            return new UserInfo
            {
                Id = userId,
                Nome = userName,
                Email = email,
                Username = userName,
                IsAtivo = true,
                Roles = roles.ToList(),
                UltimoAcesso = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            LogGetUserError(_logger, ex);
            return null;
        }
    }
}
