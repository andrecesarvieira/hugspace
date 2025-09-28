namespace SynQcore.Domain.Entities.Communication;

public class PostLike : BaseEntity
{
    public Guid PostId { get; set; }

    public Post Post { get; set; } = null!;

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public ReactionType ReactionType { get; set; } = ReactionType.Like;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}

public enum ReactionType
{
    Like = 0,

    Helpful = 1,

    Insightful = 2,

    Celebrate = 3
}
