using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;

namespace SynQcore.Application.Features.Feed.Handlers;

public partial class MarkFeedItemAsReadHandler : IRequestHandler<MarkFeedItemAsReadCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<MarkFeedItemAsReadHandler> _logger;

    public MarkFeedItemAsReadHandler(
        ISynQcoreDbContext context,
        ILogger<MarkFeedItemAsReadHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(MarkFeedItemAsReadCommand request, CancellationToken cancellationToken)
    {
        LogMarkingItemAsRead(_logger, request.FeedEntryId, request.UserId);

        var feedEntry = await _context.FeedEntries
            .FirstOrDefaultAsync(fe => fe.Id == request.FeedEntryId &&
                               fe.UserId == request.UserId, cancellationToken);

        if (feedEntry == null)
        {
            LogFeedEntryNotFound(_logger, request.FeedEntryId, request.UserId);
            return;
        }

        // Atualiza status apenas se ainda não foi marcado como lido
        if (!feedEntry.IsRead)
        {
            feedEntry.IsRead = true;
            feedEntry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            LogItemMarkedAsRead(_logger, request.FeedEntryId, request.UserId);
        }

        // Sempre atualiza timestamp de visualização
        feedEntry.ViewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    [LoggerMessage(EventId = 3410, Level = LogLevel.Information,
        Message = "Marking feed item {FeedEntryId} as read for user {UserId}")]
    private static partial void LogMarkingItemAsRead(ILogger logger, Guid feedEntryId, Guid userId);

    [LoggerMessage(EventId = 3411, Level = LogLevel.Warning,
        Message = "Feed entry {FeedEntryId} not found for user {UserId}")]
    private static partial void LogFeedEntryNotFound(ILogger logger, Guid feedEntryId, Guid userId);

    [LoggerMessage(EventId = 3412, Level = LogLevel.Information,
        Message = "Feed item {FeedEntryId} marked as read for user {UserId}")]
    private static partial void LogItemMarkedAsRead(ILogger logger, Guid feedEntryId, Guid userId);
}

public partial class ToggleFeedBookmarkHandler : IRequestHandler<ToggleFeedBookmarkCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<ToggleFeedBookmarkHandler> _logger;

    public ToggleFeedBookmarkHandler(
        ISynQcoreDbContext context,
        ILogger<ToggleFeedBookmarkHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ToggleFeedBookmarkCommand request, CancellationToken cancellationToken)
    {
        LogTogglingBookmark(_logger, request.FeedEntryId, request.UserId);

        var feedEntry = await _context.FeedEntries
            .FirstOrDefaultAsync(fe => fe.Id == request.FeedEntryId &&
                               fe.UserId == request.UserId, cancellationToken);

        if (feedEntry == null)
        {
            LogFeedEntryNotFoundForBookmark(_logger, request.FeedEntryId, request.UserId);
            return;
        }

        feedEntry.IsBookmarked = !feedEntry.IsBookmarked;
        feedEntry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        LogBookmarkToggled(_logger, request.FeedEntryId, request.UserId, feedEntry.IsBookmarked);
    }

    [LoggerMessage(EventId = 3413, Level = LogLevel.Information,
        Message = "Toggling bookmark for feed item {FeedEntryId} by user {UserId}")]
    private static partial void LogTogglingBookmark(ILogger logger, Guid feedEntryId, Guid userId);

    [LoggerMessage(EventId = 3414, Level = LogLevel.Warning,
        Message = "Feed entry {FeedEntryId} not found for user {UserId}")]
    private static partial void LogFeedEntryNotFoundForBookmark(ILogger logger, Guid feedEntryId, Guid userId);

    [LoggerMessage(EventId = 3415, Level = LogLevel.Information,
        Message = "Bookmark toggled for feed item {FeedEntryId} by user {UserId}, bookmarked: {IsBookmarked}")]
    private static partial void LogBookmarkToggled(ILogger logger, Guid feedEntryId, Guid userId, bool isBookmarked);
}

public partial class HideFeedItemHandler : IRequestHandler<HideFeedItemCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<HideFeedItemHandler> _logger;

    public HideFeedItemHandler(
        ISynQcoreDbContext context,
        ILogger<HideFeedItemHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(HideFeedItemCommand request, CancellationToken cancellationToken)
    {
        LogHidingItem(_logger, request.FeedEntryId, request.UserId, request.Reason ?? "No reason");

        var feedEntry = await _context.FeedEntries
            .FirstOrDefaultAsync(fe => fe.Id == request.FeedEntryId &&
                               fe.UserId == request.UserId, cancellationToken);

        if (feedEntry == null)
        {
            LogFeedEntryNotFoundForHide(_logger, request.FeedEntryId, request.UserId);
            return;
        }

        feedEntry.IsHidden = true;
        feedEntry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        LogItemHidden(_logger, request.FeedEntryId, request.UserId);
    }

    [LoggerMessage(EventId = 3416, Level = LogLevel.Information,
        Message = "Hiding feed item {FeedEntryId} for user {UserId}, reason: {Reason}")]
    private static partial void LogHidingItem(ILogger logger, Guid feedEntryId, Guid userId, string reason);

    [LoggerMessage(EventId = 3417, Level = LogLevel.Warning,
        Message = "Feed entry {FeedEntryId} not found for user {UserId}")]
    private static partial void LogFeedEntryNotFoundForHide(ILogger logger, Guid feedEntryId, Guid userId);

    [LoggerMessage(EventId = 3418, Level = LogLevel.Information,
        Message = "Feed item {FeedEntryId} hidden for user {UserId}")]
    private static partial void LogItemHidden(ILogger logger, Guid feedEntryId, Guid userId);
}
