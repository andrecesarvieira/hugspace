using System;
using System.Collections.Generic;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Departments.DTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
    public int ChildrenCount { get; set; }
    public int EmployeesCount { get; set; }
    public List<EmployeeSummaryDto> Employees { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}