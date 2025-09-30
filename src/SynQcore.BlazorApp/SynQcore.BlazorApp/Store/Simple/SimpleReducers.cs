using Fluxor;

namespace SynQcore.BlazorApp.Store.Simple;

/// <summary>
/// Reducers simples para testar Fluxor
/// </summary>
public static class SimpleReducers
{
    [ReducerMethod]
    public static SimpleState ReduceSimpleAction(SimpleState state, SimpleAction action)
    {
        Console.WriteLine($"[SIMPLE REDUCER] Processando SimpleAction - Message: {action.Message}");
        return state with { Message = action.Message };
    }

    [ReducerMethod]
    public static SimpleState ReduceIncrementAction(SimpleState state, IncrementCountAction action)
    {
        Console.WriteLine($"[SIMPLE REDUCER] Processando IncrementCountAction - Count atual: {state.Count}");
        return state with { Count = state.Count + 1 };
    }
}
