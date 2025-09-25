using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

public record EmployeeHierarchyDto
{
    public EmployeeDto Employee { get; init; } = null!;
    public EmployeeDto? Manager { get; init; }
    public List<EmployeeDto> Subordinates { get; init; } = new();
    public List<EmployeeDto> Peers { get; init; } = new();
}

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