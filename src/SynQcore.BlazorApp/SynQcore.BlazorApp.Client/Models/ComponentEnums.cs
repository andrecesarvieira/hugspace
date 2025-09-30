namespace SynQcore.BlazorApp.Client.Models;

/// <summary>
/// Variantes visuais dos botões corporativos
/// </summary>
public enum SynQButtonVariant
{
    /// <summary>
    /// Botão primário (ação principal)
    /// </summary>
    Primary,

    /// <summary>
    /// Botão secundário (ação secundária)
    /// </summary>
    Secondary,

    /// <summary>
    /// Botão com borda (outline)
    /// </summary>
    Outline,

    /// <summary>
    /// Botão fantasma (transparente)
    /// </summary>
    Ghost
}

/// <summary>
/// Tamanhos dos botões corporativos
/// </summary>
public enum SynQButtonSize
{
    /// <summary>
    /// Tamanho pequeno
    /// </summary>
    Small,

    /// <summary>
    /// Tamanho médio (padrão)
    /// </summary>
    Medium,

    /// <summary>
    /// Tamanho grande
    /// </summary>
    Large
}

/// <summary>
/// Variantes de cards corporativos
/// </summary>
public enum SynQCardVariant
{
    /// <summary>
    /// Card padrão
    /// </summary>
    Default,

    /// <summary>
    /// Card com elevação
    /// </summary>
    Elevated,

    /// <summary>
    /// Card com borda
    /// </summary>
    Outlined,

    /// <summary>
    /// Card preenchido
    /// </summary>
    Filled
}

/// <summary>
/// Tamanhos de avatares corporativos
/// </summary>
public enum SynQAvatarSize
{
    /// <summary>
    /// Tamanho extra pequeno (24px)
    /// </summary>
    ExtraSmall,

    /// <summary>
    /// Tamanho pequeno (32px)
    /// </summary>
    Small,

    /// <summary>
    /// Tamanho médio (40px)
    /// </summary>
    Medium,

    /// <summary>
    /// Tamanho grande (48px)
    /// </summary>
    Large,

    /// <summary>
    /// Tamanho extra grande (64px)
    /// </summary>
    ExtraLarge
}

/// <summary>
/// Variantes de badges corporativos
/// </summary>
public enum SynQBadgeVariant
{
    /// <summary>
    /// Badge primário
    /// </summary>
    Primary,

    /// <summary>
    /// Badge secundário
    /// </summary>
    Secondary,

    /// <summary>
    /// Badge de sucesso
    /// </summary>
    Success,

    /// <summary>
    /// Badge de aviso
    /// </summary>
    Warning,

    /// <summary>
    /// Badge de erro
    /// </summary>
    Error,

    /// <summary>
    /// Badge informativo
    /// </summary>
    Info
}

/// <summary>
/// Tipos de alertas corporativos
/// </summary>
public enum SynQAlertType
{
    /// <summary>
    /// Informação
    /// </summary>
    Info,

    /// <summary>
    /// Sucesso
    /// </summary>
    Success,

    /// <summary>
    /// Aviso
    /// </summary>
    Warning,

    /// <summary>
    /// Erro
    /// </summary>
    Error
}

/// <summary>
/// Posições de loading
/// </summary>
public enum SynQLoadingPosition
{
    /// <summary>
    /// Centro da tela
    /// </summary>
    Center,

    /// <summary>
    /// Topo da tela
    /// </summary>
    Top,

    /// <summary>
    /// Inline com o conteúdo
    /// </summary>
    Inline
}
