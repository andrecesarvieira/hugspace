namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa a associação entre um post e uma tag.
/// Controla metadados de quando e por quem a tag foi adicionada.
/// </summary>
public class PostTag : BaseEntity
{
    /// <summary>
    /// ID do post que possui a tag.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Post que possui a tag.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    /// ID da tag associada ao post.
    /// </summary>
    public Guid TagId { get; set; }

    /// <summary>
    /// Tag associada ao post.
    /// </summary>
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// ID do funcionário que adicionou a tag ao post.
    /// </summary>
    public Guid AddedById { get; set; }

    /// <summary>
    /// Funcionário que adicionou a tag ao post.
    /// </summary>
    public Employee AddedBy { get; set; } = null!;

    /// <summary>
    /// Data e hora quando a tag foi adicionada ao post.
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
