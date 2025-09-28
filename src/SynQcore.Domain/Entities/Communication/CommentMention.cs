namespace SynQcore.Domain.Entities.Communication;

public class CommentMention : BaseEntity
{
    public Guid CommentId { get; set; }

    public Comment Comment { get; set; } = null!;

    public Guid MentionedEmployeeId { get; set; }

    public Employee MentionedEmployee { get; set; } = null!;

    public Guid MentionedById { get; set; }

    public Employee MentionedBy { get; set; } = null!;

    public string MentionText { get; set; } = string.Empty;

    public int StartPosition { get; set; }

    public int Length { get; set; }

    public bool HasBeenNotified { get; set; }

    public DateTime? NotifiedAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public MentionContext Context { get; set; } = MentionContext.General;

    public MentionUrgency Urgency { get; set; } = MentionUrgency.Normal;
}

public enum MentionContext
{
    General = 0,

    Question = 1,

    Action = 2,

    FYI = 3,

    Decision = 4,

    Approval = 5,

    Review = 6,

    Escalation = 7
}

public enum MentionUrgency
{
    Low = 0,

    Normal = 1,

    High = 2,

    Urgent = 3
}
