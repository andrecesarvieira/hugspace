namespace SynQcore.Domain.Entities.Communication;

public class CommentLike : BaseEntity
{
    public Guid CommentId { get; set; }

    public Comment Comment { get; set; } = null!;

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
