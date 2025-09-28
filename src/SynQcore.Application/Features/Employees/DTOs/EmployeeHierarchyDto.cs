namespace SynQcore.Application.Features.Employees.DTOs;

public record EmployeeHierarchyDto
{
    public EmployeeDto Employee { get; init; } = null!;

    public EmployeeDto? Manager { get; init; }

    public List<EmployeeDto> Subordinates { get; init; } = new();

    public List<EmployeeDto> Peers { get; init; } = new();
}
