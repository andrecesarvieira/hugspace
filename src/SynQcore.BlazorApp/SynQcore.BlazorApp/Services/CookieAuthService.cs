using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using SynQcore.BlazorApp.Store.User;
using SynQcore.BlazorApp.Services.StateManagement;

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
    private readonly StateManager _stateManager;
    private readonly ILogger<CookieAuthService> _logger;

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Iniciando processo de login para usuário: {email}")]
    private static partial void LogLoginStarted(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Login bem-sucedido para usuário: {email}")]
    private static partial void LogLoginSuccessful(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Warning, "Falha no login para usuário: {email} - {error}")]
    private static partial void LogLoginFailed(ILogger logger, string email, string error, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Logout realizado para usuário: {email}")]
    private static partial void LogLogoutCompleted(ILogger logger, string email, Exception? exception);

    [LoggerMessage(LogLevel.Error, "Erro durante autenticação: {error}")]
    private static partial void LogAuthenticationError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Verificando status de autenticação do usuário")]
    private static partial void LogAuthenticationCheck(ILogger logger, Exception? exception);

    public CookieAuthService(
        IHttpContextAccessor httpContextAccessor,
        IApiService apiService,
        StateManager stateManager,
        ILogger<CookieAuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiService = apiService;
        _stateManager = stateManager;
        _logger = logger;
    }

    public bool IsAuthenticated
    {
        get
        {
            try
            {
                LogAuthenticationCheck(_logger, null);
                var context = _httpContextAccessor.HttpContext;
                return context?.User?.Identity?.IsAuthenticated == true;
            }
            catch (Exception ex)
            {
                LogAuthenticationError(_logger, ex.Message, ex);
                return false;
            }
        }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            LogLoginStarted(_logger, email, null);

            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                LogLoginFailed(_logger, email, "HttpContext não disponível", null);
                return false;
            }

            // Usar StateManager para fazer login (simula API)
            await _stateManager.User.LoginAsync(email, password);
            
            var userInfo = _stateManager.User.CurrentUser;
            if (userInfo == null)
            {
                LogLoginFailed(_logger, email, "Falha na autenticação", null);
                return false;
            }

            // Criar claims para o cookie
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userInfo.Id),
                new(ClaimTypes.Name, userInfo.Nome),
                new(ClaimTypes.Email, userInfo.Email),
                new("username", userInfo.Username)
            };

            // Adicionar roles
            if (userInfo.Roles != null)
            {
                foreach (var role in userInfo.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Criar identity e principal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            // Realizar login no ASP.NET Core Authentication
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            LogLoginSuccessful(_logger, email, null);
            return true;
        }
        catch (Exception ex)
        {
            LogAuthenticationError(_logger, ex.Message, ex);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var currentUser = GetCurrentUser();
            var userEmail = currentUser?.Email ?? "usuário desconhecido";

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            // Limpar estado no StateManager
            _stateManager.User.Logout();

            LogLogoutCompleted(_logger, userEmail, null);
        }
        catch (Exception ex)
        {
            LogAuthenticationError(_logger, ex.Message, ex);
        }
    }

    public UserInfo? GetCurrentUser()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var user = context.User;
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim))
            {
                return null;
            }

            return new UserInfo
            {
                Id = idClaim,
                Nome = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Username = user.FindFirst("username")?.Value ?? string.Empty,
                Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            };
        }
        catch (Exception ex)
        {
            LogAuthenticationError(_logger, ex.Message, ex);
            return null;
        }
    }
}
