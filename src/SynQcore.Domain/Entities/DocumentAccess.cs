using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

public class DocumentAccess : BaseEntity
{
    public Guid DocumentId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? Role { get; set; }

    public AccessType AccessType { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid GrantedByEmployeeId { get; set; }

    public DateTimeOffset GrantedAt { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; }

    // Relacionamentos
    public CorporateDocument Document { get; set; } = null!;

    public Employee? Employee { get; set; }

    public Department? Department { get; set; }

    public Employee GrantedByEmployee { get; set; } = null!;
}

public class DocumentAccessLog : BaseEntity
{
    public Guid DocumentId { get; set; }

    public Guid EmployeeId { get; set; }

    public DocumentAction Action { get; set; }

    public DateTimeOffset AccessedAt { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Details { get; set; }

    public int? SessionDurationSeconds { get; set; }

    // Relacionamentos
    public CorporateDocument Document { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
}