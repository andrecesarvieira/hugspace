namespace SynQcore.Domain.Entities.Communication;

public class Comment : BaseEntity
{
    // Conteúdo
    public string Content { get; set; } = string.Empty;

    // Hierarquia de comentários (replies)
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];

    // Relacionamentos
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public Employee Author { get; set; } = null!;

    // Moderação
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsFlagged { get; set; }

    // Propriedades de Navegação
    public ICollection<CommentLike> Likes { get; set; } = [];

}