namespace SynQcore.Application.DTOs.Communication;

/// <summary>
/// DTO detalhado para comentários em discussion threads
/// </summary>
public class DiscussionCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorJobTitle { get; set; } = string.Empty;
    public string? AuthorProfilePhotoUrl { get; set; }
    
    // Thread hierarchy
    public Guid? ParentCommentId { get; set; }
    public int ThreadLevel { get; set; }
    public string ThreadPath { get; set; } = string.Empty;
    public int ReplyCount { get; set; }
    public List<DiscussionCommentDto> Replies { get; set; } = [];
    
    // Discussion features
    public string Type { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? ResolvedByName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }
    
    // Moderation
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsFlagged { get; set; }
    public string ModerationStatus { get; set; } = string.Empty;
    public string? ModerationReason { get; set; }
    public DateTime? ModeratedAt { get; set; }
    
    // Visibility and priority
    public string Visibility { get; set; } = string.Empty;
    public bool IsConfidential { get; set; }
    public bool IsHighlighted { get; set; }
    public string Priority { get; set; } = string.Empty;
    
    // Engagement metrics
    public int LikeCount { get; set; }
    public int EndorsementCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public DateTime LastActivityAt { get; set; }
    
    // Mentions
    public List<CommentMentionDto> Mentions { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO para criação de comentário em discussion thread
/// </summary>
public class CreateDiscussionCommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Type { get; set; } = "Regular";
    public string Visibility { get; set; } = "Public";
    public bool IsConfidential { get; set; }
    public string Priority { get; set; } = "Normal";
    public List<CreateCommentMentionDto> Mentions { get; set; } = [];
}

/// <summary>
/// DTO para atualização de comentário
/// </summary>
public class UpdateDiscussionCommentDto
{
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "Regular";
    public string Visibility { get; set; } = "Public";
    public bool IsConfidential { get; set; }
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// DTO para resolução de comentário do tipo Question ou Concern
/// </summary>
public class ResolveCommentDto
{
    public string? ResolutionNote { get; set; }
}

/// <summary>
/// DTO para moderação de comentário
/// </summary>
public class ModerateCommentDto
{
    public string ModerationStatus { get; set; } = string.Empty;
    public string? ModerationReason { get; set; }
}

/// <summary>
/// DTO para thread de discussão completa
/// </summary>
public class DiscussionThreadDto
{
    public Guid PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public List<DiscussionCommentDto> Comments { get; set; } = [];
    public int TotalComments { get; set; }
    public int UnresolvedQuestions { get; set; }
    public int FlaggedComments { get; set; }
    public DateTime LastActivityAt { get; set; }
}

/// <summary>
/// Resposta para operações de comentário
/// </summary>
public class CommentOperationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DiscussionCommentDto? Comment { get; set; }
    public List<string> ValidationErrors { get; set; } = [];
}