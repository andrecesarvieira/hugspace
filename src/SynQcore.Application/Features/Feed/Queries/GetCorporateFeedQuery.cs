using MediatR;
using SynQcore.Application.DTOs;

namespace SynQcore.Application.Features.Feed.Queries;

/// <summary>
/// Query para obter o feed corporativo personalizado do usuário
/// Implementa algoritmo de relevância e personalização
/// </summary>
public record GetCorporateFeedQuery : IRequest<CorporateFeedResponseDto>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? FeedType { get; init; } // "mixed", "department", "team", "following"
    public FeedFiltersDto? Filters { get; init; }
    public string? SortBy { get; init; } = "relevance"; // "relevance", "date", "popularity"
    public bool RefreshFeed { get; init; } // Force regenerate feed
}

/// <summary>
/// Query para obter estatísticas do feed do usuário
/// </summary>
public record GetFeedStatsQuery : IRequest<FeedStatsDto>
{
    public Guid UserId { get; init; }
}

/// <summary>
/// Query para obter feed de um departamento específico
/// </summary>
public record GetDepartmentFeedQuery : IRequest<CorporateFeedResponseDto>
{
    public Guid UserId { get; init; }
    public Guid DepartmentId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public FeedFiltersDto? Filters { get; init; }
}

/// <summary>
/// Query para obter conteúdo em alta (trending)
/// </summary>
public record GetTrendingContentQuery : IRequest<CorporateFeedResponseDto>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public TimeSpan TimeWindow { get; init; } = TimeSpan.FromDays(7); // Últimos 7 dias
    public string? Department { get; init; } // Filtrar por departamento
}

/// <summary>
/// Query para obter recomendações baseadas em interesses
/// </summary>
public record GetRecommendedContentQuery : IRequest<CorporateFeedResponseDto>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 15;
    public double MinRelevanceScore { get; init; } = 0.5;
}