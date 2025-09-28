using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

public record EmployeeSearchRequest
{
    public string? SearchTerm { get; init; }

    public Guid? DepartmentId { get; init; }

    public Guid? TeamId { get; init; }

    public Guid? ManagerId { get; init; }

    public bool? IsActive { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
