using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities.Organization;

public class Employee : BaseEntity
{
    // Identificação
    public string EmployeeId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Dados Pessoais
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }

    // Dados Profissionais
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Propriedades calculadas
    public string FullName =>
        $"{FirstName}{LastName}".Trim();
    public string DisplayName =>
        string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)
        ? Email
        : FullName;
    public int YearsOfService =>
        DateTime.UtcNow.Year - HireDate.Year;

    // Propriedades de Navegação
    //public ICollection<EmployeeDepartment> Departments { get; set; } = [];
    //public ICollection<Post> Posts { get; set; } = [];
}