using MediatR;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Application.Commands.Communication.DiscussionThreads;

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

public record UpdateDiscussionCommentCommand(
    Guid CommentId,
    string Content,
    string Type = "Regular",
    string Visibility = "Public",
    bool IsConfidential = false,
    string Priority = "Normal"
) : IRequest<CommentOperationResponse>;

public record DeleteDiscussionCommentCommand(
    Guid CommentId
) : IRequest<CommentOperationResponse>;

public record ResolveDiscussionCommentCommand(
    Guid CommentId,
    string? ResolutionNote = null
) : IRequest<CommentOperationResponse>;

public record ModerateDiscussionCommentCommand(
    Guid CommentId,
    string ModerationStatus,
    string? ModerationReason = null
) : IRequest<CommentOperationResponse>;

public record HighlightDiscussionCommentCommand(
    Guid CommentId,
    bool IsHighlighted
) : IRequest<CommentOperationResponse>;

public record MarkMentionAsReadCommand(
    Guid MentionId
) : IRequest<bool>;
