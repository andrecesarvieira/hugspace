using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;

namespace SynQcore.Application.Features.CorporateSearch.Queries;

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

public class GetContentStatsQuery : IRequest<ContentStatsDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public bool IncludeInactive { get; set; }
    public bool IncludeDeleted { get; set; }
}

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

public class GetRecentContentQuery : IRequest<PagedResult<SearchResultDto>>
{
    public int Hours { get; set; } = 24;
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchConfigDto? Config { get; set; }
}

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

public class GetSearchConfigQuery : IRequest<SearchConfigDto>
{
    // Query simples sem parâmetros - retorna configuração atual
}
