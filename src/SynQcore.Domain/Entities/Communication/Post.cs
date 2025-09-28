namespace SynQcore.Domain.Entities.Communication;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string? ImageUrl { get; set; }

    public string? DocumentUrl { get; set; }

    public PostType Type { get; set; } = PostType.Post;

    public bool IsPinned { get; set; }

    public bool IsOfficial { get; set; }

    public bool RequiresApproval { get; set; }

    public PostStatus Status { get; set; } = PostStatus.Draft;

    public PostVisibility Visibility { get; set; } = PostVisibility.Public;

    public Guid? DepartmentId { get; set; }

    public Guid? TeamId { get; set; }

    public Guid? CategoryId { get; set; }

    public KnowledgeCategory? Category { get; set; }

    public int ViewCount { get; set; }

    public int LikeCount { get; set; }

    public int CommentCount { get; set; }

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public string Version { get; set; } = "1.0";

    public Guid? ParentPostId { get; set; }

    public Post? ParentPost { get; set; }

    public ICollection<Post> Versions { get; set; } = [];

    public Guid AuthorId { get; set; }

    public Employee Author { get; set; } = null!;

    public Department? Department { get; set; }

    public Team? Team { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<PostLike> Likes { get; set; } = [];

    public ICollection<PostTag> PostTags { get; set; } = [];

    public ICollection<Endorsement> Endorsements { get; set; } = [];
}

public enum PostType
{
    Post = 0,

    Article = 1,

    Announcement = 2,

    Policy = 3,

    FAQ = 4,

    HowTo = 5,

    News = 6
}

public enum PostStatus
{
    Draft = 0,

    PendingApproval = 1,

    Published = 2,

    Archived = 3,

    Rejected = 4
}

public enum PostVisibility
{
    Public = 0,

    Department = 1,

    Team = 2,

    Company = 3
}
