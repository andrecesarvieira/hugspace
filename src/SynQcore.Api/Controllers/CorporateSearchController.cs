using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Application.Features.CorporateSearch.Queries;
using SynQcore.Application.Features.CorporateSearch.Commands;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para funcionalidades de busca corporativa e analytics
/// Fase 4.4 - Corporate Search & Analytics System
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CorporateSearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CorporateSearchController> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, string, string, Exception?> LogProcessandoBusca =
        LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(4401, nameof(LogProcessandoBusca)),
            "Processando requisição de busca corporativa - Termo: {SearchTerm}, Usuário: {UserId}");

    private static readonly Action<ILogger, int, Exception?> LogBuscaConcluida =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(4402, nameof(LogBuscaConcluida)),
            "Busca corporativa concluída: {ResultCount} resultados");

    private static readonly Action<ILogger, string, Exception?> LogProcessandoAnalytics =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(4403, nameof(LogProcessandoAnalytics)),
            "Processando requisição de analytics - Usuário: {UserId}");

    private static readonly Action<ILogger, string, Exception?> LogProcessandoTrending =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(4404, nameof(LogProcessandoTrending)),
            "Processando requisição de trending topics - Usuário: {UserId}");

    private static readonly Action<ILogger, Exception?> LogErroProcessamentoBusca =
        LoggerMessage.Define(LogLevel.Error, new EventId(4499, nameof(LogErroProcessamentoBusca)),
            "Erro no processamento de busca corporativa");

    public CorporateSearchController(IMediator mediator, ILogger<CorporateSearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Executa busca corporativa básica
    /// </summary>
    /// <param name="request">Parâmetros de busca</param>
    /// <returns>Resultados paginados da busca</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> Search([FromQuery] CorporateSearchRequest request)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        LogProcessandoBusca(_logger, request.Query, userId, null);

        try
        {
            var query = new CorporateSearchQuery(request.Query, request.Page, request.PageSize)
            {
                Filters = request.Filters,
                Config = request.Config
            };

            var result = await _mediator.Send(query);

            LogBuscaConcluida(_logger, result.Items.Count, null);

            // Registrar evento de busca para analytics
            var recordCommand = new RecordSearchEventCommand(request.Query, Guid.Empty, result.TotalCount);
            await _mediator.Send(recordCommand);

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno no processamento da busca");
        }
    }

    /// <summary>
    /// Executa busca avançada com múltiplos critérios
    /// </summary>
    /// <param name="request">Parâmetros de busca avançada</param>
    /// <returns>Resultados paginados da busca avançada</returns>
    [HttpPost("advanced")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> AdvancedSearch([FromBody] AdvancedSearchRequest request)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        LogProcessandoBusca(_logger, request.Query ?? "advanced", userId, null);

        try
        {
            var query = new AdvancedSearchQuery
            {
                Query = request.Query,
                Title = request.Title,
                Content = request.Content,
                Author = request.Author,
                ExactPhrase = request.ExactPhrase,
                AllWords = request.AllWords,
                AnyWords = request.AnyWords,
                ExcludeWords = request.ExcludeWords,
                Page = request.Page,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Config = request.Config
            };

            var result = await _mediator.Send(query);

            LogBuscaConcluida(_logger, result.Items.Count, null);

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno no processamento da busca avançada");
        }
    }

    /// <summary>
    /// Obtém sugestões de busca baseadas em texto parcial
    /// </summary>
    /// <param name="request">Parâmetros para sugestões</param>
    /// <returns>Lista de sugestões de busca</returns>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(List<SearchSuggestionDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<List<SearchSuggestionDto>>> GetSuggestions([FromQuery] SearchSuggestionsRequest request)
    {
        try
        {
            var query = new GetSearchSuggestionsQuery(request.Partial)
            {
                MaxSuggestions = request.MaxSuggestions,
                Categories = request.Categories,
                IncludePopular = request.IncludePopular,
                IncludeRecent = request.IncludeRecent
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno ao obter sugestões");
        }
    }

    /// <summary>
    /// Busca conteúdo por categoria específica
    /// </summary>
    /// <param name="category">Nome da categoria</param>
    /// <param name="request">Parâmetros adicionais de busca</param>
    /// <returns>Resultados paginados por categoria</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> SearchByCategory(
        [FromRoute] string category,
        [FromQuery] SearchByCategoryRequest request)
    {
        try
        {
            var query = new SearchByCategoryQuery(category)
            {
                SubCategory = request.SubCategory,
                Page = request.Page,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Config = request.Config
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca por categoria");
        }
    }

    /// <summary>
    /// Busca conteúdo por autor específico
    /// </summary>
    /// <param name="authorId">ID do autor</param>
    /// <param name="request">Parâmetros adicionais de busca</param>
    /// <returns>Resultados paginados por autor</returns>
    [HttpGet("author/{authorId:guid}")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> SearchByAuthor(
        [FromRoute] Guid authorId,
        [FromQuery] SearchByAuthorRequest request)
    {
        try
        {
            var query = new SearchByAuthorQuery(authorId)
            {
                ContentType = request.ContentType,
                Page = request.Page,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Config = request.Config
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca por autor");
        }
    }

    /// <summary>
    /// Busca conteúdo por departamento específico
    /// </summary>
    /// <param name="departmentId">ID do departamento</param>
    /// <param name="request">Parâmetros adicionais de busca</param>
    /// <returns>Resultados paginados por departamento</returns>
    [HttpGet("department/{departmentId:guid}")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> SearchByDepartment(
        [FromRoute] Guid departmentId,
        [FromQuery] SearchByDepartmentRequest request)
    {
        try
        {
            var query = new SearchByDepartmentQuery(departmentId)
            {
                ContentType = request.ContentType,
                Page = request.Page,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Config = request.Config
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca por departamento");
        }
    }

    /// <summary>
    /// Busca conteúdo por tags específicas
    /// </summary>
    /// <param name="request">Parâmetros de busca por tags</param>
    /// <returns>Resultados paginados por tags</returns>
    [HttpPost("tags")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> SearchByTags([FromBody] SearchByTagsRequest request)
    {
        try
        {
            var query = new SearchByTagsQuery(request.Tags)
            {
                AllTags = request.AllTags,
                Page = request.Page,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Config = request.Config
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca por tags");
        }
    }

    /// <summary>
    /// Obtém conteúdo similar a um item específico
    /// </summary>
    /// <param name="contentId">ID do conteúdo de referência</param>
    /// <param name="contentType">Tipo do conteúdo</param>
    /// <param name="maxResults">Número máximo de resultados</param>
    /// <returns>Lista de conteúdo similar</returns>
    [HttpGet("similar/{contentId:guid}")]
    [ProducesResponseType(typeof(List<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<List<SearchResultDto>>> GetSimilarContent(
        [FromRoute] Guid contentId,
        [FromQuery] string contentType,
        [FromQuery] int maxResults = 10)
    {
        try
        {
            var query = new GetSimilarContentQuery(contentId, contentType)
            {
                MaxResults = maxResults
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca de conteúdo similar");
        }
    }

    /// <summary>
    /// Obtém conteúdo recente baseado em período
    /// </summary>
    /// <param name="request">Parâmetros para conteúdo recente</param>
    /// <returns>Resultados paginados de conteúdo recente</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> GetRecentContent([FromQuery] GetRecentContentRequest request)
    {
        try
        {
            var query = new GetRecentContentQuery
            {
                Hours = request.Hours,
                ContentTypes = request.ContentTypes,
                DepartmentIds = request.DepartmentIds,
                Page = request.Page,
                PageSize = request.PageSize,
                Config = request.Config
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca de conteúdo recente");
        }
    }

    /// <summary>
    /// Obtém conteúdo popular baseado em métricas de engajamento
    /// </summary>
    /// <param name="request">Parâmetros para conteúdo popular</param>
    /// <returns>Resultados paginados de conteúdo popular</returns>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(PagedResult<SearchResultDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> GetPopularContent([FromQuery] GetPopularContentRequest request)
    {
        try
        {
            var query = new GetPopularContentQuery
            {
                Period = request.Period,
                ContentTypes = request.ContentTypes,
                DepartmentIds = request.DepartmentIds,
                Page = request.Page,
                PageSize = request.PageSize,
                Config = request.Config,
                MetricType = request.MetricType
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na busca de conteúdo popular");
        }
    }

    /// <summary>
    /// Obtém analytics de busca corporativa
    /// </summary>
    /// <param name="request">Parâmetros para analytics</param>
    /// <returns>Dados de analytics paginados</returns>
    [HttpGet("analytics")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(PagedResult<SearchAnalyticsDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<PagedResult<SearchAnalyticsDto>>> GetSearchAnalytics([FromQuery] SearchAnalyticsRequest request)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        LogProcessandoAnalytics(_logger, userId, null);

        try
        {
            var query = new GetSearchAnalyticsQuery
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SearchTerms = request.SearchTerms,
                Categories = request.Categories,
                DepartmentIds = request.DepartmentIds,
                Page = 1,
                PageSize = 20,
                TopResults = request.TopResults,
                GroupBy = request.GroupBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno ao obter analytics");
        }
    }

    /// <summary>
    /// Obtém trending topics corporativos
    /// </summary>
    /// <param name="request">Parâmetros para trending topics</param>
    /// <returns>Lista de trending topics</returns>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(List<TrendingTopicDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<List<TrendingTopicDto>>> GetTrendingTopics([FromQuery] TrendingTopicsRequest request)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        LogProcessandoTrending(_logger, userId, null);

        try
        {
            var query = new GetTrendingTopicsQuery
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Period = request.Period,
                Categories = request.Categories,
                DepartmentIds = request.DepartmentIds,
                MaxTopics = request.MaxTopics,
                MinTrendScore = request.MinTrendScore
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno ao obter trending topics");
        }
    }

    /// <summary>
    /// Obtém estatísticas gerais de conteúdo
    /// </summary>
    /// <param name="request">Parâmetros para estatísticas</param>
    /// <returns>Estatísticas de conteúdo</returns>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ContentStatsDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ContentStatsDto>> GetContentStats([FromQuery] ContentStatsRequest request)
    {
        try
        {
            var query = new GetContentStatsQuery
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ContentTypes = request.ContentTypes,
                DepartmentIds = request.DepartmentIds,
                IncludeInactive = request.IncludeInactive,
                IncludeDeleted = request.IncludeDeleted
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno ao obter estatísticas");
        }
    }

    /// <summary>
    /// Exporta dados de busca em formato específico
    /// </summary>
    /// <param name="request">Parâmetros para exportação</param>
    /// <returns>Dados exportados</returns>
    [HttpPost("export")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(SearchExportDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<SearchExportDto>> ExportSearchData([FromBody] SearchExportRequest request)
    {
        try
        {
            var query = new ExportSearchDataQuery(request.Query, request.Format)
            {
                Filters = request.Filters,
                Config = request.Config,
                IncludeAnalytics = request.IncludeAnalytics,
                IncludeMetadata = request.IncludeMetadata,
                MaxResults = request.MaxResults
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno na exportação");
        }
    }

    /// <summary>
    /// Registra clique em resultado de busca para analytics
    /// </summary>
    /// <param name="request">Dados do clique</param>
    /// <returns>Confirmação de registro</returns>
    [HttpPost("click")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<bool>> RecordSearchClick([FromBody] RecordSearchClickRequest request)
    {
        try
        {
            var command = new RecordSearchClickCommand(request.SearchTerm, request.ResultId, request.ResultType, request.Position, Guid.Empty);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErroProcessamentoBusca(_logger, ex);
            return StatusCode(500, "Erro interno ao registrar clique");
        }
    }
}

// Request DTOs complementares para o controller

public class SearchByCategoryRequest
{
    public string? SubCategory { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
}

public class SearchByAuthorRequest
{
    public string? ContentType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
}

public class SearchByDepartmentRequest
{
    public string? ContentType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
}

public class SearchByTagsRequest
{
    [Required]
    public List<string> Tags { get; set; } = new();
    public bool AllTags { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
}

public class GetRecentContentRequest
{
    public int Hours { get; set; } = 24;
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchConfigDto? Config { get; set; }
}

public class GetPopularContentRequest
{
    public string Period { get; set; } = "week";
    public List<string>? ContentTypes { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public SearchConfigDto? Config { get; set; }
    public string MetricType { get; set; } = "engagement";
}

public class RecordSearchClickRequest
{
    [Required]
    public string SearchTerm { get; set; } = string.Empty;
    [Required]
    public Guid ResultId { get; set; }
    [Required]
    public string ResultType { get; set; } = string.Empty;
    [Required]
    public string ResultTitle { get; set; } = string.Empty;
    public int Position { get; set; }
    public float RelevanceScore { get; set; }
}
