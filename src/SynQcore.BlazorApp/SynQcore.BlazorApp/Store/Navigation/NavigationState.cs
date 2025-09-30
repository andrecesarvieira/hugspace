using Fluxor;

namespace SynQcore.BlazorApp.Store.Navigation;

/// <summary>
/// Estado global de navegação
/// </summary>
[FeatureState]
public record NavigationState
{
    /// <summary>
    /// Página atual
    /// </summary>
    public string CurrentPage { get; init; } = "/";

    /// <summary>
    /// Página anterior
    /// </summary>
    public string? PreviousPage { get; init; }

    /// <summary>
    /// Parâmetros da página atual
    /// </summary>
    public Dictionary<string, string> PageParameters { get; init; } = new();

    /// <summary>
    /// Histórico de navegação
    /// </summary>
    public List<NavigationEntry> History { get; init; } = new();

    /// <summary>
    /// Menu ativo
    /// </summary>
    public string? ActiveMenu { get; init; }

    /// <summary>
    /// Submenu ativo
    /// </summary>
    public string? ActiveSubmenu { get; init; }

    /// <summary>
    /// Título da página atual
    /// </summary>
    public string PageTitle { get; init; } = "SynQcore";

    /// <summary>
    /// Metadados da página
    /// </summary>
    public PageMetadata Metadata { get; init; } = new();

    /// <summary>
    /// Indica se está navegando
    /// </summary>
    public bool IsNavigating { get; init; }

    /// <summary>
    /// Tabs abertas
    /// </summary>
    public List<TabInfo> OpenTabs { get; init; } = new();

    /// <summary>
    /// Tab ativa
    /// </summary>
    public string? ActiveTab { get; init; }
}

/// <summary>
/// Entrada no histórico de navegação
/// </summary>
public record NavigationEntry
{
    public string Url { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public Dictionary<string, string> Parameters { get; init; } = new();
}

/// <summary>
/// Metadados da página
/// </summary>
public record PageMetadata
{
    public string Description { get; init; } = string.Empty;
    public List<string> Keywords { get; init; } = new();
    public string? OgTitle { get; init; }
    public string? OgDescription { get; init; }
    public string? OgImage { get; init; }
    public Dictionary<string, string> CustomMetadata { get; init; } = new();
}

/// <summary>
/// Informações da tab
/// </summary>
public record TabInfo
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public bool IsPinned { get; init; }
    public bool HasChanges { get; init; }
    public DateTime LastAccessed { get; init; } = DateTime.Now;
}
