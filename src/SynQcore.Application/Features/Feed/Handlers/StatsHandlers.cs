using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs;
using SynQcore.Application.Features.Feed.Queries;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para obter estatísticas do feed
/// </summary>
public partial class GetFeedStatsHandler : IRequestHandler<GetFeedStatsQuery, FeedStatsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetFeedStatsHandler> _logger;

    public GetFeedStatsHandler(
        ISynQcoreDbContext context,
        ILogger<GetFeedStatsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FeedStatsDto> Handle(GetFeedStatsQuery request, CancellationToken cancellationToken)
    {
        LogGettingFeedStats(_logger, request.UserId);

        var feedEntries = _context.FeedEntries.Where(fe => fe.UserId == request.UserId);

        var totalItems = await feedEntries.CountAsync(cancellationToken);
        var unreadCount = await feedEntries.Where(fe => !fe.IsRead).CountAsync(cancellationToken);
        var bookmarkedCount = await feedEntries.Where(fe => fe.IsBookmarked).CountAsync(cancellationToken);
        var hiddenCount = await feedEntries.Where(fe => fe.IsHidden).CountAsync(cancellationToken);

        return new FeedStatsDto
        {
            TotalItems = totalItems,
            UnreadCount = unreadCount,
            BookmarkedCount = bookmarkedCount,
            HiddenCount = hiddenCount,
            PriorityBreakdown = new Dictionary<string, int>(),
            TopEngagedPosts = new List<TopEngagedPostDto>(),
            RecentInteractions = new List<RecentInteractionDto>(),
            LastUpdated = DateTime.UtcNow
        };
    }

    [LoggerMessage(EventId = 3425, Level = LogLevel.Information,
        Message = "Getting feed stats for user {UserId}")]
    private static partial void LogGettingFeedStats(ILogger logger, Guid userId);
}

/// <summary>
/// Handler para obter interesses do usuário
/// </summary>
public partial class GetUserInterestsHandler : IRequestHandler<GetUserInterestsQuery, UserInterestsResponseDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetUserInterestsHandler> _logger;

    public GetUserInterestsHandler(
        ISynQcoreDbContext context,
        ILogger<GetUserInterestsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserInterestsResponseDto> Handle(GetUserInterestsQuery request, CancellationToken cancellationToken)
    {
        LogGettingUserInterests(_logger, request.UserId);

        var userInterests = await _context.UserInterests
            .Where(ui => ui.UserId == request.UserId)
            .OrderByDescending(ui => ui.Score)
            .Take(10)
            .ToListAsync(cancellationToken);

        var topInterests = userInterests.ToUserInterestDtos();

        return new UserInterestsResponseDto
        {
            TopInterests = topInterests,
            InterestsByType = new Dictionary<string, List<UserInterestDto>>(),
            TotalInterests = userInterests.Count,
            AverageScore = userInterests.Count > 0 ? userInterests.Average(ui => ui.Score) : 0,
            TotalInteractions = userInterests.Sum(ui => ui.InteractionCount),
            LastUpdated = DateTime.UtcNow
        };
    }

    [LoggerMessage(EventId = 3426, Level = LogLevel.Information,
        Message = "Getting user interests for {UserId}")]
    private static partial void LogGettingUserInterests(ILogger logger, Guid userId);
}