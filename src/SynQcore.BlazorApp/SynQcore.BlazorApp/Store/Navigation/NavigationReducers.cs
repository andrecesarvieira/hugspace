using Fluxor;

namespace SynQcore.BlazorApp.Store.Navigation;

/// <summary>
/// Reducers para o estado de navegação
/// </summary>
public static class NavigationReducers
{
    /// <summary>
    /// Reducer para navegar para uma página
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceNavigateToAction(NavigationState state, NavigationActions.NavigateToAction action)
    {
        var entry = new NavigationEntry
        {
            Url = action.Url,
            Title = action.Title ?? state.PageTitle,
            Parameters = action.Parameters ?? new Dictionary<string, string>()
        };

        var history = new List<NavigationEntry>(state.History) { entry };

        // Limita histórico a 50 entradas
        if (history.Count > 50)
        {
            history = history.Skip(history.Count - 50).ToList();
        }

        return state with
        {
            PreviousPage = state.CurrentPage,
            CurrentPage = action.Url,
            PageParameters = action.Parameters ?? new Dictionary<string, string>(),
            History = history,
            PageTitle = action.Title ?? state.PageTitle,
            IsNavigating = false
        };
    }

    /// <summary>
    /// Reducer para voltar na navegação
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceNavigateBackAction(NavigationState state, NavigationActions.NavigateBackAction action)
    {
        if (state.PreviousPage == null)
            return state;

        return state with
        {
            CurrentPage = state.PreviousPage,
            PreviousPage = state.History.LastOrDefault()?.Url,
            IsNavigating = false
        };
    }

    /// <summary>
    /// Reducer para definir menu ativo
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceSetActiveMenuAction(NavigationState state, NavigationActions.SetActiveMenuAction action)
    {
        return state with
        {
            ActiveMenu = action.MenuId,
            ActiveSubmenu = action.SubmenuId
        };
    }

    /// <summary>
    /// Reducer para definir título da página
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceSetPageTitleAction(NavigationState state, NavigationActions.SetPageTitleAction action)
    {
        return state with { PageTitle = action.Title };
    }

    /// <summary>
    /// Reducer para definir metadados da página
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceSetPageMetadataAction(NavigationState state, NavigationActions.SetPageMetadataAction action)
    {
        return state with { Metadata = action.Metadata };
    }

    /// <summary>
    /// Reducer para iniciar navegação
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceStartNavigationAction(NavigationState state, NavigationActions.StartNavigationAction action)
    {
        return state with { IsNavigating = true };
    }

    /// <summary>
    /// Reducer para finalizar navegação
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceEndNavigationAction(NavigationState state, NavigationActions.EndNavigationAction action)
    {
        return state with { IsNavigating = false };
    }

    /// <summary>
    /// Reducer para abrir nova tab
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceOpenTabAction(NavigationState state, NavigationActions.OpenTabAction action)
    {
        var newTab = new TabInfo
        {
            Url = action.Url,
            Title = action.Title,
            Icon = action.Icon,
            IsPinned = action.IsPinned
        };

        var tabs = new List<TabInfo>(state.OpenTabs) { newTab };

        return state with
        {
            OpenTabs = tabs,
            ActiveTab = newTab.Id
        };
    }

    /// <summary>
    /// Reducer para fechar tab
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceCloseTabAction(NavigationState state, NavigationActions.CloseTabAction action)
    {
        var tabs = state.OpenTabs.Where(t => t.Id != action.TabId).ToList();
        var activeTab = state.ActiveTab == action.TabId ? tabs.FirstOrDefault()?.Id : state.ActiveTab;

        return state with
        {
            OpenTabs = tabs,
            ActiveTab = activeTab
        };
    }

    /// <summary>
    /// Reducer para ativar tab
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceActivateTabAction(NavigationState state, NavigationActions.ActivateTabAction action)
    {
        var tabs = state.OpenTabs.Select(t =>
            t.Id == action.TabId ? t with { LastAccessed = DateTime.Now } : t
        ).ToList();

        return state with
        {
            ActiveTab = action.TabId,
            OpenTabs = tabs
        };
    }

    /// <summary>
    /// Reducer para fixar/desafixar tab
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceToggleTabPinAction(NavigationState state, NavigationActions.ToggleTabPinAction action)
    {
        var tabs = state.OpenTabs.Select(t =>
            t.Id == action.TabId ? t with { IsPinned = !t.IsPinned } : t
        ).ToList();

        return state with { OpenTabs = tabs };
    }

    /// <summary>
    /// Reducer para marcar tab como modificada
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceMarkTabAsChangedAction(NavigationState state, NavigationActions.MarkTabAsChangedAction action)
    {
        var tabs = state.OpenTabs.Select(t =>
            t.Id == action.TabId ? t with { HasChanges = action.HasChanges } : t
        ).ToList();

        return state with { OpenTabs = tabs };
    }

    /// <summary>
    /// Reducer para atualizar título da tab
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceUpdateTabTitleAction(NavigationState state, NavigationActions.UpdateTabTitleAction action)
    {
        var tabs = state.OpenTabs.Select(t =>
            t.Id == action.TabId ? t with { Title = action.Title } : t
        ).ToList();

        return state with { OpenTabs = tabs };
    }

    /// <summary>
    /// Reducer para limpar histórico
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceClearHistoryAction(NavigationState state, NavigationActions.ClearHistoryAction action)
    {
        return state with { History = new List<NavigationEntry>() };
    }

    /// <summary>
    /// Reducer para adicionar entrada personalizada ao histórico
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceAddHistoryEntryAction(NavigationState state, NavigationActions.AddHistoryEntryAction action)
    {
        var history = new List<NavigationEntry>(state.History) { action.Entry };

        // Limita histórico a 50 entradas
        if (history.Count > 50)
        {
            history = history.Skip(history.Count - 50).ToList();
        }

        return state with { History = history };
    }

    /// <summary>
    /// Reducer para remover entrada do histórico
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceRemoveHistoryEntryAction(NavigationState state, NavigationActions.RemoveHistoryEntryAction action)
    {
        var history = state.History.Where(h => h.Url != action.Url).ToList();
        return state with { History = history };
    }

    /// <summary>
    /// Reducer para fechar todas as tabs
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceCloseAllTabsAction(NavigationState state, NavigationActions.CloseAllTabsAction action)
    {
        return state with
        {
            OpenTabs = new List<TabInfo>(),
            ActiveTab = null
        };
    }

    /// <summary>
    /// Reducer para fechar outras tabs
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceCloseOtherTabsAction(NavigationState state, NavigationActions.CloseOtherTabsAction action)
    {
        var tabs = state.OpenTabs.Where(t => t.Id == action.ExceptTabId || t.IsPinned).ToList();

        return state with
        {
            OpenTabs = tabs,
            ActiveTab = action.ExceptTabId
        };
    }

    /// <summary>
    /// Reducer para duplicar tab
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceDuplicateTabAction(NavigationState state, NavigationActions.DuplicateTabAction action)
    {
        var originalTab = state.OpenTabs.FirstOrDefault(t => t.Id == action.TabId);
        if (originalTab == null)
            return state;

        var duplicatedTab = originalTab with
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"{originalTab.Title} (Cópia)",
            IsPinned = false,
            HasChanges = false,
            LastAccessed = DateTime.Now
        };

        var tabs = new List<TabInfo>(state.OpenTabs) { duplicatedTab };

        return state with
        {
            OpenTabs = tabs,
            ActiveTab = duplicatedTab.Id
        };
    }

    /// <summary>
    /// Reducer para reordenar tabs
    /// </summary>
    [ReducerMethod]
    public static NavigationState ReduceReorderTabsAction(NavigationState state, NavigationActions.ReorderTabsAction action)
    {
        var reorderedTabs = new List<TabInfo>();

        foreach (var tabId in action.TabOrder)
        {
            var tab = state.OpenTabs.FirstOrDefault(t => t.Id == tabId);
            if (tab != null)
            {
                reorderedTabs.Add(tab);
            }
        }

        return state with { OpenTabs = reorderedTabs };
    }
}
