using MediatR;
using SynQcore.Application.DTOs;
using SynQcore.Application.Common.DTOs;

namespace SynQcore.Application.Features.Feed.Queries;

/// <summary>
/// Query para obter feed corporativo personalizado
/// </summary>
public record GetCorporateFeedQuery : IRequest<PagedResult<FeedItemDto>>
{
    /// <summary>
    /// ID do usuário solicitante
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Número da página para paginação
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Tipo de feed (all, department, following, etc.)
    /// </summary>
    public string? FeedType { get; init; } = "all";

    /// <summary>
    /// Filtros adicionais para o feed
    /// </summary>
    public FeedFiltersDto? Filters { get; init; }

    /// <summary>
    /// Campo para ordenação dos resultados
    /// </summary>
    public string? SortBy { get; init; } = "created_date";

    /// <summary>
    /// Indica se deve atualizar o cache do feed
    /// </summary>
    public bool RefreshFeed { get; init; }
}

/// <summary>
/// Query para obter estatísticas do feed do usuário
/// </summary>
public record GetFeedStatsQuery : IRequest<FeedStatsDto>
{
    /// <summary>
    /// ID do usuário para obter estatísticas
    /// </summary>
    public Guid UserId { get; init; }
}

/// <summary>
/// Query para obter feed específico de um departamento
/// </summary>
public record GetDepartmentFeedQuery : IRequest<PagedResult<FeedItemDto>>
{
    /// <summary>
    /// ID do usuário solicitante
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// ID do departamento para filtrar o feed
    /// </summary>
    public Guid DepartmentId { get; init; }

    /// <summary>
    /// Número da página para paginação
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Filtros adicionais para o feed
    /// </summary>
    public FeedFiltersDto? Filters { get; init; }
}

/// <summary>
/// Query para obter conteúdo em alta/tendência
/// </summary>
public record GetTrendingContentQuery : IRequest<PagedResult<FeedItemDto>>
{
    /// <summary>
    /// ID do usuário solicitante
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Número da página para paginação
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Janela de tempo para análise de tendências (24h, 7d, 30d)
    /// </summary>
    public string? TimeWindow { get; init; } = "7d";

    /// <summary>
    /// Filtro por departamento específico
    /// </summary>
    public string? Department { get; init; }
}

/// <summary>
/// Query para obter conteúdo recomendado baseado no perfil do usuário
/// </summary>
public record GetRecommendedContentQuery : IRequest<PagedResult<FeedItemDto>>
{
    /// <summary>
    /// ID do usuário para gerar recomendações
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Número da página para paginação
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamanho da página para paginação
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Score mínimo de relevância para incluir na recomendação
    /// </summary>
    public double? MinRelevanceScore { get; init; } = 0.6;
}
