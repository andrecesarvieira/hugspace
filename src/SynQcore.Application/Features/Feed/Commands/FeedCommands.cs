using MediatR;
using SynQcore.Application.Features.Feed.DTOs;

namespace SynQcore.Application.Features.Feed.Commands;

public record CreateFeedPostCommand : IRequest<FeedPostDto>
{
    public Guid AuthorId { get; init; }
    public string Content { get; init; } = string.Empty;
    public string[]? Tags { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsPublic { get; init; } = true;
}

public record RegenerateFeedCommand : IRequest
{
    public Guid UserId { get; init; }
    public bool PreserveBookmarks { get; init; } = true;
    public int DaysToInclude { get; init; } = 30;
    public int? MaxItems { get; init; }
}

public record MarkFeedItemAsReadCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid FeedEntryId { get; init; }
}

public record ToggleFeedBookmarkCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid FeedEntryId { get; init; }
}

public record HideFeedItemCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid FeedEntryId { get; init; }
    public string? Reason { get; init; }
}

public record UpdateUserInterestsCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid ContentId { get; init; }
    public string InteractionType { get; init; } = string.Empty; // "view", "like", "comment", "share", "bookmark"
    public bool RecalculateScores { get; init; }
}

public record ProcessBulkFeedUpdateCommand : IRequest
{
    public List<Guid> PostIds { get; init; } = [];
    public string UpdateType { get; init; } = "new_post"; // "new_post", "post_updated", "post_deleted"
}

public record UpdateFeedPostCommand : IRequest<FeedPostDto>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
    public string? Content { get; init; }
    public string[]? Tags { get; init; }
    public string? ImageUrl { get; init; }
    public bool? IsPublic { get; init; }
}

public record DeleteFeedPostCommand : IRequest
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
}

public record GetFeedPostCommand : IRequest<FeedPostDto?>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
}
