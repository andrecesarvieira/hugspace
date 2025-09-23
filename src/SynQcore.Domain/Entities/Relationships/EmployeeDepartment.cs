namespace SynQcore.Domain.Entities.Relationships;

public class EmployeeDepartment : BaseEntity
{
    // Relacionamentos
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Metadados
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPrimary { get; set; }

    // Cargo específico no departamento
    public string? RoleInDepartment { get; set; }

    // Propriedades calculadas
    public bool IsCurrentAssignment => IsActive && EndDate == null;
}