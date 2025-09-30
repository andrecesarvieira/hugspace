namespace SynQcore.BlazorApp.Store.Navigation;

/// <summary>
/// Ações relacionadas à navegação
/// </summary>
public static class NavigationActions
{
    /// <summary>
    /// Ação para navegar para uma página
    /// </summary>
    public record NavigateToAction(
        string Url,
        string? Title = null,
        Dictionary<string, string>? Parameters = null
    );

    /// <summary>
    /// Ação para voltar na navegação
    /// </summary>
    public record NavigateBackAction();

    /// <summary>
    /// Ação para definir menu ativo
    /// </summary>
    public record SetActiveMenuAction(string MenuId, string? SubmenuId = null);

    /// <summary>
    /// Ação para definir título da página
    /// </summary>
    public record SetPageTitleAction(string Title);

    /// <summary>
    /// Ação para definir metadados da página
    /// </summary>
    public record SetPageMetadataAction(PageMetadata Metadata);

    /// <summary>
    /// Ação para iniciar navegação
    /// </summary>
    public record StartNavigationAction();

    /// <summary>
    /// Ação para finalizar navegação
    /// </summary>
    public record EndNavigationAction();

    /// <summary>
    /// Ação para abrir nova tab
    /// </summary>
    public record OpenTabAction(
        string Url,
        string Title,
        string? Icon = null,
        bool IsPinned = false
    );

    /// <summary>
    /// Ação para fechar tab
    /// </summary>
    public record CloseTabAction(string TabId);

    /// <summary>
    /// Ação para ativar tab
    /// </summary>
    public record ActivateTabAction(string TabId);

    /// <summary>
    /// Ação para fixar/desafixar tab
    /// </summary>
    public record ToggleTabPinAction(string TabId);

    /// <summary>
    /// Ação para marcar tab como modificada
    /// </summary>
    public record MarkTabAsChangedAction(string TabId, bool HasChanges = true);

    /// <summary>
    /// Ação para atualizar título da tab
    /// </summary>
    public record UpdateTabTitleAction(string TabId, string Title);

    /// <summary>
    /// Ação para limpar histórico
    /// </summary>
    public record ClearHistoryAction();

    /// <summary>
    /// Ação para adicionar entrada personalizada ao histórico
    /// </summary>
    public record AddHistoryEntryAction(NavigationEntry Entry);

    /// <summary>
    /// Ação para remover entrada do histórico
    /// </summary>
    public record RemoveHistoryEntryAction(string Url);

    /// <summary>
    /// Ação para fechar todas as tabs
    /// </summary>
    public record CloseAllTabsAction();

    /// <summary>
    /// Ação para fechar outras tabs
    /// </summary>
    public record CloseOtherTabsAction(string ExceptTabId);

    /// <summary>
    /// Ação para duplicar tab
    /// </summary>
    public record DuplicateTabAction(string TabId);

    /// <summary>
    /// Ação para reordenar tabs
    /// </summary>
    public record ReorderTabsAction(List<string> TabOrder);
}
