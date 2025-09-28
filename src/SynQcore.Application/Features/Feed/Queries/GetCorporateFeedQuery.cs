using MediatR;
using SynQcore.Application.DTOs;
using SynQcore.Application.Common.DTOs;

namespace SynQcore.Application.Features.Feed.Queries;

public record GetCorporateFeedQuery : IRequest<PagedResult<FeedItemDto>>
{
    public Guid UserId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? FeedType { get; init; } = "all";

    public FeedFiltersDto? Filters { get; init; }

    public string? SortBy { get; init; } = "created_date";

    public bool RefreshFeed { get; init; }
}

public record GetFeedStatsQuery : IRequest<FeedStatsDto>
{
    public Guid UserId { get; init; }
}

public record GetDepartmentFeedQuery : IRequest<PagedResult<FeedItemDto>>
{
    public Guid UserId { get; init; }

    public Guid DepartmentId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public FeedFiltersDto? Filters { get; init; }
}

public record GetTrendingContentQuery : IRequest<PagedResult<FeedItemDto>>
{
    public Guid UserId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? TimeWindow { get; init; } = "7d";

    public string? Department { get; init; }
}

public record GetRecommendedContentQuery : IRequest<PagedResult<FeedItemDto>>
{
    public Guid UserId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public double? MinRelevanceScore { get; init; } = 0.6;
}
