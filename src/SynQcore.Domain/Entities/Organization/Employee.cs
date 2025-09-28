namespace SynQcore.Domain.Entities.Organization;

public class Employee : BaseEntity
{
    public string EmployeeId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public string? Bio { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? ManagerId { get; set; }

    public Employee? Manager { get; set; }

    public string FullName =>
        $"{FirstName}{LastName}".Trim();

    public string DisplayName =>
        string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)
        ? Email
        : FullName;

    public int YearsOfService =>
        DateTime.UtcNow.Year - HireDate.Year;

    public ICollection<Employee> Subordinates { get; set; } = [];

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = [];

    public ICollection<TeamMembership> TeamMemberships { get; set; } = [];

    public ICollection<ReportingRelationship> DirectReports { get; set; } = [];

    public ICollection<ReportingRelationship> ManagerRelationships { get; set; } = [];

    public ICollection<Post> Posts { get; set; } = [];

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<PostLike> PostLikes { get; set; } = [];

    public ICollection<CommentLike> CommentLikes { get; set; } = [];

    public ICollection<CommentMention> MentionsMade { get; set; } = [];

    public ICollection<Notification> ReceivedNotifications { get; set; } = [];

    public ICollection<Notification> SentNotifications { get; set; } = [];
}
