using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.Queries;
using SynQcore.Application.DTOs;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento do feed corporativo personalizado
/// Fornece endpoints para visualização, interação e personalização do feed
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém o feed corporativo personalizado do usuário
    /// </summary>
    /// <param name="feedType">Tipo do feed: mixed, official, department, interests</param>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 20, máximo: 100)</param>
    /// <param name="sortBy">Ordenação: relevance, date, popularity</param>
    /// <param name="refreshFeed">Força regeneração do feed</param>
    /// <param name="postTypes">Filtro por tipos de post</param>
    /// <param name="onlyUnread">Apenas itens não lidos</param>
    /// <param name="onlyBookmarked">Apenas itens bookmarkados</param>
    /// <param name="fromDate">Data inicial do filtro</param>
    /// <param name="toDate">Data final do filtro</param>
    /// <param name="tags">Filtro por tags</param>
    /// <param name="categories">Filtro por categorias</param>
    [HttpGet]
    public async Task<ActionResult<CorporateFeedResponseDto>> GetCorporateFeed(
        [FromQuery] string? feedType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool refreshFeed = false,
        [FromQuery] string[]? postTypes = null,
        [FromQuery] bool? onlyUnread = null,
        [FromQuery] bool? onlyBookmarked = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] string[]? tags = null,
        [FromQuery] string[]? categories = null)
    {
        var userId = GetCurrentUserId();
        
        // Valida parâmetros
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        FeedFiltersDto? filters = null;
        if (HasFilters(postTypes, onlyUnread, onlyBookmarked, fromDate, toDate, tags, categories))
        {
            filters = new FeedFiltersDto
            {
                PostTypes = postTypes?.ToList(),
                OnlyUnread = onlyUnread,
                OnlyBookmarked = onlyBookmarked,
                FromDate = fromDate,
                ToDate = toDate,
                Tags = tags?.ToList(),
                Categories = categories?.ToList()
            };
        }

        var query = new GetCorporateFeedQuery
        {
            UserId = userId,
            FeedType = feedType,
            PageNumber = page,
            PageSize = pageSize,
            SortBy = sortBy,
            RefreshFeed = refreshFeed,
            Filters = filters
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Marca item do feed como lido
    /// </summary>
    /// <param name="feedEntryId">ID da entrada do feed</param>
    [HttpPut("{feedEntryId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid feedEntryId)
    {
        var userId = GetCurrentUserId();
        
        var command = new MarkFeedItemAsReadCommand
        {
            FeedEntryId = feedEntryId,
            UserId = userId
        };

        await _mediator.Send(command);
        return Ok(new { message = "Item marcado como lido com sucesso" });
    }

    /// <summary>
    /// Alterna bookmark do item do feed
    /// </summary>
    /// <param name="feedEntryId">ID da entrada do feed</param>
    [HttpPut("{feedEntryId}/bookmark")]
    public async Task<IActionResult> ToggleBookmark(Guid feedEntryId)
    {
        var userId = GetCurrentUserId();
        
        var command = new ToggleFeedBookmarkCommand
        {
            FeedEntryId = feedEntryId,
            UserId = userId
        };

        await _mediator.Send(command);
        return Ok(new { message = "Bookmark atualizado com sucesso" });
    }

    /// <summary>
    /// Oculta item do feed
    /// </summary>
    /// <param name="feedEntryId">ID da entrada do feed</param>
    /// <param name="reason">Motivo para ocultar (opcional)</param>
    [HttpPut("{feedEntryId}/hide")]
    public async Task<IActionResult> HideItem(Guid feedEntryId, [FromBody] string? reason = null)
    {
        var userId = GetCurrentUserId();
        
        var command = new HideFeedItemCommand
        {
            FeedEntryId = feedEntryId,
            UserId = userId,
            Reason = reason
        };

        await _mediator.Send(command);
        return Ok(new { message = "Item ocultado com sucesso" });
    }

    /// <summary>
    /// Regenera completamente o feed do usuário
    /// </summary>
    /// <param name="preserveBookmarks">Preservar itens bookmarkados</param>
    /// <param name="daysToInclude">Dias para incluir no feed (padrão: 30)</param>
    /// <param name="maxItems">Máximo de itens no feed</param>
    [HttpPost("regenerate")]
    public async Task<IActionResult> RegenerateFeed(
        [FromQuery] bool preserveBookmarks = true,
        [FromQuery] int daysToInclude = 30,
        [FromQuery] int? maxItems = null)
    {
        var userId = GetCurrentUserId();
        
        var command = new RegenerateFeedCommand
        {
            UserId = userId,
            PreserveBookmarks = preserveBookmarks,
            DaysToInclude = Math.Min(daysToInclude, 90), // Máximo 90 dias
            MaxItems = maxItems
        };

        await _mediator.Send(command);
        return Ok(new { message = "Feed regenerado com sucesso" });
    }

    /// <summary>
    /// Atualiza interesses do usuário baseado em interação
    /// </summary>
    /// <param name="contentId">ID do conteúdo</param>
    /// <param name="interactionType">Tipo de interação: like, comment, share, view, bookmark</param>
    /// <param name="recalculateScores">Recalcular todos os scores</param>
    [HttpPost("interests/update")]
    public async Task<IActionResult> UpdateInterests(
        [FromQuery] Guid contentId,
        [FromQuery] string interactionType,
        [FromQuery] bool recalculateScores = false)
    {
        var userId = GetCurrentUserId();
        
        // Valida tipo de interação
        var validTypes = new[] { "like", "comment", "share", "view", "bookmark" };
        if (!validTypes.Contains(interactionType.ToLowerInvariant()))
        {
            return BadRequest(new { message = "Tipo de interação inválido" });
        }

        var command = new UpdateUserInterestsCommand
        {
            UserId = userId,
            ContentId = contentId,
            InteractionType = interactionType,
            RecalculateScores = recalculateScores
        };

        await _mediator.Send(command);
        return Ok(new { message = "Interesses atualizados com sucesso" });
    }

    /// <summary>
    /// Obtém contadores e estatísticas do feed
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<FeedStatsDto>> GetFeedStats()
    {
        var userId = GetCurrentUserId();
        
        var query = new GetFeedStatsQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtém interesses atuais do usuário
    /// </summary>
    [HttpGet("interests")]
    public async Task<ActionResult<UserInterestsResponseDto>> GetUserInterests()
    {
        var userId = GetCurrentUserId();
        
        var query = new GetUserInterestsQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Processa atualizações em lote do feed (para processos internos)
    /// </summary>
    /// <param name="postIds">IDs dos posts a processar</param>
    /// <param name="updateType">Tipo de atualização: new_post, post_updated, post_deleted</param>
    [HttpPost("bulk-update")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ProcessBulkUpdate(
        [FromBody] List<Guid> postIds,
        [FromQuery] string updateType = "new_post")
    {
        var command = new ProcessBulkFeedUpdateCommand
        {
            PostIds = postIds,
            UpdateType = updateType
        };

        await _mediator.Send(command);
        return Ok(new { message = $"Processamento em lote iniciado para {postIds.Count} posts" });
    }

    /// <summary>
    /// Obtém ID do usuário atual do contexto JWT
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value ?? 
                         User.FindFirst("id")?.Value ?? 
                         User.FindFirst("userId")?.Value;
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("ID do usuário não encontrado no token");
        }
        
        return userId;
    }

    /// <summary>
    /// Verifica se há filtros aplicados
    /// </summary>
    private static bool HasFilters(string[]? postTypes, bool? onlyUnread, bool? onlyBookmarked,
        DateOnly? fromDate, DateOnly? toDate, string[]? tags, string[]? categories)
    {
        return postTypes?.Length > 0 ||
               onlyUnread.HasValue ||
               onlyBookmarked.HasValue ||
               fromDate.HasValue ||
               toDate.HasValue ||
               tags?.Length > 0 ||
               categories?.Length > 0;
    }
}