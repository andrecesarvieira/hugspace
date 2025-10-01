namespace SynQcore.BlazorApp.Store.UI;

/// <summary>
/// Ações relacionadas ao estado da UI
/// </summary>
public static class UIActions
{
    /// <summary>
    /// Ação para iniciar carregamento
    /// </summary>
    public record SetLoadingAction(bool IsLoading);

    /// <summary>
    /// Ação para expandir/recolher sidebar
    /// </summary>
    public record ToggleSidebarAction();

    /// <summary>
    /// Ação para adicionar notificação
    /// </summary>
    public record AddNotificationAction(
        string Title,
        string Message,
        NotificationType Type = NotificationType.Info,
        int? AutoHideAfterMs = 5000
    );

    /// <summary>
    /// Ação para remover notificação
    /// </summary>
    public record RemoveNotificationAction(string NotificationId);

    /// <summary>
    /// Ação para marcar notificação como lida
    /// </summary>
    public record MarkNotificationAsReadAction(string NotificationId);

    /// <summary>
    /// Ação para limpar todas as notificações
    /// </summary>
    public record ClearAllNotificationsAction();

    /// <summary>
    /// Ação para abrir modal
    /// </summary>
    public record OpenModalAction(string ModalName);

    /// <summary>
    /// Ação para fechar modal
    /// </summary>
    public record CloseModalAction();

    /// <summary>
    /// Ação para definir breadcrumb
    /// </summary>
    public record SetBreadcrumbAction(List<BreadcrumbItem> Items);

    /// <summary>
    /// Ação para adicionar item ao breadcrumb
    /// </summary>
    public record AddBreadcrumbItemAction(BreadcrumbItem Item);

    /// <summary>
    /// Ação para definir status da conexão API
    /// </summary>
    public record SetApiConnectionStatusAction(ConnectionStatus Status);

    /// <summary>
    /// Ação para atualizar configurações de acessibilidade
    /// </summary>
    public record UpdateAccessibilitySettingsAction(AccessibilitySettings Settings);

    /// <summary>
    /// Ação para mostrar mensagem de sucesso
    /// </summary>
    public record ShowSuccessMessageAction(string Title, string Message);

    /// <summary>
    /// Ação para mostrar mensagem de erro
    /// </summary>
    public record ShowErrorMessageAction(string Title, string Message);

    /// <summary>
    /// Ação para mostrar mensagem de aviso
    /// </summary>
    public record ShowWarningMessageAction(string Title, string Message);

    /// <summary>
    /// Ação para mostrar mensagem de informação
    /// </summary>
    public record ShowInfoMessageAction(string Title, string Message);
}
