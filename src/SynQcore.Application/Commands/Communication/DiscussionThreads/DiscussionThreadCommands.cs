using MediatR;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Application.Commands.Communication.DiscussionThreads;

/// <summary>
/// Command para criar comentário em discussion thread
/// </summary>
public record CreateDiscussionCommentCommand(
    Guid PostId,
    string Content,
    Guid? ParentCommentId = null,
    string Type = "Regular",
    string Visibility = "Public",
    bool IsConfidential = false,
    string Priority = "Normal",
    List<CreateCommentMentionDto>? Mentions = null
) : IRequest<CommentOperationResponse>;

/// <summary>
/// Command para atualizar comentário
/// </summary>
public record UpdateDiscussionCommentCommand(
    Guid CommentId,
    string Content,
    string Type = "Regular",
    string Visibility = "Public",
    bool IsConfidential = false,
    string Priority = "Normal"
) : IRequest<CommentOperationResponse>;

/// <summary>
/// Command para deletar comentário
/// </summary>
public record DeleteDiscussionCommentCommand(
    Guid CommentId
) : IRequest<CommentOperationResponse>;

/// <summary>
/// Command para resolver comentário (Questions/Concerns)
/// </summary>
public record ResolveDiscussionCommentCommand(
    Guid CommentId,
    string? ResolutionNote = null
) : IRequest<CommentOperationResponse>;

/// <summary>
/// Command para moderar comentário
/// </summary>
public record ModerateDiscussionCommentCommand(
    Guid CommentId,
    string ModerationStatus,
    string? ModerationReason = null
) : IRequest<CommentOperationResponse>;

/// <summary>
/// Command para destacar/deshighlightar comentário
/// </summary>
public record HighlightDiscussionCommentCommand(
    Guid CommentId,
    bool IsHighlighted
) : IRequest<CommentOperationResponse>;

/// <summary>
/// Command para marcar menção como lida
/// </summary>
public record MarkMentionAsReadCommand(
    Guid MentionId
) : IRequest<bool>;