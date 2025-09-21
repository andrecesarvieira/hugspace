using HugSpace.Domain.Common;

namespace HugSpace.Domain.Entities;

public class Post : BaseEntity
{
    public string? Content { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsPublic { get; private set; }
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }
    public int ViewCount { get; private set; }

    // Propriedades para navegação EF Core
    public User? User { get; private set; }
    public ICollection<PostLike> Likes { get; private set; } = [];
    public ICollection<Comment> Comments { get; private set; } = [];

    public Post() { } // EF Core

    public Post(string content, Guid userId, bool isPublic = true)
    {
        Content = content;
        UserId = userId;
        IsPublic = isPublic;
        LikeCount = 0;
        CommentCount = 0;
    }

    public void UpdateContent(string content)
    {
        Content = content;
        UpdateTimestamp();
    }

    public void SetVisibility(bool isPublic)
    {
        IsPublic = isPublic;
        UpdateTimestamp();
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementLikeCount()
    {
        LikeCount++;
    }

    public void DecrementLikeCount()
    {
        if (LikeCount > 0)
            LikeCount--;
    }

    public void IncrementCommentCount()
    {
        CommentCount++;
    }

    public void DecrementCommentCount()
    {
        if (CommentCount > 0)
            CommentCount--;
    }
}