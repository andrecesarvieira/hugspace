namespace SynQcore.Application.Features.Departments.DTOs;

public class EmployeeSummaryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime HireDate { get; set; }

    // Propriedades calculadas
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : Email;
}