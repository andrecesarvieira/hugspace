using System.Net.Http.Json;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using System.Text.Json;

namespace SynQcore.BlazorApp.Services;

public partial class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SearchService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Executando busca corporativa: {query}")]
    private static partial void LogCorporateSearchStarted(ILogger logger, string query, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao executar busca corporativa: {query}")]
    private static partial void LogCorporateSearchError(ILogger logger, string query, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Executando busca avançada")]
    private static partial void LogAdvancedSearchStarted(ILogger logger, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao executar busca avançada")]
    private static partial void LogAdvancedSearchError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo sugestões de busca: {partialText}")]
    private static partial void LogSuggestionsStarted(ILogger logger, string partialText, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter sugestões de busca: {partialText}")]
    private static partial void LogSuggestionsError(ILogger logger, string partialText, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Buscando por categoria: {category}")]
    private static partial void LogCategorySearchStarted(ILogger logger, string category, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar por categoria: {category}")]
    private static partial void LogCategorySearchError(ILogger logger, string category, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Buscando por autor: {authorId}")]
    private static partial void LogAuthorSearchStarted(ILogger logger, Guid authorId, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar por autor: {authorId}")]
    private static partial void LogAuthorSearchError(ILogger logger, Guid authorId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Buscando por departamento: {departmentId}")]
    private static partial void LogDepartmentSearchStarted(ILogger logger, Guid departmentId, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar por departamento: {departmentId}")]
    private static partial void LogDepartmentSearchError(ILogger logger, Guid departmentId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Buscando por tags: {tags}")]
    private static partial void LogTagsSearchStarted(ILogger logger, string tags, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar por tags: {tags}")]
    private static partial void LogTagsSearchError(ILogger logger, string tags, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo conteúdo recente: {hours}h")]
    private static partial void LogRecentContentStarted(ILogger logger, int hours, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter conteúdo recente")]
    private static partial void LogRecentContentError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo conteúdo popular: {period}")]
    private static partial void LogPopularContentStarted(ILogger logger, string period, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter conteúdo popular")]
    private static partial void LogPopularContentError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo estatísticas de conteúdo")]
    private static partial void LogContentStatsStarted(ILogger logger, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter estatísticas de conteúdo")]
    private static partial void LogContentStatsError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo tópicos em tendência: {period}")]
    private static partial void LogTrendingTopicsStarted(ILogger logger, string period, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter tópicos em tendência")]
    private static partial void LogTrendingTopicsError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo conteúdo similar: {contentId}")]
    private static partial void LogSimilarContentStarted(ILogger logger, Guid contentId, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter conteúdo similar: {contentId}")]
    private static partial void LogSimilarContentError(ILogger logger, Guid contentId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo configuração de busca")]
    private static partial void LogSearchConfigStarted(ILogger logger, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter configuração de busca")]
    private static partial void LogSearchConfigError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo analytics de busca")]
    private static partial void LogSearchAnalyticsStarted(ILogger logger, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter analytics de busca")]
    private static partial void LogSearchAnalyticsError(ILogger logger, Exception? exception);

    public SearchService(HttpClient httpClient, ILogger<SearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<PagedResult<SearchResultDto>> SearchAsync(string query, int page = 1, int pageSize = 20, SearchFiltersDto? filters = null)
    {
        try
        {
            LogCorporateSearchStarted(_logger, query);

            var request = new CorporateSearchRequest
            {
                Query = query,
                Page = page,
                PageSize = pageSize,
                Filters = filters
            };

            var response = await _httpClient.PostAsJsonAsync("/api/corporate-search", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogCorporateSearchError(_logger, query, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> AdvancedSearchAsync(AdvancedSearchRequest request)
    {
        try
        {
            LogAdvancedSearchStarted(_logger);

            var response = await _httpClient.PostAsJsonAsync("/api/corporate-search/advanced", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogAdvancedSearchError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<List<SearchSuggestionDto>> GetSearchSuggestionsAsync(string partialText, int maxSuggestions = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(partialText) || partialText.Length < 2)
                return new List<SearchSuggestionDto>();

            LogSuggestionsStarted(_logger, partialText);

            var encodedPartial = Uri.EscapeDataString(partialText);
            var response = await _httpClient.GetAsync($"/api/corporate-search/suggestions?partial={encodedPartial}&maxSuggestions={maxSuggestions}");
            response.EnsureSuccessStatusCode();

            var suggestions = await response.Content.ReadFromJsonAsync<List<SearchSuggestionDto>>(_jsonOptions);
            return suggestions ?? new List<SearchSuggestionDto>();
        }
        catch (Exception ex)
        {
            LogSuggestionsError(_logger, partialText, ex);
            return new List<SearchSuggestionDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByCategoryAsync(string category, int page = 1, int pageSize = 20, SearchFiltersDto? filters = null)
    {
        try
        {
            LogCategorySearchStarted(_logger, category);

            var encodedCategory = Uri.EscapeDataString(category);
            var response = await _httpClient.GetAsync($"/api/corporate-search/category/{encodedCategory}?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogCategorySearchError(_logger, category, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByAuthorAsync(Guid authorId, int page = 1, int pageSize = 20, string? contentType = null)
    {
        try
        {
            LogAuthorSearchStarted(_logger, authorId);

            var url = $"/api/corporate-search/author/{authorId}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(contentType))
                url += $"&contentType={Uri.EscapeDataString(contentType)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogAuthorSearchError(_logger, authorId, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByDepartmentAsync(Guid departmentId, int page = 1, int pageSize = 20, string? contentType = null)
    {
        try
        {
            LogDepartmentSearchStarted(_logger, departmentId);

            var url = $"/api/corporate-search/department/{departmentId}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(contentType))
                url += $"&contentType={Uri.EscapeDataString(contentType)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogDepartmentSearchError(_logger, departmentId, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> SearchByTagsAsync(List<string> tags, bool allTags = true, int page = 1, int pageSize = 20)
    {
        try
        {
            LogTagsSearchStarted(_logger, string.Join(", ", tags));

            var request = new
            {
                Tags = tags,
                AllTags = allTags,
                Page = page,
                PageSize = pageSize
            };

            var response = await _httpClient.PostAsJsonAsync("/api/corporate-search/tags", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogTagsSearchError(_logger, string.Join(", ", tags), ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> GetRecentContentAsync(int hours = 24, List<string>? contentTypes = null, int page = 1, int pageSize = 20)
    {
        try
        {
            LogRecentContentStarted(_logger, hours);

            var url = $"/api/corporate-search/recent?hours={hours}&page={page}&pageSize={pageSize}";
            if (contentTypes?.Count > 0)
                url += $"&contentTypes={string.Join(",", contentTypes.Select(Uri.EscapeDataString))}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogRecentContentError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<PagedResult<SearchResultDto>> GetPopularContentAsync(string period = "week", List<string>? contentTypes = null, int page = 1, int pageSize = 20)
    {
        try
        {
            LogPopularContentStarted(_logger, period);

            var url = $"/api/corporate-search/popular?period={period}&page={page}&pageSize={pageSize}";
            if (contentTypes?.Count > 0)
                url += $"&contentTypes={string.Join(",", contentTypes.Select(Uri.EscapeDataString))}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchResultDto>>(_jsonOptions);
            return result ?? new PagedResult<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogPopularContentError(_logger, ex);
            return new PagedResult<SearchResultDto>();
        }
    }

    public async Task<ContentStatsDto> GetContentStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            LogContentStatsStarted(_logger);

            var url = "/api/corporate-search/stats";
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
            LogContentStatsError(_logger, ex);
            return new ContentStatsDto();
        }
    }

    public async Task<List<TrendingTopicDto>> GetTrendingTopicsAsync(string period = "week", int maxTopics = 20)
    {
        try
        {
            LogTrendingTopicsStarted(_logger, period);

            var response = await _httpClient.GetAsync($"/api/corporate-search/trending?period={period}&maxTopics={maxTopics}");
            response.EnsureSuccessStatusCode();

            var topics = await response.Content.ReadFromJsonAsync<List<TrendingTopicDto>>(_jsonOptions);
            return topics ?? new List<TrendingTopicDto>();
        }
        catch (Exception ex)
        {
            LogTrendingTopicsError(_logger, ex);
            return new List<TrendingTopicDto>();
        }
    }

    public async Task<List<SearchResultDto>> GetSimilarContentAsync(Guid contentId, string contentType, int maxResults = 10)
    {
        try
        {
            LogSimilarContentStarted(_logger, contentId);

            var response = await _httpClient.GetAsync($"/api/corporate-search/similar/{contentId}?contentType={Uri.EscapeDataString(contentType)}&maxResults={maxResults}");
            response.EnsureSuccessStatusCode();

            var similar = await response.Content.ReadFromJsonAsync<List<SearchResultDto>>(_jsonOptions);
            return similar ?? new List<SearchResultDto>();
        }
        catch (Exception ex)
        {
            LogSimilarContentError(_logger, contentId, ex);
            return new List<SearchResultDto>();
        }
    }

    public async Task<SearchConfigDto> GetSearchConfigAsync()
    {
        try
        {
            LogSearchConfigStarted(_logger);

            var response = await _httpClient.GetAsync("/api/corporate-search/config");
            response.EnsureSuccessStatusCode();

            var config = await response.Content.ReadFromJsonAsync<SearchConfigDto>(_jsonOptions);
            return config ?? new SearchConfigDto();
        }
        catch (Exception ex)
        {
            LogSearchConfigError(_logger, ex);
            return new SearchConfigDto();
        }
    }

    public async Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            LogSearchAnalyticsStarted(_logger);

            var url = "/api/corporate-search/analytics";
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
            LogSearchAnalyticsError(_logger, ex);
            return new SearchAnalyticsDto();
        }
    }
}