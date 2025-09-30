using Fluxor;
using SynQcore.BlazorApp.Store.User;
using SynQcore.BlazorApp.Store.UI;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Modelos para requisições de autenticação
/// </summary>
public record LoginRequest(string Email, string Password);
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
    private readonly IDispatcher _dispatcher;
    private readonly IStateInitializationService _stateInitialization;

    public AuthService(
        IApiService apiService,
        IDispatcher dispatcher,
        IStateInitializationService stateInitialization)
    {
        _apiService = apiService;
        _dispatcher = dispatcher;
        _stateInitialization = stateInitialization;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            _dispatcher.Dispatch(new UserActions.StartLoginAction(email, password));

            var request = new LoginRequest(email, password);

            // Faz requisição para a API SynQcore
            var response = await _apiService.PostAsync<LoginResponse>("api/auth/login", request);

            if (response != null)
            {
                // Configura o header de autorização
                _apiService.SetAuthorizationHeader(response.AccessToken);

                // Atualiza o estado global
                _dispatcher.Dispatch(new UserActions.LoginSuccessAction(
                    response.User,
                    response.AccessToken,
                    response.RefreshToken,
                    response.ExpiresAt
                ));

                _dispatcher.Dispatch(new UIActions.ShowSuccessMessageAction(
                    "Login realizado",
                    $"Bem-vindo, {response.User.Nome}!"
                ));

                return true;
            }
            else
            {
                _dispatcher.Dispatch(new UserActions.LoginFailureAction("Credenciais inválidas"));
                return false;
            }
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new UserActions.LoginFailureAction($"Erro no login: {ex.Message}"));
            return false;
        }
    }

    /// <summary>
    /// Atualiza o token de acesso
    /// </summary>
    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            _dispatcher.Dispatch(new UserActions.StartRefreshTokenAction());

            // Aqui você precisaria implementar a lógica de refresh token
            // Por enquanto, vamos simular uma resposta bem-sucedida

            await Task.Delay(1000); // Simula requisição

            var newToken = "new-jwt-token-" + DateTime.Now.Ticks;
            var expiresAt = DateTime.Now.AddHours(8);

            _apiService.SetAuthorizationHeader(newToken);

            _dispatcher.Dispatch(new UserActions.RefreshTokenSuccessAction(
                newToken,
                null,
                expiresAt
            ));

            return true;
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new UserActions.RefreshTokenFailureAction($"Erro ao atualizar token: {ex.Message}"));
            return false;
        }
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            // Tenta fazer logout na API (opcional)
            try
            {
                await _apiService.PostAsync("api/auth/logout", new { });
            }
            catch
            {
                // Ignora erros de logout na API
            }

            // Limpa o header de autorização
            _apiService.ClearAuthorizationHeader();

            // Atualiza o estado global
            _dispatcher.Dispatch(new UserActions.LogoutAction());

            _dispatcher.Dispatch(new UIActions.ShowInfoMessageAction(
                "Logout realizado",
                "Você foi desconectado com sucesso"
            ));
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new UIActions.ShowErrorMessageAction(
                "Erro no logout",
                $"Erro ao fazer logout: {ex.Message}"
            ));
        }
    }

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var userInfo = await _apiService.GetAsync<UserInfo>("api/auth/me");

            if (userInfo != null)
            {
                _dispatcher.Dispatch(new UserActions.UpdateUserInfoAction(userInfo));
                _dispatcher.Dispatch(new UserActions.UpdateLastAccessAction(DateTime.Now));
            }

            return userInfo;
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new UIActions.ShowErrorMessageAction(
                "Erro ao carregar usuário",
                $"Não foi possível carregar informações do usuário: {ex.Message}"
            ));
            return null;
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

    /// <summary>
    /// Login demo para testes (simula API)
    /// </summary>
    public async Task<bool> LoginDemoAsync()
    {
        try
        {
            _dispatcher.Dispatch(new UserActions.StartLoginAction("demo@synqcore.com", "demo"));

            // Simula delay de API
            await Task.Delay(1500);

            var demoUser = new UserInfo
            {
                Id = Guid.NewGuid().ToString(),
                Nome = "Usuário Demo",
                Email = "demo@synqcore.com",
                Username = "demo",
                Cargo = "Desenvolvedor Frontend",
                Departamento = "Tecnologia da Informação",
                DataCadastro = DateTime.Now.AddDays(-30),
                IsAtivo = true,
                Roles = new List<string> { "User", "Developer", "Admin" },
                FotoUrl = "https://api.dicebear.com/7.x/avatars/svg?seed=demo"
            };

            var token = "demo-jwt-token-" + DateTime.Now.Ticks;
            var expiresAt = DateTime.Now.AddHours(8);

            _apiService.SetAuthorizationHeader(token);

            _dispatcher.Dispatch(new UserActions.LoginSuccessAction(
                demoUser,
                token,
                null,
                expiresAt
            ));

            _dispatcher.Dispatch(new UIActions.ShowSuccessMessageAction(
                "Login Demo realizado",
                $"Bem-vindo ao modo demonstração, {demoUser.Nome}!"
            ));

            return true;
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new UserActions.LoginFailureAction($"Erro no login demo: {ex.Message}"));
            return false;
        }
    }
}
