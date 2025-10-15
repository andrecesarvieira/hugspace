using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.KnowledgeManagement.Commands;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Features.KnowledgeManagement.Queries;

namespace SynQcore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
/// <summary>
/// Classe para operações do sistema
/// </summary>
public class KnowledgePostsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<KnowledgePostsController> _logger;

    // LoggerMessage delegates para alta performance
    private static readonly Action<ILogger, Exception?> LogProcessingKnowledgeSearch =
        LoggerMessage.Define(LogLevel.Information, new EventId(1001, nameof(LogProcessingKnowledgeSearch)),
            "Processando busca de artigos de conhecimento");

    private static readonly Action<ILogger, int, Exception?> LogSearchCompleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(1002, nameof(LogSearchCompleted)),
            "Busca realizada com sucesso. {Count} artigos encontrados");

    private static readonly Action<ILogger, Exception?> LogSearchError =
        LoggerMessage.Define(LogLevel.Error, new EventId(1003, nameof(LogSearchError)),
            "Erro ao buscar artigos de conhecimento");

    private static readonly Action<ILogger, Guid, Exception?> LogSearchingArticle =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1004, nameof(LogSearchingArticle)),
            "Buscando artigo de conhecimento: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogArticleFound =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1005, nameof(LogArticleFound)),
            "Artigo encontrado com sucesso: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogArticleNotFound =
        LoggerMessage.Define<Guid>(LogLevel.Warning, new EventId(1006, nameof(LogArticleNotFound)),
            "Artigo não encontrado: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogSearchArticleError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(1007, nameof(LogSearchArticleError)),
            "Erro ao buscar artigo: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogSearchingByCategory =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1008, nameof(LogSearchingByCategory)),
            "Buscando artigos por categoria: {CategoryId}");

    private static readonly Action<ILogger, int, Exception?> LogCategorySearchCompleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(1009, nameof(LogCategorySearchCompleted)),
            "Busca por categoria realizada: {Count} artigos encontrados");

    private static readonly Action<ILogger, Guid, Exception?> LogCategoryNotFound =
        LoggerMessage.Define<Guid>(LogLevel.Warning, new EventId(1010, nameof(LogCategoryNotFound)),
            "Categoria não encontrada: {CategoryId}");

    private static readonly Action<ILogger, Guid, Exception?> LogCategorySearchError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(1011, nameof(LogCategorySearchError)),
            "Erro ao buscar artigos por categoria: {CategoryId}");

    private static readonly Action<ILogger, Guid, Exception?> LogSearchingVersions =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1012, nameof(LogSearchingVersions)),
            "Buscando versões do artigo: {ArticleId}");

    private static readonly Action<ILogger, int, Guid, Exception?> LogVersionsFound =
        LoggerMessage.Define<int, Guid>(LogLevel.Information, new EventId(1013, nameof(LogVersionsFound)),
            "Versões encontradas: {Count} para artigo {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogParentArticleNotFound =
        LoggerMessage.Define<Guid>(LogLevel.Warning, new EventId(1014, nameof(LogParentArticleNotFound)),
            "Artigo pai não encontrado: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogVersionsSearchError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(1015, nameof(LogVersionsSearchError)),
            "Erro ao buscar versões do artigo: {ArticleId}");

    private static readonly Action<ILogger, Exception?> LogInvalidAuthenticatedUser =
        LoggerMessage.Define(LogLevel.Warning, new EventId(1016, nameof(LogInvalidAuthenticatedUser)),
            "Tentativa de criar artigo sem usuário autenticado válido");

    private static readonly Action<ILogger, Guid, Exception?> LogCreatingArticle =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1017, nameof(LogCreatingArticle)),
            "Criando novo artigo de conhecimento por usuário: {UserId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogArticleCreated =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Information, new EventId(1018, nameof(LogArticleCreated)),
            "Artigo criado com sucesso: {ArticleId} por {UserId}");

    private static readonly Action<ILogger, string, Exception?> LogInvalidCreationData =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1019, nameof(LogInvalidCreationData)),
            "Dados inválidos para criação de artigo: {Error}");

    private static readonly Action<ILogger, Guid, Exception?> LogCreationError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(1020, nameof(LogCreationError)),
            "Erro ao criar artigo por usuário: {UserId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogUpdatingArticle =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Information, new EventId(1021, nameof(LogUpdatingArticle)),
            "Atualizando artigo: {ArticleId} por usuário: {UserId}");

    private static readonly Action<ILogger, Guid, Exception?> LogArticleUpdated =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1022, nameof(LogArticleUpdated)),
            "Artigo atualizado com sucesso: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogUpdateNotFound =
        LoggerMessage.Define<Guid>(LogLevel.Warning, new EventId(1023, nameof(LogUpdateNotFound)),
            "Artigo não encontrado para atualização: {ArticleId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogUpdateUnauthorized =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Warning, new EventId(1024, nameof(LogUpdateUnauthorized)),
            "Usuário {UserId} não autorizado a atualizar artigo: {ArticleId}");

    private static readonly Action<ILogger, string, Exception?> LogInvalidUpdateData =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1025, nameof(LogInvalidUpdateData)),
            "Dados inválidos para atualização: {Error}");

    private static readonly Action<ILogger, Guid, Exception?> LogUpdateError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(1026, nameof(LogUpdateError)),
            "Erro ao atualizar artigo: {ArticleId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogDeletingArticle =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Information, new EventId(1027, nameof(LogDeletingArticle)),
            "Excluindo artigo: {ArticleId} por usuário: {UserId}");

    private static readonly Action<ILogger, Guid, Exception?> LogArticleDeleted =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1028, nameof(LogArticleDeleted)),
            "Artigo excluído com sucesso: {ArticleId}");

    private static readonly Action<ILogger, Guid, Exception?> LogDeleteNotFound =
        LoggerMessage.Define<Guid>(LogLevel.Warning, new EventId(1029, nameof(LogDeleteNotFound)),
            "Artigo não encontrado para exclusão: {ArticleId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogDeleteUnauthorized =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Warning, new EventId(1030, nameof(LogDeleteUnauthorized)),
            "Usuário {UserId} não autorizado a excluir artigo: {ArticleId}");

    private static readonly Action<ILogger, string, Exception?> LogDeleteValidationError =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1031, nameof(LogDeleteValidationError)),
            "Erro de validação na exclusão: {Error}");

    private static readonly Action<ILogger, Guid, Exception?> LogDeleteError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(1032, nameof(LogDeleteError)),
            "Erro ao excluir artigo: {ArticleId}");

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public KnowledgePostsController(IMediator mediator, ILogger<KnowledgePostsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Obter ID do usuário autenticado dos claims
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Busca artigos de conhecimento com filtros e paginação
    /// </summary>
    /// <param name="searchRequest">Parâmetros de busca e filtros</param>
    /// <returns>Lista paginada de artigos de conhecimento</returns>
    [HttpGet]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(PagedResult<KnowledgePostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<KnowledgePostDto>>> GetKnowledgePosts([FromQuery] KnowledgePostSearchDto searchRequest)
    {
        LogProcessingKnowledgeSearch(_logger, null);

        try
        {
            var query = new GetKnowledgePostsQuery { SearchRequest = searchRequest };
            var result = await _mediator.Send(query);

            LogSearchCompleted(_logger, result.TotalCount, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogSearchError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar artigos.");
        }
    }

    /// <summary>
    /// Obtém artigo de conhecimento específico por ID com incremento de visualização
    /// </summary>
    /// <param name="id">ID do artigo de conhecimento</param>
    /// <param name="incrementViewCount">Se deve incrementar contador de visualizações</param>
    /// <returns>Dados completos do artigo de conhecimento</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(KnowledgePostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<KnowledgePostDto>> GetKnowledgePost(Guid id, [FromQuery] bool incrementViewCount = true)
    {
        LogSearchingArticle(_logger, id, null);

        try
        {
            var query = new GetKnowledgePostByIdQuery
            {
                Id = id,
                IncrementViewCount = incrementViewCount
            };
            var result = await _mediator.Send(query);

            LogArticleFound(_logger, id, null);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            LogArticleNotFound(_logger, id, null);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogSearchArticleError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar artigo.");
        }
    }

    /// <summary>
    /// Busca artigos de uma categoria específica com paginação e ordenação
    /// </summary>
    /// <param name="categoryId">ID da categoria</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="sortBy">Campo de ordenação</param>
    /// <param name="sortDescending">Se deve ordenar de forma decrescente</param>
    /// <returns>Lista paginada de artigos da categoria</returns>
    [HttpGet("category/{categoryId:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(PagedResult<KnowledgePostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<KnowledgePostDto>>> GetKnowledgePostsByCategory(
        Guid categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true)
    {
        LogSearchingByCategory(_logger, categoryId, null);

        try
        {
            var query = new GetKnowledgePostsByCategoryQuery
            {
                CategoryId = categoryId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };
            var result = await _mediator.Send(query);

            LogCategorySearchCompleted(_logger, result.TotalCount, null);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            LogCategoryNotFound(_logger, categoryId, null);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogCategorySearchError(_logger, categoryId, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Obtém histórico de versões de um artigo de conhecimento
    /// </summary>
    /// <param name="id">ID do artigo de conhecimento</param>
    /// <returns>Lista de versões do artigo</returns>
    [HttpGet("{id:guid}/versions")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(List<KnowledgePostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<KnowledgePostDto>>> GetKnowledgePostVersions(Guid id)
    {
        LogSearchingVersions(_logger, id, null);

        try
        {
            var query = new GetKnowledgePostVersionsQuery { ParentPostId = id };
            var result = await _mediator.Send(query);

            LogVersionsFound(_logger, result.Count, id, null);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            LogParentArticleNotFound(_logger, id, null);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogVersionsSearchError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Cria novo artigo de conhecimento com validações corporativas
    /// </summary>
    /// <param name="createDto">Dados do artigo a ser criado</param>
    /// <returns>Artigo de conhecimento criado</returns>
    [HttpPost]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(KnowledgePostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<KnowledgePostDto>> CreateKnowledgePost([FromBody] CreateKnowledgePostDto createDto)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogCreatingArticle(_logger, userId, null);

        try
        {
            var command = new CreateKnowledgePostCommand
            {
                Data = createDto,
                AuthorId = userId
            };
            var result = await _mediator.Send(command);

            LogArticleCreated(_logger, result.Id, userId, null);
            return CreatedAtAction(nameof(GetKnowledgePost), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            LogInvalidCreationData(_logger, ex.Message, null);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogCreationError(_logger, userId, ex);
            return StatusCode(500, "Erro interno do servidor ao criar artigo.");
        }
    }

    /// <summary>
    /// Atualiza artigo de conhecimento (apenas autor original ou admin)
    /// </summary>
    /// <param name="id">ID do artigo a ser atualizado</param>
    /// <param name="updateDto">Dados de atualização do artigo</param>
    /// <returns>Artigo de conhecimento atualizado</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(KnowledgePostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KnowledgePostDto>> UpdateKnowledgePost(Guid id, [FromBody] UpdateKnowledgePostDto updateDto)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogUpdatingArticle(_logger, id, userId, null);

        try
        {
            var command = new UpdateKnowledgePostCommand
            {
                Id = id,
                Data = updateDto,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            LogArticleUpdated(_logger, id, null);
            return Ok(result);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("não encontrado"))
        {
            LogUpdateNotFound(_logger, id, null);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            LogUpdateUnauthorized(_logger, userId, id, null);
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            LogInvalidUpdateData(_logger, ex.Message, null);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogUpdateError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor ao atualizar artigo.");
        }
    }

    /// <summary>
    /// Exclui artigo de conhecimento (apenas Manager/HR/Admin)
    /// </summary>
    /// <param name="id">ID do artigo a ser excluído</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager,HR,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteKnowledgePost(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogDeletingArticle(_logger, id, userId, null);

        try
        {
            var command = new DeleteKnowledgePostCommand
            {
                Id = id,
                UserId = userId
            };
            await _mediator.Send(command);

            LogArticleDeleted(_logger, id, null);
            return NoContent();
        }
        catch (ArgumentException ex) when (ex.Message.Contains("não encontrado"))
        {
            LogDeleteNotFound(_logger, id, null);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            LogDeleteUnauthorized(_logger, userId, id, null);
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            LogDeleteValidationError(_logger, ex.Message, null);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogDeleteError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor ao excluir artigo.");
        }
    }
}
