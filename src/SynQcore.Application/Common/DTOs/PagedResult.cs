namespace SynQcore.Application.Common.DTOs;

// Resultado paginado genérico para consultas que retornam listas
public record PagedResult<T>
{
    // Lista de itens da página atual
    public List<T> Items { get; init; } = new();
    
    // Total de itens na consulta completa
    public int TotalCount { get; init; }
    
    // Número da página atual (baseado em 1)
    public int Page { get; init; }
    
    // Tamanho da página (itens por página)
    public int PageSize { get; init; }
    
    // Total de páginas disponíveis
    public int TotalPages { get; init; }
    
    // Indica se existe página anterior
    public bool HasPrevious => Page > 1;
    
    // Indica se existe próxima página
    public bool HasNext => Page < TotalPages;
}