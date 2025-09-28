namespace SynQcore.Domain.Entities.Communication;

public class FeedEntry : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public Guid AuthorId { get; set; }

    public FeedPriority Priority { get; set; } = FeedPriority.Normal;

    public double RelevanceScore { get; set; }

    public FeedReason Reason { get; set; }

    public DateTime? ViewedAt { get; set; }

    public bool IsRead { get; set; }

    public bool IsBookmarked { get; set; }

    public bool IsHidden { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? TeamId { get; set; }

    public Employee User { get; set; } = null!;

    public Post Post { get; set; } = null!;

    public Employee Author { get; set; } = null!;

    public Department? Department { get; set; }

    public Team? Team { get; set; }
}

public enum FeedPriority
{
    Low = 0,

    Normal = 1,

    High = 2,

    Urgent = 3,

    Executive = 4
}

public enum FeedReason
{
    Following = 0,

    SameDepartment = 1,

    SameTeam = 2,

    SimilarSkills = 3,

    TagInterest = 4,

    Trending = 5,

    Recommended = 6,

    Official = 7,

    Mentioned = 8,

    CategoryInterest = 9
}
