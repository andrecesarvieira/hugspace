namespace SynQcore.Domain.Entities.Relationships;

public class ReportingRelationship : BaseEntity
{
    // Relacionamentos
    public Guid ManagerId { get; set; }
    public Employee Manager { get; set; } = null!;
    public Guid SubordinateId { get; set; }
    public Employee Subordinate { get; set; } = null!;

    // Metadados
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Tipo de relacionamento hierárquico
    public ReportingType Type { get; set; } = ReportingType.Direct;

    // Contexto organizacional
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }

    // Propriedades calculadas
    public bool IsCurrentRelationship => IsActive && EndDate == null;
    public TimeSpan? RelationshipDuration => EndDate?.Subtract(StartDate)
        ?? DateTime.UtcNow.Subtract(StartDate);
}

public enum ReportingType
{
    Direct = 0,        // Subordinado direto
    Indirect = 1,      // Subordinado indireto (através de outros)
    Matrix = 2,        // Relacionamento matricial
    Functional = 3,    // Relacionamento funcional temporário
    ProjectBased = 4   // Baseado em projeto específico
}