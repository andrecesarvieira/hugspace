namespace SynQcore.Domain.Entities.Relationships;

public class TeamMembership : BaseEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid TeamId { get; set; }

    public Team Team { get; set; } = null!;

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LeftDate { get; set; }

    public bool IsActive { get; set; } = true;

    public TeamRole Role { get; set; } = TeamRole.Member;

    public string? SpecificRole { get; set; }

    public bool IsCurrentMember => IsActive && LeftDate == null;

    public TimeSpan? MembershipDuration => LeftDate?.Subtract(JoinedDate)
        ?? DateTime.UtcNow.Subtract(JoinedDate);
}

public enum TeamRole
{
    Member = 0,

    Leader = 1,

    CoLeader = 2,

    Coordinator = 3
}
