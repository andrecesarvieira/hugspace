using System.Net.Http.Json;
using System.Text.Json;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço para busca corporativa com integração real à API
/// </summary>
public partial class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SearchService> _logger;

    // Cache de JsonSerializerOptions para performance
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Executando busca corporativa: {Query}")]
    private static partial void LogSearchStarted(ILogger logger, string query);

    [LoggerMessage(LogLevel.Information, "Busca concluída - Resultados: {Count}")]
    private static partial void LogSearchCompleted(ILogger logger, int count);

    [LoggerMessage(LogLevel.Error, "Erro ao executar busca corporativa: {Query}")]
    private static partial void LogSearchError(ILogger logger, string query, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro em operação de busca")]
    private static partial void LogOperationError(ILogger logger, Exception exception);

    public SearchService(HttpClient httpClient, ILogger<SearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResult<SearchResultDto>> SearchAsync(string query, int page = 1, int pageSize = 20, SearchFiltersDto? filters = null)
    {
        LogSearchStarted(_logger, query);

        try
        {
            // Construir query string para busca básica (GET)
            var queryParams = new List<string>
            {
                $"query={Uri.EscapeDataString(query)}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (filters != null)
            {
                if (filters.ContentTypes?.Count > 0)
                {
                    queryParams.AddRange(filters.ContentTypes.Select(ct => $"filters.ContentTypes={Uri.EscapeDataString(ct)}"));
                }
                if (filters.Categories?.Count > 0)
                {
                    queryParams.AddRange(filters.Categories.Select(cat => $"filters.Categories={Uri.EscapeDataString(cat)}"));
                }
                if (filters.DepartmentIds?.Count > 0)
                {
                    queryParams.AddRange(filters.DepartmentIds.Select(dept => $"filters.DepartmentIds={dept}"));
                }
                if (filters.AuthorIds?.Count > 0)
                {
                    queryParams.AddRange(filters.AuthorIds.Select(author => $"filters.AuthorIds={author}"));
                }
                if (filters.Tags?.Count > 0)
                {
                    queryParams.AddRange(filters.Tags.Select(tag => $"filters.Tags={Uri.EscapeDataString(tag)}"));
                }
            }

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/CorporateSearch?{queryString}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogSearchError(_logger, query, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> AdvancedSearchAsync(AdvancedSearchRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/CorporateSearch/advanced", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<List<SearchSuggestionDto>> GetSearchSuggestionsAsync(string partialText, int maxSuggestions = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/CorporateSearch/suggestions?partial={Uri.EscapeDataString(partialText)}&maxSuggestions={maxSuggestions}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<SearchSuggestionDto>>(_jsonOptions);
            return result ?? new List<SearchSuggestionDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new List<SearchSuggestionDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByCategoryAsync(string category, int page = 1, int pageSize = 20, SearchFiltersDto? filters = null)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/CorporateSearch/category/{Uri.EscapeDataString(category)}?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByAuthorAsync(Guid authorId, int page = 1, int pageSize = 20, string? contentType = null)
    {
        try
        {
            var url = $"/api/CorporateSearch/author/{authorId}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(contentType))
                url += $"&contentType={Uri.EscapeDataString(contentType)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByDepartmentAsync(Guid departmentId, int page = 1, int pageSize = 20, string? contentType = null)
    {
        try
        {
            var url = $"/api/CorporateSearch/department/{departmentId}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(contentType))
                url += $"&contentType={Uri.EscapeDataString(contentType)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByTagsAsync(List<string> tags, bool allTags = true, int page = 1, int pageSize = 20)
    {
        try
        {
            var request = new
            {
                Tags = tags,
                AllTags = allTags,
                Page = page,
                PageSize = pageSize
            };

            var response = await _httpClient.PostAsJsonAsync("/api/CorporateSearch/tags", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> GetRecentContentAsync(int hours = 24, List<string>? contentTypes = null, int page = 1, int pageSize = 20)
    {
        try
        {
            var url = $"/api/CorporateSearch/recent?hours={hours}&page={page}&pageSize={pageSize}";
            if (contentTypes?.Count > 0)
                url += $"&contentTypes={string.Join(",", contentTypes.Select(Uri.EscapeDataString))}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> GetPopularContentAsync(string period = "week", List<string>? contentTypes = null, int page = 1, int pageSize = 20)
    {
        try
        {
            var url = $"/api/CorporateSearch/popular?period={Uri.EscapeDataString(period)}&page={page}&pageSize={pageSize}";
            if (contentTypes?.Count > 0)
                url += $"&contentTypes={string.Join(",", contentTypes.Select(Uri.EscapeDataString))}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            LogSearchCompleted(_logger, result?.Items?.Count ?? 0);

            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<ContentStatsDto> GetContentStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var url = "/api/CorporateSearch/stats";
            var queryParams = new List<string>();

            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stats = await response.Content.ReadFromJsonAsync<ContentStatsDto>(_jsonOptions);
            return stats ?? new ContentStatsDto();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new ContentStatsDto();
        }
    }

    public async Task<List<TrendingTopicDto>> GetTrendingTopicsAsync(string period = "week", int maxTopics = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/CorporateSearch/trending?period={Uri.EscapeDataString(period)}&maxTopics={maxTopics}");
            response.EnsureSuccessStatusCode();

            var topics = await response.Content.ReadFromJsonAsync<List<TrendingTopicDto>>(_jsonOptions);
            return topics ?? new List<TrendingTopicDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new List<TrendingTopicDto>();
        }
    }

    public async Task<List<SearchResultDto>> GetSimilarContentAsync(Guid contentId, string contentType, int maxResults = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/CorporateSearch/similar/{contentId}?contentType={Uri.EscapeDataString(contentType)}&maxResults={maxResults}");
            response.EnsureSuccessStatusCode();

            var similar = await response.Content.ReadFromJsonAsync<List<SearchResultDto>>(_jsonOptions);
            return similar ?? new List<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new List<SearchResultDto>();
        }
    }

    public async Task<SearchConfigDto> GetSearchConfigAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/CorporateSearch/config");
            response.EnsureSuccessStatusCode();

            var config = await response.Content.ReadFromJsonAsync<SearchConfigDto>(_jsonOptions);
            return config ?? new SearchConfigDto();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new SearchConfigDto();
        }
    }

    public async Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var url = "/api/CorporateSearch/analytics";
            var queryParams = new List<string>();

            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var analytics = await response.Content.ReadFromJsonAsync<SearchAnalyticsDto>(_jsonOptions);
            return analytics ?? new SearchAnalyticsDto();
        }
        catch (Exception ex)
        {
            LogOperationError(_logger, ex);
            return new SearchAnalyticsDto();
        }
    }
}