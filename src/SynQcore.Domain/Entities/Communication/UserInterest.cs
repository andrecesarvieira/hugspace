namespace SynQcore.Domain.Entities.Communication;

public class UserInterest : BaseEntity
{
    public Guid UserId { get; set; }

    public InterestType Type { get; set; }

    public string InterestValue { get; set; } = string.Empty;

    public double Score { get; set; } = 1.0;

    public int InteractionCount { get; set; }

    public DateTime LastInteractionAt { get; set; } = DateTime.UtcNow;

    public InterestSource Source { get; set; }

    public bool IsExplicit { get; set; }

    public Employee User { get; set; } = null!;
}

public enum InterestType
{
    Tag = 0,

    Category = 1,

    Department = 2,

    Author = 3,

    PostType = 4,

    Skill = 5
}

public enum InterestSource
{
    UserDefined = 0,

    ViewHistory = 1,

    LikeHistory = 2,

    CommentHistory = 3,

    SearchHistory = 4,

    ProfileSkills = 5
}
