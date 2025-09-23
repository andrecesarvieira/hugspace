using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public class Comment : BaseEntity
{
    public string? Content { get; private set; }
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }

    // Propriedade de Navegação
    public User? User { get; private set; }
    public Post? Post { get; private set; }

    public Comment() { }

    public Comment(string content, Guid userId, Guid postId)
    {
        Content = content;
        UserId = userId;
        PostId = postId;
    }

    public void UpdateContent(string content)
    {
        Content = content;
        UpdateTimestamp();
    }
}