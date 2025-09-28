using SynQcore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.CorporateSearch.DTOs;

/// <summary>
/// DTO para resultado de busca corporativa
/// </summary>
public class SearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Post, Document, MediaAsset, Employee, etc.
    public string Category { get; set; } = string.Empty;
    public float RelevanceScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Author information
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string? AuthorDepartment { get; set; }

    // Additional metadata
    public Dictionary<string, object> Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> HighlightedTerms { get; set; } = new();

    // Engagement metrics
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public int DownloadCount { get; set; }
}

/// <summary>
/// DTO para filtros de busca avançada
/// </summary>
public class SearchFiltersDto
{
    public List<string>? ContentTypes { get; set; }
    public List<string>? Categories { get; set; }
    public List<Guid>? AuthorIds { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
    public DocumentAccessLevel? MinAccessLevel { get; set; }
    public DocumentAccessLevel? MaxAccessLevel { get; set; }
    public int? MinEngagement { get; set; }
    public bool? IsApproved { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO para configurações de busca
/// </summary>
public class SearchConfigDto
{
    public List<string> EnabledContentTypes { get; set; } = new();
    public List<string> EnabledFilters { get; set; } = new();
    public string DefaultSortBy { get; set; } = "Relevance";
    public List<string> AvailableSortOptions { get; set; } = new();
    public int MaxResultsPerType { get; set; } = 50;
    public int SearchTimeout { get; set; } = 5000;
    public bool EnableAutoComplete { get; set; } = true;
    public bool EnableSuggestions { get; set; } = true;
    public bool EnableAnalytics { get; set; } = true;
    public int MinQueryLength { get; set; } = 2;
    public int MaxQueryLength { get; set; } = 500;
    public bool EnableFuzzySearch { get; set; } = true;
    public float FuzzySearchThreshold { get; set; } = 0.7f;
    public bool EnableHighlighting { get; set; } = true;
    public List<string> HighlightTags { get; set; } = new();
    public int CacheTimeout { get; set; } = 300; // 5 minutos
}

/// <summary>
/// DTO para sugestões de busca
/// </summary>
public class SearchSuggestionDto
{
    public string Term { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public float Score { get; set; }
    public List<string> RelatedTerms { get; set; } = new();
}

/// <summary>
/// DTO para analytics de busca
/// </summary>
public class SearchAnalyticsDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalSearches { get; set; }
    public int UniqueUsers { get; set; }
    public double AverageResponseTime { get; set; }
    public double SuccessRate { get; set; }
    public List<PopularSearchTermDto> PopularSearchTerms { get; set; } = new();
    public List<PopularResultDto> PopularResults { get; set; } = new();
    public List<PopularCategoryDto> PopularCategories { get; set; } = new();
    public Dictionary<string, int> SearchTimeDistribution { get; set; } = new();
    public List<string> ZeroResultsQueries { get; set; } = new();
    public double QueryRefinementRate { get; set; }
}

/// <summary>
/// DTO para trending topics corporativos
/// </summary>
public class TrendingTopicDto
{
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MentionCount { get; set; }
    public int SearchCount { get; set; }
    public int EngagementCount { get; set; }
    public float TrendScore { get; set; }
    public float GrowthRate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<string> RelatedTopics { get; set; } = new();
    public List<Guid> TopContributors { get; set; } = new();
}

/// <summary>
/// DTO para estatísticas de conteúdo
/// </summary>
public class ContentStatsDto
{
    public int TotalPosts { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalMediaAssets { get; set; }
    public int TotalComments { get; set; }
    public int TotalEmployees { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public Dictionary<string, int> ContentTypeDistribution { get; set; } = new();
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    public Dictionary<string, int> DepartmentActivity { get; set; } = new();
    public List<string> TopSearchTerms { get; set; } = new();
    public List<TrendingTopicDto> TrendingTopics { get; set; } = new();
}

/// <summary>
/// DTO para exportação de dados de busca
/// </summary>
public class SearchExportDto
{
    public string SearchTerm { get; set; } = string.Empty;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto Config { get; set; } = new();
    public DateTime ExportedAt { get; set; }
    public Guid ExportedBy { get; set; }
    public string ExportFormat { get; set; } = "json"; // json, csv, excel
    public List<SearchResultDto> Results { get; set; } = new();
    public SearchAnalyticsDto Analytics { get; set; } = new();
}

// Request DTOs

/// <summary>
/// Request para busca corporativa
/// </summary>
public class CorporateSearchRequest
{
    [Required]
    [StringLength(500)]
    public string Query { get; set; } = string.Empty;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
}

/// <summary>
/// Request para busca avançada
/// </summary>
public class AdvancedSearchRequest : CorporateSearchRequest
{
    [StringLength(500)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Content { get; set; }

    [StringLength(200)]
    public string? Author { get; set; }

    public bool ExactPhrase { get; set; }
    public bool AllWords { get; set; } = true;
    public bool AnyWords { get; set; }
    public List<string>? ExcludeWords { get; set; }
}

/// <summary>
/// Request para sugestões de busca
/// </summary>
public class SearchSuggestionsRequest
{
    [Required]
    [StringLength(100)]
    public string Partial { get; set; } = string.Empty;

    public int MaxSuggestions { get; set; } = 10;
    public List<string>? Categories { get; set; }
    public bool IncludePopular { get; set; } = true;
    public bool IncludeRecent { get; set; } = true;
}

/// <summary>
/// Request para analytics de busca
/// </summary>
public class SearchAnalyticsRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? SearchTerms { get; set; }
    public List<string>? Categories { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int TopResults { get; set; } = 50;
    public string GroupBy { get; set; } = "day"; // day, week, month
}

/// <summary>
/// Request para trending topics
/// </summary>
public class TrendingTopicsRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Period { get; set; } = "week"; // day, week, month, quarter
    public List<string>? Categories { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int MaxTopics { get; set; } = 20;
    public float MinTrendScore { get; set; } = 0.5f;
}

/// <summary>
/// Request para estatísticas de conteúdo
/// </summary>
public class ContentStatsRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public bool IncludeInactive { get; set; }
    public bool IncludeDeleted { get; set; }
}

/// <summary>
/// DTO para termos de busca populares
/// </summary>
public class PopularSearchTermDto
{
    public string Term { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Trend { get; set; } = "stable"; // up, down, stable
}

/// <summary>
/// DTO para resultados populares
/// </summary>
public class PopularResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int ClickCount { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// DTO para categorias populares
/// </summary>
public class PopularCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public int SearchCount { get; set; }
    public int ResultCount { get; set; }
}

/// <summary>
/// Request para exportação de dados
/// </summary>
public class SearchExportRequest : CorporateSearchRequest
{
    [Required]
    [StringLength(20)]
    public string Format { get; set; } = "json"; // json, csv, excel

    public bool IncludeAnalytics { get; set; } = true;
    public bool IncludeMetadata { get; set; }
    public int MaxResults { get; set; } = 1000;
}
