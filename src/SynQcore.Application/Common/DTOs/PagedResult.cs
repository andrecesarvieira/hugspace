namespace SynQcore.Application.Common.DTOs;

public record PagedResult<T>
{
    public List<T> Items { get; init; } = new();

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalPages { get; init; }

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
