using Fluxor;

namespace SynQcore.BlazorApp.Store.User;

/// <summary>
/// Estado global do usuário
/// </summary>
[FeatureState]
public record UserState
{
    /// <summary>
    /// Usuário está autenticado
    /// </summary>
    public bool IsAuthenticated { get; init; }

    /// <summary>
    /// Informações do usuário atual
    /// </summary>
    public UserInfo? CurrentUser { get; init; }

    /// <summary>
    /// Token de autenticação
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Token de refresh
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Data de expiração do token
    /// </summary>
    public DateTime? TokenExpiresAt { get; init; }

    /// <summary>
    /// Permissões do usuário
    /// </summary>
    public List<string> Permissions { get; init; } = new();

    /// <summary>
    /// Preferências do usuário
    /// </summary>
    public UserPreferences Preferences { get; init; } = new();

    /// <summary>
    /// Status do login
    /// </summary>
    public LoginStatus Status { get; init; } = LoginStatus.LoggedOut;

    /// <summary>
    /// Último erro de autenticação
    /// </summary>
    public string? LastAuthError { get; init; }

    /// <summary>
    /// Tentativas de login falharam
    /// </summary>
    public int FailedLoginAttempts { get; init; }

    /// <summary>
    /// Último login realizado
    /// </summary>
    public DateTime? LastLoginAt { get; init; }
}

/// <summary>
/// Informações do usuário
/// </summary>
public record UserInfo
{
    public string Id { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? FotoUrl { get; init; }
    public string Cargo { get; init; } = string.Empty;
    public string Departamento { get; init; } = string.Empty;
    public string? Telefone { get; init; }
    public DateTime DataCadastro { get; init; }
    public DateTime? UltimoAcesso { get; init; }
    public bool IsAtivo { get; init; } = true;
    public List<string> Roles { get; init; } = new();
}

/// <summary>
/// Preferências do usuário
/// </summary>
public record UserPreferences
{
    public string Tema { get; init; } = "light";
    public string Idioma { get; init; } = "pt-BR";
    public bool NotificacoesPorEmail { get; init; } = true;
    public bool NotificacoesPush { get; init; } = true;
    public bool NotificacoesDesktop { get; init; }
    public int TamanhoFonte { get; init; } = 14;
    public bool ModoEscuro { get; init; }
    public bool ReducirAnimacoes { get; init; }
    public bool SomNotificacoes { get; init; } = true;
    public Dictionary<string, object> ConfiguracoesCustomizadas { get; init; } = new();
}

/// <summary>
/// Status do login
/// </summary>
public enum LoginStatus
{
    LoggedOut,
    LoggingIn,
    LoggedIn,
    LoginFailed,
    TokenExpired,
    RefreshingToken
}
