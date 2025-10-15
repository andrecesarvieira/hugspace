using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;

namespace SynQcore.Application.Features.Feed.Handlers;

public partial class RegenerateFeedHandler : IRequestHandler<RegenerateFeedCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<RegenerateFeedHandler> _logger;

    public RegenerateFeedHandler(
        ISynQcoreDbContext context,
        ILogger<RegenerateFeedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(RegenerateFeedCommand request, CancellationToken cancellationToken)
    {
        LogRegeneratingFeed(_logger, request.UserId);

        // Remove entradas antigas se não preservar bookmarks
        var entriesToRemove = _context.FeedEntries.Where(fe => fe.UserId == request.UserId);

        if (request.PreserveBookmarks)
        {
            entriesToRemove = entriesToRemove.Where(fe => !fe.IsBookmarked);
        }

        _context.FeedEntries.RemoveRange(entriesToRemove);
        await _context.SaveChangesAsync(cancellationToken);

        LogFeedRegenerated(_logger, request.UserId);
    }

    [LoggerMessage(EventId = 3419, Level = LogLevel.Information,
        Message = "Regenerating feed for user {UserId}")]
    private static partial void LogRegeneratingFeed(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 3420, Level = LogLevel.Information,
        Message = "Feed regenerated for user {UserId}")]
    private static partial void LogFeedRegenerated(ILogger logger, Guid userId);
}

public partial class UpdateUserInterestsHandler : IRequestHandler<UpdateUserInterestsCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateUserInterestsHandler> _logger;

    public UpdateUserInterestsHandler(
        ISynQcoreDbContext context,
        ILogger<UpdateUserInterestsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UpdateUserInterestsCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingInterests(_logger, request.UserId, request.InteractionType);

        // Implementação básica - pode ser expandida futuramente
        await _context.SaveChangesAsync(cancellationToken);

        LogInterestsUpdated(_logger, request.UserId);
    }

    [LoggerMessage(EventId = 3421, Level = LogLevel.Information,
        Message = "Updating interests for user {UserId} based on {InteractionType}")]
    private static partial void LogUpdatingInterests(ILogger logger, Guid userId, string interactionType);

    [LoggerMessage(EventId = 3422, Level = LogLevel.Information,
        Message = "User interests updated for {UserId}")]
    private static partial void LogInterestsUpdated(ILogger logger, Guid userId);
}

public partial class ProcessBulkFeedUpdateHandler : IRequestHandler<ProcessBulkFeedUpdateCommand>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<ProcessBulkFeedUpdateHandler> _logger;

    public ProcessBulkFeedUpdateHandler(
        ISynQcoreDbContext context,
        ILogger<ProcessBulkFeedUpdateHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ProcessBulkFeedUpdateCommand request, CancellationToken cancellationToken)
    {
        LogProcessingBulkUpdate(_logger, request.PostIds.Count);

        // Implementação básica - pode ser expandida futuramente
        await _context.SaveChangesAsync(cancellationToken);

        LogBulkUpdateCompleted(_logger, request.PostIds.Count);
    }

    [LoggerMessage(EventId = 3423, Level = LogLevel.Information,
        Message = "Processing bulk feed update for {PostCount} posts")]
    private static partial void LogProcessingBulkUpdate(ILogger logger, int postCount);

    [LoggerMessage(EventId = 3424, Level = LogLevel.Information,
        Message = "Bulk feed update completed for {PostCount} posts")]
    private static partial void LogBulkUpdateCompleted(ILogger logger, int postCount);
}
