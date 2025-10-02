using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;

namespace SynQcore.BlazorApp.Services;

public interface ISearchService
{
    /// <summary>
    /// Executa busca corporativa geral
    /// </summary>
    Task<PagedResult<SearchResultDto>> SearchAsync(string query, int page = 1, int pageSize = 20, SearchFiltersDto? filters = null);

    /// <summary>
    /// Executa busca avançada com múltiplos critérios
    /// </summary>
    Task<PagedResult<SearchResultDto>> AdvancedSearchAsync(AdvancedSearchRequest request);

    /// <summary>
    /// Obtém sugestões de busca baseadas em entrada parcial
    /// </summary>
    Task<List<SearchSuggestionDto>> GetSearchSuggestionsAsync(string partial, int maxSuggestions = 10);

    /// <summary>
    /// Busca por categoria específica
    /// </summary>
    Task<PagedResult<SearchResultDto>> SearchByCategoryAsync(string category, int page = 1, int pageSize = 20, SearchFiltersDto? filters = null);

    /// <summary>
    /// Busca por autor específico
    /// </summary>
    Task<PagedResult<SearchResultDto>> SearchByAuthorAsync(Guid authorId, int page = 1, int pageSize = 20, string? contentType = null);

    /// <summary>
    /// Busca por departamento específico
    /// </summary>
    Task<PagedResult<SearchResultDto>> SearchByDepartmentAsync(Guid departmentId, int page = 1, int pageSize = 20, string? contentType = null);

    /// <summary>
    /// Busca por tags
    /// </summary>
    Task<PagedResult<SearchResultDto>> SearchByTagsAsync(List<string> tags, bool allTags = true, int page = 1, int pageSize = 20);

    /// <summary>
    /// Obtém conteúdo recente
    /// </summary>
    Task<PagedResult<SearchResultDto>> GetRecentContentAsync(int hours = 24, List<string>? contentTypes = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// Obtém conteúdo popular
    /// </summary>
    Task<PagedResult<SearchResultDto>> GetPopularContentAsync(string period = "week", List<string>? contentTypes = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// Obtém estatísticas de conteúdo
    /// </summary>
    Task<ContentStatsDto> GetContentStatsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Obtém tópicos em tendência
    /// </summary>
    Task<List<TrendingTopicDto>> GetTrendingTopicsAsync(string period = "week", int maxTopics = 20);

    /// <summary>
    /// Obtém conteúdo similar
    /// </summary>
    Task<List<SearchResultDto>> GetSimilarContentAsync(Guid contentId, string contentType, int maxResults = 10);

    /// <summary>
    /// Obtém configuração de busca
    /// </summary>
    Task<SearchConfigDto> GetSearchConfigAsync();

    /// <summary>
    /// Obtém analytics de busca
    /// </summary>
    Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}