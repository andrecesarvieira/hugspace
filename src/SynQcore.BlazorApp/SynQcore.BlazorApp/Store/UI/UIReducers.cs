using Fluxor;

namespace SynQcore.BlazorApp.Store.UI;

/// <summary>
/// Reducers para o estado da UI
/// </summary>
public static class UIReducers
{
    /// <summary>
    /// Reducer para definir estado de carregamento
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceSetLoadingAction(UIState state, UIActions.SetLoadingAction action)
    {
        return state with { IsLoading = action.IsLoading };
    }

    /// <summary>
    /// Reducer para alternar sidebar
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceToggleSidebarAction(UIState state, UIActions.ToggleSidebarAction action)
    {
        return state with { IsSidebarExpanded = !state.IsSidebarExpanded };
    }

    /// <summary>
    /// Reducer para adicionar notificação
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceAddNotificationAction(UIState state, UIActions.AddNotificationAction action)
    {
        var notification = new NotificationMessage
        {
            Title = action.Title,
            Message = action.Message,
            Type = action.Type,
            AutoHideAfterMs = action.AutoHideAfterMs
        };

        var notifications = new List<NotificationMessage>(state.Notifications) { notification };
        return state with { Notifications = notifications };
    }

    /// <summary>
    /// Reducer para remover notificação
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceRemoveNotificationAction(UIState state, UIActions.RemoveNotificationAction action)
    {
        var notifications = state.Notifications.Where(n => n.Id != action.NotificationId).ToList();
        return state with { Notifications = notifications };
    }

    /// <summary>
    /// Reducer para marcar notificação como lida
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceMarkNotificationAsReadAction(UIState state, UIActions.MarkNotificationAsReadAction action)
    {
        var notifications = state.Notifications.Select(n =>
            n.Id == action.NotificationId ? n with { IsRead = true } : n
        ).ToList();

        return state with { Notifications = notifications };
    }

    /// <summary>
    /// Reducer para limpar todas as notificações
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceClearAllNotificationsAction(UIState state, UIActions.ClearAllNotificationsAction action)
    {
        return state with { Notifications = new List<NotificationMessage>() };
    }

    /// <summary>
    /// Reducer para abrir modal
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceOpenModalAction(UIState state, UIActions.OpenModalAction action)
    {
        return state with { CurrentModal = action.ModalName };
    }

    /// <summary>
    /// Reducer para fechar modal
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceCloseModalAction(UIState state, UIActions.CloseModalAction action)
    {
        return state with { CurrentModal = null };
    }

    /// <summary>
    /// Reducer para definir breadcrumb
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceSetBreadcrumbAction(UIState state, UIActions.SetBreadcrumbAction action)
    {
        return state with { Breadcrumb = action.Items };
    }

    /// <summary>
    /// Reducer para adicionar item ao breadcrumb
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceAddBreadcrumbItemAction(UIState state, UIActions.AddBreadcrumbItemAction action)
    {
        var breadcrumb = new List<BreadcrumbItem>(state.Breadcrumb) { action.Item };
        return state with { Breadcrumb = breadcrumb };
    }

    /// <summary>
    /// Reducer para definir status da conexão API
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceSetApiConnectionStatusAction(UIState state, UIActions.SetApiConnectionStatusAction action)
    {
        return state with { ApiConnectionStatus = action.Status };
    }

    /// <summary>
    /// Reducer para atualizar configurações de acessibilidade
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceUpdateAccessibilitySettingsAction(UIState state, UIActions.UpdateAccessibilitySettingsAction action)
    {
        return state with { Accessibility = action.Settings };
    }

    /// <summary>
    /// Reducer para mostrar mensagem de sucesso
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceShowSuccessMessageAction(UIState state, UIActions.ShowSuccessMessageAction action)
    {
        var notification = new NotificationMessage
        {
            Title = action.Title,
            Message = action.Message,
            Type = NotificationType.Success,
            AutoHideAfterMs = 5000
        };

        var notifications = new List<NotificationMessage>(state.Notifications) { notification };
        return state with { Notifications = notifications };
    }

    /// <summary>
    /// Reducer para mostrar mensagem de erro
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceShowErrorMessageAction(UIState state, UIActions.ShowErrorMessageAction action)
    {
        var notification = new NotificationMessage
        {
            Title = action.Title,
            Message = action.Message,
            Type = NotificationType.Error,
            AutoHideAfterMs = 8000 // Erros ficam mais tempo visíveis
        };

        var notifications = new List<NotificationMessage>(state.Notifications) { notification };
        return state with { Notifications = notifications };
    }

    /// <summary>
    /// Reducer para mostrar mensagem de aviso
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceShowWarningMessageAction(UIState state, UIActions.ShowWarningMessageAction action)
    {
        var notification = new NotificationMessage
        {
            Title = action.Title,
            Message = action.Message,
            Type = NotificationType.Warning,
            AutoHideAfterMs = 6000
        };

        var notifications = new List<NotificationMessage>(state.Notifications) { notification };
        return state with { Notifications = notifications };
    }

    /// <summary>
    /// Reducer para mostrar mensagem de informação
    /// </summary>
    [ReducerMethod]
    public static UIState ReduceShowInfoMessageAction(UIState state, UIActions.ShowInfoMessageAction action)
    {
        var notification = new NotificationMessage
        {
            Title = action.Title,
            Message = action.Message,
            Type = NotificationType.Info,
            AutoHideAfterMs = 4000
        };

        var notifications = new List<NotificationMessage>(state.Notifications) { notification };
        return state with { Notifications = notifications };
    }
}
