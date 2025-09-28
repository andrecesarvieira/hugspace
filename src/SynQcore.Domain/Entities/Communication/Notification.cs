namespace SynQcore.Domain.Entities.Communication;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? ActionUrl { get; set; }

    public NotificationType Type { get; set; }

    public NotificationPriority Priority { get; set; } =
        NotificationPriority.Normal;

    public Guid RecipientId { get; set; }

    public Employee Recipient { get; set; } = null!;

    public Guid? SenderId { get; set; }

    public Employee? Sender { get; set; }

    public Guid? PostId { get; set; }

    public Post? Post { get; set; }

    public Guid? CommentId { get; set; }

    public Comment? Comment { get; set; }

    public Guid? RelatedEntityId { get; set; }

    public string? RelatedEntityType { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool IsEmailSent { get; set; }
}

public enum NotificationType
{
    PostLike = 0,

    PostComment = 1,

    CommentReply = 2,

    CommentLike = 3,

    Mention = 4,

    Follow = 5,

    CompanyNews = 6,

    SystemAlert = 7,

    Birthday = 8,

    WorkAnniversary = 9,

    NewEmployee = 10,

    ModerationAction = 11
}

public enum NotificationPriority
{
    Low = 0,

    Normal = 1,

    High = 2,

    Critical = 3
}
