namespace SynQcore.Domain.Entities.Relationships;

public class ReportingRelationship : BaseEntity
{
    public Guid ManagerId { get; set; }

    public Employee Manager { get; set; } = null!;

    public Guid SubordinateId { get; set; }

    public Employee Subordinate { get; set; } = null!;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public ReportingType Type { get; set; } = ReportingType.Direct;

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public Guid? TeamId { get; set; }

    public Team? Team { get; set; }

    public bool IsCurrentRelationship => IsActive && EndDate == null;

    public TimeSpan? RelationshipDuration => EndDate?.Subtract(StartDate)
        ?? DateTime.UtcNow.Subtract(StartDate);
}

public enum ReportingType
{
    Direct = 0,

    Indirect = 1,

    Matrix = 2,

    Functional = 3,

    ProjectBased = 4
}
