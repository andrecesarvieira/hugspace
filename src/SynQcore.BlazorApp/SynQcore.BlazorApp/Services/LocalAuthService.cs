using Blazored.LocalStorage;
using SynQcore.BlazorApp.Store.User;
using static SynQcore.BlazorApp.Services.AuthService;

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
    Task<string?> GetAccessTokenAsync();
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
            var response = await _apiService.PostAsync<ApiLoginResponse>("auth/login", loginRequest);

            if (response != null && response.Success && !string.IsNullOrEmpty(response.Token))
            {
                // Salvar token e dados do usuário usando cache e localStorage
                var userInfo = new UserInfo
                {
                    Id = response.User.Id,
                    Nome = response.User.UserName,
                    Email = response.User.Email,
                    Username = response.User.UserName,
                    IsAtivo = true,
                    DataCadastro = DateTime.UtcNow,
                    UltimoAcesso = DateTime.UtcNow
                };

                await SaveAuthDataAsync(response.Token, userInfo);

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

            // Proteção contra JavaScript Interop durante renderização estática
            try
            {
                // Limpar LocalStorage
                await _localStorage.RemoveItemAsync(AUTH_TOKEN_KEY);
                await _localStorage.RemoveItemAsync(USER_INFO_KEY);
                LogLogoutSuccess(_logger);
            }
            catch (InvalidOperationException jsInteropEx) when (jsInteropEx.Message.Contains("JavaScript interop"))
            {
                // Durante renderização estática, cache já foi limpo
                Console.WriteLine("[LOCAL AUTH] JavaScript Interop não disponível - logout realizado apenas em cache");
                LogLogoutSuccess(_logger);
            }
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

            // Proteção contra JavaScript Interop durante renderização estática
            try
            {
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
            catch (InvalidOperationException jsInteropEx) when (jsInteropEx.Message.Contains("JavaScript interop"))
            {
                // Durante renderização estática, não podemos acessar localStorage
                // Usar apenas cache em memória
                Console.WriteLine($"[LOCAL AUTH] JavaScript Interop não disponível durante renderização - usando cache: {_isAuthenticated}");
                return _isAuthenticated;
            }
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

            // Proteção contra JavaScript Interop durante renderização estática
            try
            {
                // Buscar do LocalStorage se não estiver em cache
                var userInfo = await _localStorage.GetItemAsync<UserInfo>(USER_INFO_KEY);
                if (userInfo != null)
                {
                    _cachedUserInfo = userInfo; // Atualizar cache
                    Console.WriteLine($"[LOCAL AUTH] GetCurrentUserAsync - LocalStorage: {userInfo.Nome}");
                }

                return userInfo;
            }
            catch (InvalidOperationException jsInteropEx) when (jsInteropEx.Message.Contains("JavaScript interop"))
            {
                // Durante renderização estática, retornar dados do cache
                Console.WriteLine($"[LOCAL AUTH] JavaScript Interop não disponível - retornando cache: {_cachedUserInfo?.Nome ?? "null"}");
                return _cachedUserInfo;
            }
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

            // Proteção contra JavaScript Interop durante renderização estática
            try
            {
                // Salvar no LocalStorage para persistência
                await _localStorage.SetItemAsync(AUTH_TOKEN_KEY, token);
                await _localStorage.SetItemAsync(USER_INFO_KEY, userInfo);

                // Verificação imediata para confirmar que foi salvo
                var savedToken = await _localStorage.GetItemAsync<string>(AUTH_TOKEN_KEY);
                var savedUser = await _localStorage.GetItemAsync<UserInfo>(USER_INFO_KEY);

                Console.WriteLine($"[LOCAL AUTH] Dados salvos com sucesso - Token recuperado: {!string.IsNullOrEmpty(savedToken)}, User recuperado: {savedUser != null}");
                LogLoginSuccess(_logger, userInfo.Email);
            }
            catch (InvalidOperationException jsInteropEx) when (jsInteropEx.Message.Contains("JavaScript interop"))
            {
                // Durante renderização estática, dados já estão no cache
                Console.WriteLine($"[LOCAL AUTH] JavaScript Interop não disponível - dados mantidos em cache");
                LogLoginSuccess(_logger, userInfo.Email);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOCAL AUTH] ERRO ao salvar dados: {ex.Message}");
            LogLoginError(_logger, userInfo.Email, ex);
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            // Verificar cache primeiro
            if (_isAuthenticated && !string.IsNullOrEmpty(_cachedToken))
            {
                return _cachedToken;
            }

            // Proteção contra JavaScript Interop durante renderização estática
            try
            {
                // Buscar no LocalStorage
                var token = await _localStorage.GetItemAsync<string>(AUTH_TOKEN_KEY);
                
                if (!string.IsNullOrEmpty(token))
                {
                    _cachedToken = token;
                    return token;
                }

                return null;
            }
            catch (InvalidOperationException jsInteropEx) when (jsInteropEx.Message.Contains("JavaScript interop"))
            {
                // Durante renderização estática, retornar token do cache
                Console.WriteLine($"[LOCAL AUTH] JavaScript Interop não disponível - retornando token do cache: {!string.IsNullOrEmpty(_cachedToken)}");
                return _cachedToken;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOCAL AUTH] Erro ao obter access token: {ex.Message}");
            return null;
        }
    }
}
