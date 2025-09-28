namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma curtida ou reação de um funcionário a um post.
/// Suporta diferentes tipos de reações corporativas.
/// </summary>
public class PostLike : BaseEntity
{
    /// <summary>
    /// ID do post que foi curtido.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Post que foi curtido.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    /// ID do funcionário que curtiu o post.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário que curtiu o post.
    /// </summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// Tipo de reação corporativa ao post.
    /// </summary>
    public ReactionType ReactionType { get; set; } = ReactionType.Like;

    /// <summary>
    /// Data e hora quando a curtida foi registrada.
    /// </summary>
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Tipos de reações disponíveis para posts corporativos.
/// </summary>
public enum ReactionType
{
    /// <summary>
    /// Curtida padrão (👍).
    /// </summary>
    Like = 0,

    /// <summary>
    /// Marca como útil ou valioso (🔥).
    /// </summary>
    Helpful = 1,

    /// <summary>
    /// Marca como perspicaz ou inteligente (💡).
    /// </summary>
    Insightful = 2,

    /// <summary>
    /// Celebra uma conquista ou marcos (🎉).
    /// </summary>
    Celebrate = 3
}
