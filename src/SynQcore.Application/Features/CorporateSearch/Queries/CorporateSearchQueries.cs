using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;

namespace SynQcore.Application.Features.CorporateSearch.Queries;

/// <summary>
/// Query para busca corporativa básica
/// </summary>
public class CorporateSearchQuery : IRequest<PagedResult<SearchResultDto>>
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }

    public CorporateSearchQuery(string query, int page = 1, int pageSize = 20)
    {
        Query = query;
        Page = page;
        PageSize = pageSize;
    }
}

/// <summary>
/// Query para busca avançada
/// </summary>
public class AdvancedSearchQuery : IRequest<PagedResult<SearchResultDto>>
{
    public string? Query { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public bool ExactPhrase { get; set; }
    public bool AllWords { get; set; } = true;
    public bool AnyWords { get; set; }
    public List<string>? ExcludeWords { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
}

/// <summary>
/// Query para sugestões de busca
/// </summary>
public class GetSearchSuggestionsQuery : IRequest<List<SearchSuggestionDto>>
{
    public string Partial { get; set; } = string.Empty;
    public int MaxSuggestions { get; set; } = 10;
    public List<string>? Categories { get; set; }
    public bool IncludePopular { get; set; } = true;
    public bool IncludeRecent { get; set; } = true;

    public GetSearchSuggestionsQuery(string partial)
    {
        Partial = partial;
    }
}

/// <summary>
/// Query para analytics de busca
/// </summary>
public class GetSearchAnalyticsQuery : IRequest<SearchAnalyticsDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? SearchTerms { get; set; }
    public List<string>? Categories { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TopResults { get; set; } = 50;
    public string GroupBy { get; set; } = "day";
}

/// <summary>
/// Query para trending topics
/// </summary>
public class GetTrendingTopicsQuery : IRequest<List<TrendingTopicDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Period { get; set; } = "week";
    public List<string>? Categories { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int MaxTopics { get; set; } = 20;
    public float MinTrendScore { get; set; } = 0.5f;
}

/// <summary>
/// Query para estatísticas de conteúdo
/// </summary>
public class GetContentStatsQuery : IRequest<ContentStatsDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public bool IncludeInactive { get; set; }
    public bool IncludeDeleted { get; set; }
}

/// <summary>
/// Query para buscar conteúdo similar
/// </summary>
public class GetSimilarContentQuery : IRequest<List<SearchResultDto>>
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 10;
    public float MinSimilarityScore { get; set; } = 0.3f;
    public bool IncludeSameAuthor { get; set; }

    public GetSimilarContentQuery(Guid contentId, string contentType)
    {
        ContentId = contentId;
        ContentType = contentType;
    }
}

/// <summary>
/// Query para buscar por categoria
/// </summary>
public class SearchByCategoryQuery : IRequest<PagedResult<SearchResultDto>>
{
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }

    public SearchByCategoryQuery(string category)
    {
        Category = category;
    }
}

/// <summary>
/// Query para buscar por autor
/// </summary>
public class SearchByAuthorQuery : IRequest<PagedResult<SearchResultDto>>
{
    public Guid AuthorId { get; set; }
    public string? ContentType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }

    public SearchByAuthorQuery(Guid authorId)
    {
        AuthorId = authorId;
    }
}

/// <summary>
/// Query para buscar por departamento
/// </summary>
public class SearchByDepartmentQuery : IRequest<PagedResult<SearchResultDto>>
{
    public Guid DepartmentId { get; set; }
    public string? ContentType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }

    public SearchByDepartmentQuery(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}

/// <summary>
/// Query para buscar por tags
/// </summary>
public class SearchByTagsQuery : IRequest<PagedResult<SearchResultDto>>
{
    public List<string> Tags { get; set; } = new();
    public bool AllTags { get; set; } // true = AND, false = OR
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }

    public SearchByTagsQuery(List<string> tags)
    {
        Tags = tags;
    }
}

/// <summary>
/// Query para busca de conteúdo recente
/// </summary>
public class GetRecentContentQuery : IRequest<PagedResult<SearchResultDto>>
{
    public int Hours { get; set; } = 24;
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchConfigDto? Config { get; set; }
}

/// <summary>
/// Query para busca de conteúdo popular
/// </summary>
public class GetPopularContentQuery : IRequest<PagedResult<SearchResultDto>>
{
    public string Period { get; set; } = "week"; // day, week, month
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchConfigDto? Config { get; set; }
    public string MetricType { get; set; } = "engagement"; // views, likes, comments, engagement
}

/// <summary>
/// Query para exportação de dados de busca
/// </summary>
public class ExportSearchDataQuery : IRequest<SearchExportDto>
{
    public string Query { get; set; } = string.Empty;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
    public string Format { get; set; } = "json";
    public bool IncludeAnalytics { get; set; } = true;
    public bool IncludeMetadata { get; set; }
    public int MaxResults { get; set; } = 1000;

    public ExportSearchDataQuery(string query, string format)
    {
        Query = query;
        Format = format;
    }
}

/// <summary>
/// Query para obter configuração de busca
/// </summary>
public class GetSearchConfigQuery : IRequest<SearchConfigDto>
{
    // Query simples sem parâmetros - retorna configuração atual
}
