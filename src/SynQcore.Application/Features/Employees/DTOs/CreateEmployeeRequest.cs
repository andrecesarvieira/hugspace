using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

public record CreateEmployeeRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public DateTime HireDate { get; init; }
    public Guid? ManagerId { get; init; }
    public List<Guid> DepartmentIds { get; init; } = new();
    public List<Guid> TeamIds { get; init; } = new();
}