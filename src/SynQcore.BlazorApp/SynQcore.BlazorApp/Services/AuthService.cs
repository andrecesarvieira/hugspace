using System.Text.Json.Serialization;
using SynQcore.BlazorApp.Services.StateManagement;
using SynQcore.BlazorApp.Store.User;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Modelos para requisições de autenticação
/// </summary>
public record LoginRequest(string Email, string Password, bool RememberMe = true);

public record ApiLoginResponse(
    bool Success,
    string Message,
    string Token,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("expiresAt")] DateTime ExpiresAt,
    ApiUserInfo User
);

public record ApiUserInfo(
    string Id,
    string UserName,
    string Email
);

public record LoginResponse(string AccessToken, string? RefreshToken, DateTime ExpiresAt, UserInfo User);
public record RefreshTokenRequest(string RefreshToken);

/// <summary>
/// Serviço de autenticação integrado com API
/// </summary>
public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task<bool> ValidateTokenAsync();
}

/// <summary>
/// Implementação do serviço de autenticação
/// </summary>
public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly StateManager _stateManager;
    private readonly ILocalAuthService _localAuthService;

    public AuthService(
        IApiService apiService,
        StateManager stateManager,
        ILocalAuthService localAuthService)
    {
        _apiService = apiService;
        _stateManager = stateManager;
        _localAuthService = localAuthService;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            // Inicia processo de login usando StateManager
            await _stateManager.User.LoginAsync(email, password);

            var request = new LoginRequest(email, password, true);
            var startTime = DateTime.Now; // Mover para escopo externo

            // Faz requisição real para a API SynQcore
            var apiResponse = await _apiService.PostAsync<ApiLoginResponse>("auth/login", request);

            if (apiResponse != null && apiResponse.Success)
            {
                Console.WriteLine($"[AuthService] API Response recebida - Success: {apiResponse.Success}");
                Console.WriteLine($"[AuthService] Token: {apiResponse.Token?.Substring(0, Math.Min(20, apiResponse.Token.Length))}...");
                Console.WriteLine($"[AuthService] User Info - Id: {apiResponse.User?.Id}, UserName: {apiResponse.User?.UserName}, Email: {apiResponse.User?.Email}");

                // Configura o header de autorização
                if (!string.IsNullOrEmpty(apiResponse.Token))
                {
                    _apiService.SetAuthorizationHeader(apiResponse.Token);
                    Console.WriteLine($"[AuthService] Token configurado: {apiResponse.Token.Substring(0, 10)}...");
                }

                // Mapeia a resposta da API para o modelo interno
                var userInfo = new UserInfo
                {
                    Id = apiResponse.User?.Id ?? string.Empty,
                    Nome = apiResponse.User?.UserName ?? string.Empty,
                    Email = apiResponse.User?.Email ?? string.Empty,
                    Username = apiResponse.User?.UserName ?? string.Empty,
                    Cargo = "Administrador", // Valor padrão por enquanto
                    Departamento = "Tecnologia da Informação", // Valor padrão por enquanto
                    DataCadastro = DateTime.Now,
                    IsAtivo = true,
                    Roles = new List<string> { "Admin" }
                };

                // Atualiza o estado global usando StateManager
                Console.WriteLine($"[AuthService] Fazendo login via StateManager para: {userInfo.Nome}");
                _stateManager.User.SetAuthenticatedUser(userInfo, apiResponse.Token ?? string.Empty, string.Empty);
                Console.WriteLine($"[AuthService] Login via StateManager concluído com sucesso");

                // Sincronizar com LocalAuth como backup
                try
                {
                    await _localAuthService.SaveAuthDataAsync(apiResponse.Token ?? string.Empty, userInfo);
                    Console.WriteLine("[AuthService] Sincronizado com LocalAuth");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AuthService] Erro ao sincronizar com LocalAuth: {ex.Message}");
                }

                // Mostra mensagem de sucesso usando StateManager
                _stateManager.UI.AddNotification($"Bem-vindo de volta, {userInfo.Nome}!");

                return true;
            }
            else
            {
                // await _stateManager.User.SetLoginErrorAsync("Credenciais inválidas");
                Console.WriteLine("[AuthService] Credenciais inválidas");
                return false;
            }
        }
        catch (Exception ex)
        {
            // await _stateManager.User.SetLoginErrorAsync($"Erro no login: {ex.Message}");
            Console.WriteLine($"[AuthService] Erro no login: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Atualiza o token de acesso
    /// </summary>
    public Task<bool> RefreshTokenAsync()
    {
        try
        {
            // Inicia processo de refresh token usando StateManager
            // TODO: Implementar lógica real de refresh token com API

            var newToken = "new-jwt-token-" + DateTime.Now.Ticks;
            var expiresAt = DateTime.Now.AddHours(8);

            // _apiService.SetAuthorizationHeader(newToken);

            // Usar StateManager ao invés do Fluxor
            // TODO: Implementar método apropriado no StateManager
            Console.WriteLine($"[AuthService] Token atualizado: {newToken.Substring(0, 10)}...");

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Erro ao atualizar token: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    public Task LogoutAsync()
    {
        try
        {
            // Tenta fazer logout na API (opcional)
            try
            {
                // await _apiService.PostAsync("api/auth/logout", new { });
                Console.WriteLine("[AuthService] Logout na API (simulado)");
            }
            catch
            {
                // Ignora erros de logout na API
            }

            // Limpa o header de autorização
            // _apiService.ClearAuthorizationHeader();
            Console.WriteLine("[AuthService] Header de autorização limpo (simulado)");

            // Atualiza o estado global usando StateManager
            _stateManager.User.Logout();

            _stateManager.UI.AddNotification("Logout realizado com sucesso!");
        }
        catch (Exception ex)
        {
            _stateManager.UI.AddNotification($"Erro ao fazer logout: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    public Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            // Retornar informações simuladas do usuário sem delay
            var userInfo = new UserInfo
            {
                Id = "1",
                Nome = "Usuário Teste",
                Email = "admin@synqcore.com",
                Username = "admin",
                Cargo = "Administrador",
                Departamento = "TI"
            };
            
            // var userInfo = await _apiService.GetAsync<UserInfo>("api/auth/me");

            if (userInfo != null)
            {
                // Usar StateManager ao invés do Fluxor
                // TODO: Implementar método para atualizar info do usuário no StateManager
                Console.WriteLine($"[AuthService] Informações do usuário atualizadas: {userInfo.Nome}");
            }

            return Task.FromResult<UserInfo?>(userInfo);
        }
        catch (Exception ex)
        {
            _stateManager.UI.AddNotification($"Erro ao carregar usuário: {ex.Message}");
            return Task.FromResult<UserInfo?>(null);
        }
    }

    /// <summary>
    /// Valida se o token atual é válido
    /// </summary>
    public async Task<bool> ValidateTokenAsync()
    {
        try
        {
            var response = await _apiService.GetAsync("api/auth/validate");
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }
}
