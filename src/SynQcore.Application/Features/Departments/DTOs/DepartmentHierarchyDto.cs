namespace SynQcore.Application.Features.Departments.DTOs;

public class DepartmentHierarchyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int Level { get; set; }

    public string HierarchyPath { get; set; } = string.Empty;

    public int DirectEmployeesCount { get; set; }

    public int TotalEmployeesInHierarchy { get; set; }

    public DepartmentHierarchyDto? Parent { get; set; }

    public List<DepartmentHierarchyDto> Children { get; set; } = new();

    public List<EmployeeSummaryDto> DirectEmployees { get; set; } = new();
}
