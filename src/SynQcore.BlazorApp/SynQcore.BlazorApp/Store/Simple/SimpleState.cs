using Fluxor;

namespace SynQcore.BlazorApp.Store.Simple;

/// <summary>
/// Estado simples para testar Fluxor
/// </summary>
[FeatureState]
public record SimpleState
{
    public string Message { get; init; } = "Estado inicial";
    public int Count { get; init; }
}
