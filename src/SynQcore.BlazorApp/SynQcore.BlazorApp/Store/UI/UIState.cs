using Fluxor;

namespace SynQcore.BlazorApp.Store.UI;

/// <summary>
/// Estado global da interface do usuário
/// </summary>
[FeatureState]
public record UIState
{
    /// <summary>
    /// Indica se está carregando
    /// </summary>
    public bool IsLoading { get; init; }

    /// <summary>
    /// Menu sidebar expandido
    /// </summary>
    public bool IsSidebarExpanded { get; init; } = true;

    /// <summary>
    /// Mensagens de notificação
    /// </summary>
    public List<NotificationMessage> Notifications { get; init; } = new();

    /// <summary>
    /// Modal aberto atualmente
    /// </summary>
    public string? CurrentModal { get; init; }

    /// <summary>
    /// Breadcrumb atual
    /// </summary>
    public List<BreadcrumbItem> Breadcrumb { get; init; } = new();

    /// <summary>
    /// Status da conexão com a API
    /// </summary>
    public ConnectionStatus ApiConnectionStatus { get; init; } = ConnectionStatus.Connected;

    /// <summary>
    /// Configurações de acessibilidade
    /// </summary>
    public AccessibilitySettings Accessibility { get; init; } = new();
}

/// <summary>
/// Mensagem de notificação
/// </summary>
public record NotificationMessage
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public NotificationType Type { get; init; } = NotificationType.Info;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public bool IsRead { get; init; }
    public int? AutoHideAfterMs { get; init; } = 5000;
}

/// <summary>
/// Item do breadcrumb
/// </summary>
public record BreadcrumbItem
{
    public string Label { get; init; } = string.Empty;
    public string? Url { get; init; }
    public string? Icon { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Configurações de acessibilidade
/// </summary>
public record AccessibilitySettings
{
    public bool HighContrast { get; init; }
    public bool ReducedMotion { get; init; }
    public double FontSizeScale { get; init; } = 1.0;
    public bool ScreenReaderOptimized { get; init; }
}

/// <summary>
/// Tipos de notificação
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// Status da conexão
/// </summary>
public enum ConnectionStatus
{
    Connected,
    Connecting,
    Disconnected,
    Error
}
