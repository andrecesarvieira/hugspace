namespace SynQcore.Domain.Entities.Communication;

public class PostLike : BaseEntity
{
    // Relacionamentos
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    // Tipo de reação (corporativa)
    public ReactionType ReactionType { get; set; } = ReactionType.Like;

    // Metadata
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}

public enum ReactionType
{
    Like = 0,        // 👍 Curtir
    Helpful = 1,     // 🔥 Útil
    Insightful = 2,  // 💡 Perspicaz  
    Celebrate = 3    // 🎉 Celebrar
}