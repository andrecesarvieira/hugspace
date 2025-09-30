using Fluxor;

namespace SynQcore.BlazorApp.Store.Test;

/// <summary>
/// Estado de teste para verificar se Fluxor está funcionando
/// </summary>
[FeatureState]
public record TestState
{
    /// <summary>
    /// Contador para testar se o Fluxor funciona
    /// </summary>
    public int Counter { get; init; }

    /// <summary>
    /// Mensagem de teste
    /// </summary>
    public string Message { get; init; } = "Estado inicial";
}
