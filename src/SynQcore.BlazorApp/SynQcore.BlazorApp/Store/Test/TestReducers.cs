using Fluxor;

namespace SynQcore.BlazorApp.Store.Test;

/// <summary>
/// Reducers de teste para verificar se Fluxor está funcionando
/// </summary>
public static class TestReducers
{
    /// <summary>
    /// Reducer para incrementar contador
    /// </summary>
    [ReducerMethod]
    public static TestState ReduceIncrementCounter(TestState state, TestActions.IncrementCounterAction action)
    {
        Console.WriteLine($"[TestReducer] ✅ IncrementCounter executado! Counter: {state.Counter} -> {state.Counter + 1}");

        return state with
        {
            Counter = state.Counter + 1,
            Message = $"Contador incrementado para {state.Counter + 1}"
        };
    }

    /// <summary>
    /// Reducer para definir mensagem
    /// </summary>
    [ReducerMethod]
    public static TestState ReduceSetMessage(TestState state, TestActions.SetMessageAction action)
    {
        Console.WriteLine($"[TestReducer] ✅ SetMessage executado! Message: {action.Message}");

        return state with
        {
            Message = action.Message
        };
    }
}
