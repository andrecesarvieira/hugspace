namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma curtida em comentário de discussão corporativa.
/// </summary>
public class CommentLike : BaseEntity
{
    /// <summary>
    /// ID do comentário curtido.
    /// </summary>
    public Guid CommentId { get; set; }

    /// <summary>
    /// Comentário que foi curtido.
    /// </summary>
    public Comment Comment { get; set; } = null!;

    /// <summary>
    /// ID do funcionário que curtiu o comentário.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário que curtiu o comentário.
    /// </summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// Data e hora quando a curtida foi registrada.
    /// </summary>
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
