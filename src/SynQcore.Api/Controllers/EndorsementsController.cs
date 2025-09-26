using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SynQcore.Application.Features.Collaboration.Commands;
using SynQcore.Application.Features.Collaboration.Queries;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Common.DTOs;
using System.Security.Claims;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para sistema de endorsements corporativos com funcionalidades avançadas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EndorsementsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EndorsementsController> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, Exception?> LogProcessingEndorsementRequest =
        LoggerMessage.Define(LogLevel.Information, new EventId(4001, nameof(LogProcessingEndorsementRequest)), 
            "Processando requisição de endorsements");

    private static readonly Action<ILogger, int, Exception?> LogEndorsementSearchCompleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(4002, nameof(LogEndorsementSearchCompleted)), 
            "Busca de endorsements realizada: {Count} resultados");

    private static readonly Action<ILogger, Guid, Exception?> LogEndorsementCreationRequest =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(4003, nameof(LogEndorsementCreationRequest)), 
            "Requisição de criação de endorsement por usuário: {UserId}");

    private static readonly Action<ILogger, Guid, Exception?> LogEndorsementCreated =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(4004, nameof(LogEndorsementCreated)), 
            "Endorsement criado com sucesso: {EndorsementId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogEndorsementUpdateRequest =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Information, new EventId(4005, nameof(LogEndorsementUpdateRequest)), 
            "Requisição de atualização do endorsement {EndorsementId} por usuário {UserId}");

    private static readonly Action<ILogger, Guid, Exception?> LogEndorsementUpdated =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(4006, nameof(LogEndorsementUpdated)), 
            "Endorsement atualizado com sucesso: {EndorsementId}");

    private static readonly Action<ILogger, Guid, Guid, Exception?> LogEndorsementDeleteRequest =
        LoggerMessage.Define<Guid, Guid>(LogLevel.Information, new EventId(4007, nameof(LogEndorsementDeleteRequest)),
            "Requisição de exclusão do endorsement {EndorsementId} por usuário {UserId}");

    private static readonly Action<ILogger, Exception?> LogEndorsementError =
        LoggerMessage.Define(LogLevel.Error, new EventId(4008, nameof(LogEndorsementError)), 
            "Erro ao buscar endorsements");

    private static readonly Action<ILogger, Guid, Exception?> LogEndorsementByIdError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4009, nameof(LogEndorsementByIdError)), 
            "Erro ao buscar endorsement {EndorsementId}");

    private static readonly Action<ILogger, Guid, Exception?> LogPostEndorsementsError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4010, nameof(LogPostEndorsementsError)), 
            "Erro ao buscar endorsements do post {PostId}");

    private static readonly Action<ILogger, Guid, Exception?> LogCommentEndorsementsError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4011, nameof(LogCommentEndorsementsError)), 
            "Erro ao buscar endorsements do comment {CommentId}");

    private static readonly Action<ILogger, Exception?> LogEndorsementStatsError =
        LoggerMessage.Define(LogLevel.Error, new EventId(4012, nameof(LogEndorsementStatsError)), 
            "Erro ao calcular estatísticas de endorsements");

    private static readonly Action<ILogger, Guid, Exception?> LogCreateEndorsementError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4013, nameof(LogCreateEndorsementError)), 
            "Erro ao criar endorsement por usuário {UserId}");

    private static readonly Action<ILogger, Guid, Exception?> LogToggleEndorsementError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4014, nameof(LogToggleEndorsementError)), 
            "Erro no toggle de endorsement por usuário {UserId}");

    private static readonly Action<ILogger, Guid, Exception?> LogUpdateEndorsementError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4015, nameof(LogUpdateEndorsementError)), 
            "Erro ao atualizar endorsement {EndorsementId}");

    private static readonly Action<ILogger, Guid, Exception?> LogDeleteEndorsementError =
        LoggerMessage.Define<Guid>(LogLevel.Error, new EventId(4016, nameof(LogDeleteEndorsementError)),
            "Erro ao excluir endorsement {EndorsementId}");

    private static readonly Action<ILogger, Guid, Exception?> LogEndorsementDeleted =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(4008, nameof(LogEndorsementDeleted)), 
            "Endorsement excluído com sucesso: {EndorsementId}");

    private static readonly Action<ILogger, EndorsementType, Guid, Exception?> LogEndorsementToggleRequest =
        LoggerMessage.Define<EndorsementType, Guid>(LogLevel.Information, new EventId(4009, nameof(LogEndorsementToggleRequest)), 
            "Toggle endorsement {Type} por usuário {UserId}");

    private static readonly Action<ILogger, bool, Exception?> LogEndorsementToggleCompleted =
        LoggerMessage.Define<bool>(LogLevel.Information, new EventId(4010, nameof(LogEndorsementToggleCompleted)), 
            "Toggle endorsement completado - Criado: {IsCreated}");

    private static readonly Action<ILogger, Exception?> LogInvalidAuthenticatedUser =
        LoggerMessage.Define(LogLevel.Warning, new EventId(4011, nameof(LogInvalidAuthenticatedUser)), 
            "Tentativa de operação sem usuário autenticado válido");

    public EndorsementsController(IMediator mediator, ILogger<EndorsementsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obter ID do usuário autenticado dos claims JWT
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Buscar endorsements com filtros avançados e paginação
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(PagedResult<EndorsementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<EndorsementDto>>> GetEndorsements([FromQuery] EndorsementSearchDto searchRequest)
    {
        LogProcessingEndorsementRequest(_logger, null);

        try
        {
            var query = new GetEndorsementsQuery { SearchRequest = searchRequest };
            var result = await _mediator.Send(query);
            
            LogEndorsementSearchCompleted(_logger, result.TotalCount, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogEndorsementError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar endorsements.");
        }
    }

    /// <summary>
    /// Obter endorsement específico por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(EndorsementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndorsementDto>> GetEndorsement(Guid id)
    {
        try
        {
            var query = new GetEndorsementByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogEndorsementByIdError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar endorsement.");
        }
    }

    /// <summary>
    /// Obter endorsements de um post específico
    /// </summary>
    [HttpGet("post/{postId:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(List<EndorsementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EndorsementDto>>> GetPostEndorsements(
        Guid postId, 
        [FromQuery] EndorsementType? filterByType = null,
        [FromQuery] bool includePrivate = false,
        [FromQuery] string sortBy = "EndorsedAt",
        [FromQuery] bool sortDescending = true)
    {
        try
        {
            var query = new GetPostEndorsementsQuery 
            { 
                PostId = postId,
                FilterByType = filterByType,
                IncludePrivate = includePrivate,
                SortBy = sortBy,
                SortDescending = sortDescending
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogPostEndorsementsError(_logger, postId, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Obter endorsements de um comentário específico
    /// </summary>
    [HttpGet("comment/{commentId:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(List<EndorsementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EndorsementDto>>> GetCommentEndorsements(
        Guid commentId, 
        [FromQuery] EndorsementType? filterByType = null,
        [FromQuery] bool includePrivate = false,
        [FromQuery] string sortBy = "EndorsedAt",
        [FromQuery] bool sortDescending = true)
    {
        try
        {
            var query = new GetCommentEndorsementsQuery 
            { 
                CommentId = commentId,
                FilterByType = filterByType,
                IncludePrivate = includePrivate,
                SortBy = sortBy,
                SortDescending = sortDescending
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogCommentEndorsementsError(_logger, commentId, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Obter estatísticas de endorsements de conteúdo
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(EndorsementStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndorsementStatsDto>> GetEndorsementStats(
        [FromQuery] Guid? postId = null,
        [FromQuery] Guid? commentId = null,
        [FromQuery] bool includePrivate = false)
    {
        try
        {
            var query = new GetEndorsementStatsQuery 
            { 
                PostId = postId,
                CommentId = commentId,
                IncludePrivate = includePrivate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogEndorsementStatsError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Criar novo endorsement corporativo
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(EndorsementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndorsementDto>> CreateEndorsement([FromBody] CreateEndorsementDto createDto)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogEndorsementCreationRequest(_logger, userId, null);

        try
        {
            var command = new CreateEndorsementCommand 
            { 
                Data = createDto, 
                EndorserId = userId 
            };
            var result = await _mediator.Send(command);
            
            LogEndorsementCreated(_logger, result.Id, null);
            return CreatedAtAction(nameof(GetEndorsement), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogCreateEndorsementError(_logger, userId, ex);
            return StatusCode(500, "Erro interno do servidor ao criar endorsement.");
        }
    }

    /// <summary>
    /// Toggle endorsement (adicionar/remover rapidamente)
    /// </summary>
    [HttpPost("toggle")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(EndorsementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndorsementDto?>> ToggleEndorsement(
        [FromQuery] Guid? postId = null,
        [FromQuery] Guid? commentId = null,
        [FromQuery] EndorsementType type = EndorsementType.Helpful,
        [FromQuery] string? context = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogEndorsementToggleRequest(_logger, type, userId, null);

        try
        {
            var command = new ToggleEndorsementCommand 
            { 
                PostId = postId,
                CommentId = commentId,
                Type = type,
                EndorserId = userId,
                Context = context
            };
            var result = await _mediator.Send(command);
            
            LogEndorsementToggleCompleted(_logger, result != null, null);
            
            if (result != null)
            {
                return Ok(result); // Endorsement criado
            }
            else
            {
                return NoContent(); // Endorsement removido
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogToggleEndorsementError(_logger, userId, ex);
            return StatusCode(500, "Erro interno do servidor no toggle de endorsement.");
        }
    }

    /// <summary>
    /// Atualizar endorsement existente (apenas autor)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(EndorsementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EndorsementDto>> UpdateEndorsement(Guid id, [FromBody] UpdateEndorsementDto updateDto)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogEndorsementUpdateRequest(_logger, id, userId, null);

        try
        {
            var command = new UpdateEndorsementCommand 
            { 
                Id = id, 
                Data = updateDto, 
                UserId = userId 
            };
            var result = await _mediator.Send(command);
            
            LogEndorsementUpdated(_logger, id, null);
            return Ok(result);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("não encontrado"))
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogUpdateEndorsementError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor ao atualizar endorsement.");
        }
    }

    /// <summary>
    /// Excluir endorsement (apenas autor)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteEndorsement(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            LogInvalidAuthenticatedUser(_logger, null);
            return Unauthorized("Usuário não autenticado.");
        }

        LogEndorsementDeleteRequest(_logger, id, userId, null);

        try
        {
            var command = new DeleteEndorsementCommand 
            { 
                Id = id, 
                UserId = userId 
            };
            await _mediator.Send(command);
            
            LogEndorsementDeleted(_logger, id, null);
            return NoContent();
        }
        catch (ArgumentException ex) when (ex.Message.Contains("não encontrado"))
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            LogDeleteEndorsementError(_logger, id, ex);
            return StatusCode(500, "Erro interno do servidor ao excluir endorsement.");
        }
    }
}