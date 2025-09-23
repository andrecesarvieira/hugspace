using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public class PostLike : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }

    // Propriedades de Navegação
    public User? User { get; private set; }
    public Post? Post { get; private set; }

    private PostLike() { } // EF Core

    public PostLike(Guid userId, Guid postId)
    {
        UserId = userId;
        PostId = postId;
    }
}