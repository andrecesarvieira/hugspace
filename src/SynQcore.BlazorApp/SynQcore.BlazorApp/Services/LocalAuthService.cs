using Blazored.LocalStorage;
using SynQcore.BlazorApp.Store.User;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço de autenticação local para bypass do Fluxor
/// </summary>
public interface ILocalAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task SaveAuthDataAsync(string token, UserInfo userInfo);
}

public partial class LocalAuthService : ILocalAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;
    private readonly ILogger<LocalAuthService> _logger;
    private const string AUTH_TOKEN_KEY = "synqcore_auth_token";
    private const string USER_INFO_KEY = "synqcore_user_info";

    // Cache em memória para resolver problemas de timing do LocalStorage
    private static string? _cachedToken;
    private static UserInfo? _cachedUserInfo;
    private static bool _isAuthenticated;

    // LoggerMessage delegates para performance
    [LoggerMessage(LogLevel.Information, "[LOCAL AUTH] Tentando login para: {Email}")]
    private static partial void LogLoginAttempt(ILogger logger, string email);

    [LoggerMessage(LogLevel.Information, "[LOCAL AUTH] Login bem-sucedido para: {Email}")]
    private static partial void LogLoginSuccess(ILogger logger, string email);

    [LoggerMessage(LogLevel.Warning, "[LOCAL AUTH] Login falhou - resposta inválida")]
    private static partial void LogLoginFailed(ILogger logger);

    [LoggerMessage(LogLevel.Error, "[LOCAL AUTH] Erro durante login para: {Email}")]
    private static partial void LogLoginError(ILogger logger, string email, Exception ex);

    [LoggerMessage(LogLevel.Information, "[LOCAL AUTH] Logout realizado")]
    private static partial void LogLogoutSuccess(ILogger logger);

    [LoggerMessage(LogLevel.Error, "[LOCAL AUTH] Erro durante logout")]
    private static partial void LogLogoutError(ILogger logger, Exception ex);

    public LocalAuthService(
        ILocalStorageService localStorage,
        IApiService apiService,
        ILogger<LocalAuthService> logger)
    {
        _localStorage = localStorage;
        _apiService = apiService;
        _logger = logger;
    }
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            LogLoginAttempt(_logger, email);

            var loginRequest = new { email, password };
            var response = await _apiService.PostAsync<dynamic>("/api/auth/login", loginRequest);

            if (response?.AccessToken != null)
            {
                // Salvar token
                await _localStorage.SetItemAsync(AUTH_TOKEN_KEY, response.AccessToken.ToString());

                // Salvar informações do usuário
                var userInfo = new UserInfo
                {
                    Id = response.User?.Id?.ToString() ?? "",
                    Nome = response.User?.Nome?.ToString() ?? "",
                    Email = response.User?.Email?.ToString() ?? email,
                    Username = response.User?.Username?.ToString() ?? email.Split('@')[0]
                };

                await _localStorage.SetItemAsync(USER_INFO_KEY, userInfo);

                LogLoginSuccess(_logger, email);
                return true;
            }

            LogLoginFailed(_logger);
            return false;
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
            // Limpar cache em memória primeiro
            _cachedToken = null;
            _cachedUserInfo = null;
            _isAuthenticated = false;

            Console.WriteLine("[LOCAL AUTH] Cache em memória limpo");

            // Limpar LocalStorage
            await _localStorage.RemoveItemAsync(AUTH_TOKEN_KEY);
            await _localStorage.RemoveItemAsync(USER_INFO_KEY);
            LogLogoutSuccess(_logger);
        }
        catch (Exception ex)
        {
            LogLogoutError(_logger, ex);
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            // Verificar cache em memória primeiro (mais rápido e confiável)
            if (_isAuthenticated && !string.IsNullOrEmpty(_cachedToken))
            {
                Console.WriteLine($"[LOCAL AUTH] IsAuthenticatedAsync - Cache em memória: TRUE");
                return true;
            }

            // Fallback para LocalStorage se cache vazio
            var token = await _localStorage.GetItemAsync<string>(AUTH_TOKEN_KEY);
            var isAuthenticated = !string.IsNullOrEmpty(token);

            // Atualizar cache com dados do LocalStorage
            if (isAuthenticated)
            {
                _cachedToken = token;
                _isAuthenticated = true;
                // Carregar UserInfo também se disponível
                if (_cachedUserInfo == null)
                {
                    _cachedUserInfo = await _localStorage.GetItemAsync<UserInfo>(USER_INFO_KEY);
                }
            }

            Console.WriteLine($"[LOCAL AUTH] IsAuthenticatedAsync - LocalStorage: {isAuthenticated}, Cache atualizado: {_isAuthenticated}");
            return isAuthenticated;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOCAL AUTH] Erro em IsAuthenticatedAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            // Verificar cache primeiro
            if (_isAuthenticated && _cachedUserInfo != null)
            {
                Console.WriteLine($"[LOCAL AUTH] GetCurrentUserAsync - Cache: {_cachedUserInfo.Nome}");
                return _cachedUserInfo;
            }

            // Verificar autenticação antes de buscar dados
            var isAuth = await IsAuthenticatedAsync();
            if (!isAuth) return null;

            // Buscar do LocalStorage se não estiver em cache
            var userInfo = await _localStorage.GetItemAsync<UserInfo>(USER_INFO_KEY);
            if (userInfo != null)
            {
                _cachedUserInfo = userInfo; // Atualizar cache
                Console.WriteLine($"[LOCAL AUTH] GetCurrentUserAsync - LocalStorage: {userInfo.Nome}");
            }

            return userInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOCAL AUTH] Erro em GetCurrentUserAsync: {ex.Message}");
            return null;
        }
    }

    public async Task SaveAuthDataAsync(string token, UserInfo userInfo)
    {
        try
        {
            Console.WriteLine($"[LOCAL AUTH] SaveAuthDataAsync iniciado - Token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}..., Email: {userInfo.Email}");

            // Salvar no cache em memória PRIMEIRO (para acesso imediato)
            _cachedToken = token;
            _cachedUserInfo = userInfo;
            _isAuthenticated = !string.IsNullOrEmpty(token);

            Console.WriteLine($"[LOCAL AUTH] Cache em memória atualizado - IsAuth: {_isAuthenticated}");

            // Salvar no LocalStorage para persistência
            await _localStorage.SetItemAsync(AUTH_TOKEN_KEY, token);
            await _localStorage.SetItemAsync(USER_INFO_KEY, userInfo);

            // Verificação imediata para confirmar que foi salvo
            var savedToken = await _localStorage.GetItemAsync<string>(AUTH_TOKEN_KEY);
            var savedUser = await _localStorage.GetItemAsync<UserInfo>(USER_INFO_KEY);

            Console.WriteLine($"[LOCAL AUTH] Dados salvos com sucesso - Token recuperado: {!string.IsNullOrEmpty(savedToken)}, User recuperado: {savedUser != null}");
            LogLoginSuccess(_logger, userInfo.Email);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOCAL AUTH] ERRO ao salvar dados: {ex.Message}");
            LogLoginError(_logger, userInfo.Email, ex);
        }
    }
}
