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
        Console.WriteLine($"🟢 [SIMPLE REDUCER] ReduceSimpleAction EXECUTADO!");
        Console.WriteLine($"🟢 [SIMPLE REDUCER] Estado atual: Message='{state.Message}', Count={state.Count}");
        Console.WriteLine($"🟢 [SIMPLE REDUCER] Nova mensagem: '{action.Message}'");
        
        var newState = state with { Message = action.Message };
        
        Console.WriteLine($"🟢 [SIMPLE REDUCER] Novo estado: Message='{newState.Message}', Count={newState.Count}");
        return newState;
    }

    [ReducerMethod]
    public static SimpleState ReduceIncrementAction(SimpleState state, IncrementCountAction action)
    {
        Console.WriteLine($"🟢 [SIMPLE REDUCER] ReduceIncrementAction EXECUTADO!");
        Console.WriteLine($"🟢 [SIMPLE REDUCER] Count atual: {state.Count}");
        
        var newState = state with { Count = state.Count + 1 };
        
        Console.WriteLine($"🟢 [SIMPLE REDUCER] Novo count: {newState.Count}");
        return newState;
    }
}
