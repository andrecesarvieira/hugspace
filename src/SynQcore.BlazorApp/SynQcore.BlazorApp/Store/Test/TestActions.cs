namespace SynQcore.BlazorApp.Store.Test;

/// <summary>
/// Ações de teste para verificar se Fluxor está funcionando
/// </summary>
public static class TestActions
{
    /// <summary>
    /// Ação para incrementar contador
    /// </summary>
    public record IncrementCounterAction;

    /// <summary>
    /// Ação para definir mensagem
    /// </summary>
    public record SetMessageAction(string Message);
}
