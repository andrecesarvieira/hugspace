using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Queries.Communication.DiscussionThreads;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Api.Controllers;

// Controller para gerenciamento de Discussion Threads corporativas
[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class DiscussionThreadsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DiscussionThreadsController> _logger;

    public DiscussionThreadsController(IMediator mediator, ILogger<DiscussionThreadsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Cria um novo comentário em discussion thread
    [HttpPost("comments")]
    [ProducesResponseType(typeof(CommentOperationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentOperationResponse>> CreateComment([FromBody] CreateDiscussionCommentCommand request)
    {
        try
        {
            var result = await _mediator.Send(request);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message, errors = result.ValidationErrors });
            }

            return CreatedAtAction(nameof(GetComment), new { commentId = result.Comment!.Id }, result);
        }
        catch (Exception ex)
        {
            LogErrorCreatingComment(_logger, ex, request.PostId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Atualiza um comentário existente
    [HttpPut("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(CommentOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CommentOperationResponse>> UpdateComment(
        Guid commentId, 
        [FromBody] UpdateDiscussionCommentDto dto)
    {
        try
        {
            var command = new UpdateDiscussionCommentCommand(
                commentId,
                dto.Content,
                dto.Type,
                dto.Visibility,
                dto.IsConfidential,
                dto.Priority
            );

            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message, errors = result.ValidationErrors });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorUpdatingComment(_logger, ex, commentId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Exclui um comentário
    [HttpDelete("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(CommentOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CommentOperationResponse>> DeleteComment(Guid commentId)
    {
        try
        {
            var command = new DeleteDiscussionCommentCommand(commentId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorDeletingComment(_logger, ex, commentId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Obtém um comentário específico com contexto
    [HttpGet("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(DiscussionCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiscussionCommentDto>> GetComment(
        Guid commentId,
        [FromQuery] bool includeReplies = true,
        [FromQuery] int maxReplyDepth = 3)
    {
        try
        {
            var query = new GetDiscussionCommentQuery(commentId, includeReplies, maxReplyDepth);
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { message = "Comentário não encontrado" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingComment(_logger, ex, commentId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Resolve um comentário (Questions/Concerns/Actions)
    [HttpPost("comments/{commentId:guid}/resolve")]
    [ProducesResponseType(typeof(CommentOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CommentOperationResponse>> ResolveComment(
        Guid commentId, 
        [FromBody] ResolveCommentDto dto)
    {
        try
        {
            var command = new ResolveDiscussionCommentCommand(commentId, dto.ResolutionNote);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorResolvingComment(_logger, ex, commentId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Modera um comentário (apenas Manager/HR/Admin)
    [HttpPost("comments/{commentId:guid}/moderate")]
    [Authorize(Roles = "Manager,HR,Admin")]
    [ProducesResponseType(typeof(CommentOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CommentOperationResponse>> ModerateComment(
        Guid commentId, 
        [FromBody] ModerateCommentDto dto)
    {
        try
        {
            var command = new ModerateDiscussionCommentCommand(commentId, dto.ModerationStatus, dto.ModerationReason);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorModeratingComment(_logger, ex, commentId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Destaca ou remove destaque de um comentário
    [HttpPost("comments/{commentId:guid}/highlight")]
    [ProducesResponseType(typeof(CommentOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CommentOperationResponse>> HighlightComment(
        Guid commentId, 
        [FromQuery] bool highlight = true)
    {
        try
        {
            var command = new HighlightDiscussionCommentCommand(commentId, highlight);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorHighlightingComment(_logger, ex, commentId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Obtém thread completa de discussão de um post
    [HttpGet("posts/{postId:guid}/thread")]
    [ProducesResponseType(typeof(DiscussionThreadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiscussionThreadDto>> GetDiscussionThread(
        Guid postId,
        [FromQuery] bool includeModerated = false,
        [FromQuery] string? filterByType = null,
        [FromQuery] string orderBy = "CreatedAt")
    {
        try
        {
            var query = new GetDiscussionThreadQuery(postId, includeModerated, filterByType, orderBy);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingThread(_logger, ex, postId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Busca comentários por critérios específicos
    [HttpGet("comments/search")]
    [ProducesResponseType(typeof(List<DiscussionCommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DiscussionCommentDto>>> SearchComments(
        [FromQuery] string searchTerm,
        [FromQuery] Guid? postId = null,
        [FromQuery] string? commentType = null,
        [FromQuery] string? moderationStatus = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new SearchDiscussionCommentsQuery(
                searchTerm, postId, commentType, moderationStatus, 
                fromDate, toDate, page, pageSize);
                
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorSearchingComments(_logger, ex, searchTerm);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Obtém comentários pendentes de moderação (Manager/HR/Admin)
    [HttpGet("moderation/pending")]
    [Authorize(Roles = "Manager,HR,Admin")]
    [ProducesResponseType(typeof(List<DiscussionCommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DiscussionCommentDto>>> GetPendingModerationComments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? departmentId = null)
    {
        try
        {
            var query = new GetPendingModerationCommentsQuery(page, pageSize, departmentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingPendingComments(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Obtém menções do usuário atual ou especificado
    [HttpGet("mentions")]
    [ProducesResponseType(typeof(List<MentionNotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MentionNotificationDto>>> GetUserMentions(
        [FromQuery] Guid? employeeId = null,
        [FromQuery] bool onlyUnread = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var targetEmployeeId = employeeId ?? Guid.Empty;
            
            var query = new GetUserMentionsQuery(targetEmployeeId, onlyUnread, page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingMentions(_logger, ex, employeeId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    // Marca uma menção como lida
    [HttpPost("mentions/{mentionId:guid}/mark-read")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<bool>> MarkMentionAsRead(Guid mentionId)
    {
        try
        {
            var command = new MarkMentionAsReadCommand(mentionId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorMarkingMentionAsRead(_logger, ex, mentionId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 3001, Level = LogLevel.Error,
        Message = "Erro ao criar comentário no post {PostId}")]
    private static partial void LogErrorCreatingComment(ILogger logger, Exception ex, Guid postId);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Error,
        Message = "Erro ao atualizar comentário {CommentId}")]
    private static partial void LogErrorUpdatingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Error,
        Message = "Erro ao excluir comentário {CommentId}")]
    private static partial void LogErrorDeletingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 3004, Level = LogLevel.Error,
        Message = "Erro ao obter comentário {CommentId}")]
    private static partial void LogErrorGettingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 3005, Level = LogLevel.Error,
        Message = "Erro ao resolver comentário {CommentId}")]
    private static partial void LogErrorResolvingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 3006, Level = LogLevel.Error,
        Message = "Erro ao moderar comentário {CommentId}")]
    private static partial void LogErrorModeratingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 3007, Level = LogLevel.Error,
        Message = "Erro ao destacar comentário {CommentId}")]
    private static partial void LogErrorHighlightingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 3008, Level = LogLevel.Error,
        Message = "Erro ao obter thread do post {PostId}")]
    private static partial void LogErrorGettingThread(ILogger logger, Exception ex, Guid postId);

    [LoggerMessage(EventId = 3009, Level = LogLevel.Error,
        Message = "Erro na busca de comentários com termo: {SearchTerm}")]
    private static partial void LogErrorSearchingComments(ILogger logger, Exception ex, string searchTerm);

    [LoggerMessage(EventId = 3010, Level = LogLevel.Error,
        Message = "Erro ao obter comentários pendentes de moderação")]
    private static partial void LogErrorGettingPendingComments(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3011, Level = LogLevel.Error,
        Message = "Erro ao obter comentários não resolvidos do post {PostId}")]
    private static partial void LogErrorGettingUnresolvedComments(ILogger logger, Exception ex, Guid postId);

    [LoggerMessage(EventId = 3012, Level = LogLevel.Error,
        Message = "Erro ao obter menções do usuário {EmployeeId}")]
    private static partial void LogErrorGettingMentions(ILogger logger, Exception ex, Guid? employeeId);

    [LoggerMessage(EventId = 3013, Level = LogLevel.Error,
        Message = "Erro ao marcar menção como lida {MentionId}")]
    private static partial void LogErrorMarkingMentionAsRead(ILogger logger, Exception ex, Guid mentionId);

    #endregion
}