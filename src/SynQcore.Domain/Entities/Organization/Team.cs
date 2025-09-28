namespace SynQcore.Domain.Entities.Organization;

public class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Guid? LeaderEmployeeId { get; set; }

    public Guid? DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;

    public int? MaxMembers { get; set; }

    public Employee? Leader { get; set; }

    public Department? Department { get; set; }

    public ICollection<TeamMembership> Members { get; set; } = [];

    public ICollection<Post> Posts { get; set; } = [];
}
