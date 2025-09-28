namespace SynQcore.Domain.Entities.Communication;

public class PostTag : BaseEntity
{
    public Guid PostId { get; set; }

    public Post Post { get; set; } = null!;

    public Guid TagId { get; set; }

    public Tag Tag { get; set; } = null!;

    public Guid AddedById { get; set; }

    public Employee AddedBy { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
