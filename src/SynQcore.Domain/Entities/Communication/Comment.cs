namespace SynQcore.Domain.Entities.Communication;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public Guid? ParentCommentId { get; set; }

    public Comment? ParentComment { get; set; }

    public ICollection<Comment> Replies { get; set; } = [];

    public Guid PostId { get; set; }

    public Post Post { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public Employee Author { get; set; } = null!;

    public CommentType Type { get; set; } = CommentType.Regular;

    public bool IsResolved { get; set; }

    public Guid? ResolvedById { get; set; }

    public Employee? ResolvedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ResolutionNote { get; set; }

    public bool IsEdited { get; set; }

    public DateTime? EditedAt { get; set; }

    public bool IsFlagged { get; set; }

    public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;

    public Guid? ModeratedById { get; set; }

    public Employee? ModeratedBy { get; set; }

    public DateTime? ModeratedAt { get; set; }

    public string? ModerationReason { get; set; }

    public CommentVisibility Visibility { get; set; } = CommentVisibility.Public;

    public bool IsConfidential { get; set; }

    public int ThreadLevel { get; set; }

    public string ThreadPath { get; set; } = string.Empty;

    public bool IsHighlighted { get; set; }

    public CommentPriority Priority { get; set; } = CommentPriority.Normal;

    public int LikeCount { get; set; }

    public int ReplyCount { get; set; }

    public int EndorsementCount { get; set; }

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public ICollection<CommentLike> Likes { get; set; } = [];

    public ICollection<Endorsement> Endorsements { get; set; } = [];

    public ICollection<CommentMention> Mentions { get; set; } = [];
}

public enum CommentType
{
    Regular = 0,

    Question = 1,

    Answer = 2,

    Suggestion = 3,

    Concern = 4,

    Acknowledgment = 5,

    Decision = 6,

    Action = 7
}

public enum ModerationStatus
{
    Pending = 0,

    Approved = 1,

    Flagged = 2,

    Hidden = 3,

    Rejected = 4,

    UnderReview = 5
}

public enum CommentVisibility
{
    Public = 0,

    Internal = 1,

    Confidential = 2,

    Private = 3
}

public enum CommentPriority
{
    Low = 0,

    Normal = 1,

    High = 2,

    Urgent = 3,

    Critical = 4
}
