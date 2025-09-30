namespace SynQcore.BlazorApp.Store.User;

/// <summary>
/// Ações relacionadas ao estado do usuário
/// </summary>
public static class UserActions
{
    /// <summary>
    /// Ação para iniciar processo de login
    /// </summary>
    public record StartLoginAction(string Email, string Password);

    /// <summary>
    /// Ação para login bem-sucedido
    /// </summary>
    public record LoginSuccessAction(
        UserInfo User,
        string AccessToken,
        string? RefreshToken = null,
        DateTime? ExpiresAt = null
    );

    /// <summary>
    /// Ação para login falhado
    /// </summary>
    public record LoginFailureAction(string ErrorMessage);

    /// <summary>
    /// Ação para logout
    /// </summary>
    public record LogoutAction();

    /// <summary>
    /// Ação para iniciar refresh do token
    /// </summary>
    public record StartRefreshTokenAction();

    /// <summary>
    /// Ação para refresh do token bem-sucedido
    /// </summary>
    public record RefreshTokenSuccessAction(
        string AccessToken,
        string? RefreshToken = null,
        DateTime? ExpiresAt = null
    );

    /// <summary>
    /// Ação para refresh do token falhado
    /// </summary>
    public record RefreshTokenFailureAction(string ErrorMessage);

    /// <summary>
    /// Ação para atualizar informações do usuário
    /// </summary>
    public record UpdateUserInfoAction(UserInfo User);

    /// <summary>
    /// Ação para atualizar preferências do usuário
    /// </summary>
    public record UpdateUserPreferencesAction(UserPreferences Preferences);

    /// <summary>
    /// Ação para adicionar permissão
    /// </summary>
    public record AddPermissionAction(string Permission);

    /// <summary>
    /// Ação para remover permissão
    /// </summary>
    public record RemovePermissionAction(string Permission);

    /// <summary>
    /// Ação para definir permissões
    /// </summary>
    public record SetPermissionsAction(List<string> Permissions);

    /// <summary>
    /// Ação para limpar erro de autenticação
    /// </summary>
    public record ClearAuthErrorAction();

    /// <summary>
    /// Ação para incrementar tentativas de login falhadas
    /// </summary>
    public record IncrementFailedLoginAttemptsAction();

    /// <summary>
    /// Ação para resetar tentativas de login falhadas
    /// </summary>
    public record ResetFailedLoginAttemptsAction();

    /// <summary>
    /// Ação para atualizar foto do usuário
    /// </summary>
    public record UpdateUserPhotoAction(string PhotoUrl);

    /// <summary>
    /// Ação para marcar último acesso
    /// </summary>
    public record UpdateLastAccessAction(DateTime LastAccess);

    /// <summary>
    /// Ação para alternar tema
    /// </summary>
    public record ToggleThemeAction();

    /// <summary>
    /// Ação para definir configuração personalizada
    /// </summary>
    public record SetCustomSettingAction(string Key, object Value);

    /// <summary>
    /// Ação para remover configuração personalizada
    /// </summary>
    public record RemoveCustomSettingAction(string Key);
}
