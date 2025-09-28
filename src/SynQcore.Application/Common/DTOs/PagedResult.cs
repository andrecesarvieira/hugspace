namespace SynQcore.Application.Common.DTOs;

/// <summary>
/// Resultado paginado genérico para consultas que retornam listas.
/// Fornece metadados de paginação e navegação entre páginas.
/// </summary>
/// <typeparam name="T">Tipo dos itens na lista paginada.</typeparam>
public record PagedResult<T>
{
    /// <summary>
    /// Lista de itens da página atual.
    /// </summary>
    public List<T> Items { get; init; } = new();

    /// <summary>
    /// Total de itens na consulta completa (todas as páginas).
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Número da página atual (baseado em 1).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Tamanho da página (número de itens por página).
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total de páginas disponíveis calculado.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Indica se existe página anterior à atual.
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// Indica se existe próxima página após a atual.
    /// </summary>
    public bool HasNext => Page < TotalPages;
}
