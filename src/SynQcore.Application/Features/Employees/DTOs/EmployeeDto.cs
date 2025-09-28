using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

public record EmployeeDto
{
    public Guid Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? Phone { get; init; }

    public string? Avatar { get; init; }

    public DateTime HireDate { get; init; }

    public bool IsActive { get; init; }

    public Guid? ManagerId { get; init; }

    public string? ManagerName { get; init; }

    public List<EmployeeDepartmentDto> Departments { get; init; } = new();

    public List<TeamDto> Teams { get; init; } = new();

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}

public record EmployeeDepartmentDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}

public record TeamDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string Role { get; init; } = "Member";
}
