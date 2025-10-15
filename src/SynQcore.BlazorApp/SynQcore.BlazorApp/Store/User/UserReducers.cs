using Fluxor;

namespace SynQcore.BlazorApp.Store.User;

/// <summary>
/// Reducers para o estado do usuário
/// </summary>
public static class UserReducers
{
    /// <summary>
    /// Reducer para iniciar login
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceStartLoginAction(UserState state, UserActions.StartLoginAction action)
    {
        Console.WriteLine($"[UserReducer] TESTE: Processando StartLoginAction para: {action.Email}");
        Console.WriteLine($"[UserReducer] TESTE: Estado atual - IsAuthenticated: {state.IsAuthenticated}");

        var newState = state with
        {
            Status = LoginStatus.LoggingIn,
            LastAuthError = null
        };

        Console.WriteLine($"[UserReducer] TESTE: Estado após StartLogin - Status: {newState.Status}");
        return newState;
    }

    /// <summary>
    /// Reducer para login bem-sucedido
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceLoginSuccessAction(UserState state, UserActions.LoginSuccessAction action)
    {
        Console.WriteLine($"[UserReducer] ENTRADA: LoginSuccessAction - Estado atual IsAuthenticated: {state.IsAuthenticated}");
        Console.WriteLine($"[UserReducer] ENTRADA: LoginSuccessAction - Usuario: {action.User.Nome}");
        Console.WriteLine($"[UserReducer] ENTRADA: LoginSuccessAction - Token existe: {!string.IsNullOrEmpty(action.AccessToken)}");

        var newState = state with
        {
            IsAuthenticated = true,
            CurrentUser = action.User,
            AccessToken = action.AccessToken,
            RefreshToken = action.RefreshToken,
            TokenExpiresAt = action.ExpiresAt,
            Status = LoginStatus.LoggedIn,
            LastAuthError = null,
            FailedLoginAttempts = 0,
            LastLoginAt = DateTime.Now,
            Permissions = action.User.Roles
        };

        Console.WriteLine($"[UserReducer] SAÍDA: Estado criado - IsAuthenticated: {newState.IsAuthenticated}");
        Console.WriteLine($"[UserReducer] SAÍDA: CurrentUser: {newState.CurrentUser?.Nome}");
        Console.WriteLine($"[UserReducer] SAÍDA: Status: {newState.Status}");
        Console.WriteLine($"[UserReducer] SAÍDA: Token definido: {!string.IsNullOrEmpty(newState.AccessToken)}");

        return newState;
    }

    /// <summary>
    /// Reducer para login falhado
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceLoginFailureAction(UserState state, UserActions.LoginFailureAction action)
    {
        return state with
        {
            IsAuthenticated = false,
            Status = LoginStatus.LoginFailed,
            LastAuthError = action.ErrorMessage,
            FailedLoginAttempts = state.FailedLoginAttempts + 1
        };
    }

    /// <summary>
    /// Reducer para logout
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceLogoutAction(UserState state, UserActions.LogoutAction action)
    {
        return new UserState
        {
            Status = LoginStatus.LoggedOut,
            Preferences = state.Preferences // Mantém as preferências
        };
    }

    /// <summary>
    /// Reducer para iniciar refresh do token
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceStartRefreshTokenAction(UserState state, UserActions.StartRefreshTokenAction action)
    {
        return state with { Status = LoginStatus.RefreshingToken };
    }

    /// <summary>
    /// Reducer para refresh do token bem-sucedido
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceRefreshTokenSuccessAction(UserState state, UserActions.RefreshTokenSuccessAction action)
    {
        return state with
        {
            AccessToken = action.AccessToken,
            RefreshToken = action.RefreshToken ?? state.RefreshToken,
            TokenExpiresAt = action.ExpiresAt,
            Status = LoginStatus.LoggedIn,
            LastAuthError = null
        };
    }

    /// <summary>
    /// Reducer para refresh do token falhado
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceRefreshTokenFailureAction(UserState state, UserActions.RefreshTokenFailureAction action)
    {
        return state with
        {
            Status = LoginStatus.TokenExpired,
            LastAuthError = action.ErrorMessage,
            AccessToken = null,
            RefreshToken = null,
            TokenExpiresAt = null,
            IsAuthenticated = false
        };
    }

    /// <summary>
    /// Reducer para atualizar informações do usuário
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceUpdateUserInfoAction(UserState state, UserActions.UpdateUserInfoAction action)
    {
        return state with
        {
            CurrentUser = action.User,
            Permissions = action.User.Roles
        };
    }

    /// <summary>
    /// Reducer para atualizar preferências do usuário
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceUpdateUserPreferencesAction(UserState state, UserActions.UpdateUserPreferencesAction action)
    {
        return state with { Preferences = action.Preferences };
    }

    /// <summary>
    /// Reducer para adicionar permissão
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceAddPermissionAction(UserState state, UserActions.AddPermissionAction action)
    {
        if (state.Permissions.Contains(action.Permission))
            return state;

        var permissions = new List<string>(state.Permissions) { action.Permission };
        return state with { Permissions = permissions };
    }

    /// <summary>
    /// Reducer para remover permissão
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceRemovePermissionAction(UserState state, UserActions.RemovePermissionAction action)
    {
        var permissions = state.Permissions.Where(p => p != action.Permission).ToList();
        return state with { Permissions = permissions };
    }

    /// <summary>
    /// Reducer para definir permissões
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceSetPermissionsAction(UserState state, UserActions.SetPermissionsAction action)
    {
        return state with { Permissions = action.Permissions };
    }

    /// <summary>
    /// Reducer para limpar erro de autenticação
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceClearAuthErrorAction(UserState state, UserActions.ClearAuthErrorAction action)
    {
        return state with { LastAuthError = null };
    }

    /// <summary>
    /// Reducer para incrementar tentativas de login falhadas
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceIncrementFailedLoginAttemptsAction(UserState state, UserActions.IncrementFailedLoginAttemptsAction action)
    {
        return state with { FailedLoginAttempts = state.FailedLoginAttempts + 1 };
    }

    /// <summary>
    /// Reducer para resetar tentativas de login falhadas
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceResetFailedLoginAttemptsAction(UserState state, UserActions.ResetFailedLoginAttemptsAction action)
    {
        return state with { FailedLoginAttempts = 0 };
    }

    /// <summary>
    /// Reducer para atualizar foto do usuário
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceUpdateUserPhotoAction(UserState state, UserActions.UpdateUserPhotoAction action)
    {
        if (state.CurrentUser == null)
            return state;

        var updatedUser = state.CurrentUser with { FotoUrl = action.PhotoUrl };
        return state with { CurrentUser = updatedUser };
    }

    /// <summary>
    /// Reducer para marcar último acesso
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceUpdateLastAccessAction(UserState state, UserActions.UpdateLastAccessAction action)
    {
        if (state.CurrentUser == null)
            return state;

        var updatedUser = state.CurrentUser with { UltimoAcesso = action.LastAccess };
        return state with { CurrentUser = updatedUser };
    }

    /// <summary>
    /// Reducer para alternar tema
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceToggleThemeAction(UserState state, UserActions.ToggleThemeAction action)
    {
        var newTheme = state.Preferences.Tema == "light" ? "dark" : "light";
        var updatedPreferences = state.Preferences with
        {
            Tema = newTheme,
            ModoEscuro = newTheme == "dark"
        };

        return state with { Preferences = updatedPreferences };
    }

    /// <summary>
    /// Reducer para definir configuração personalizada
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceSetCustomSettingAction(UserState state, UserActions.SetCustomSettingAction action)
    {
        var customSettings = new Dictionary<string, object>(state.Preferences.ConfiguracoesCustomizadas)
        {
            [action.Key] = action.Value
        };

        var updatedPreferences = state.Preferences with { ConfiguracoesCustomizadas = customSettings };
        return state with { Preferences = updatedPreferences };
    }

    /// <summary>
    /// Reducer para remover configuração personalizada
    /// </summary>
    [ReducerMethod]
    public static UserState ReduceRemoveCustomSettingAction(UserState state, UserActions.RemoveCustomSettingAction action)
    {
        var customSettings = state.Preferences.ConfiguracoesCustomizadas
            .Where(kv => kv.Key != action.Key)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var updatedPreferences = state.Preferences with { ConfiguracoesCustomizadas = customSettings };
        return state with { Preferences = updatedPreferences };
    }
}
