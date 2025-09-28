using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Queries.Communication.DiscussionThreads;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de Discussion Threads corporativas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class DiscussionThreadsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DiscussionThreadsController> _logger;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public DiscussionThreadsController(IMediator mediator, ILogger<DiscussionThreadsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo comentário em discussion thread
    /// </summary>
    /// <param name="request">Dados do comentário a ser criado</param>
    /// <returns>Resposta da operação de criação do comentário</returns>
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

    /// <summary>
    /// Atualiza um comentário existente
    /// </summary>
    /// <param name="commentId">ID do comentário a ser atualizado</param>
    /// <param name="dto">Dados de atualização do comentário</param>
    /// <returns>Resposta da operação de atualização do comentário</returns>
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

    /// <summary>
    /// Exclui um comentário
    /// </summary>
    /// <param name="commentId">ID do comentário a ser excluído</param>
    /// <returns>Resposta da operação de exclusão do comentário</returns>
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

    /// <summary>
    /// Obtém um comentário específico com contexto
    /// </summary>
    /// <param name="commentId">ID do comentário</param>
    /// <param name="includeReplies">Se deve incluir respostas do comentário</param>
    /// <param name="maxReplyDepth">Profundidade máxima de respostas aninhadas</param>
    /// <returns>Dados do comentário com contexto</returns>
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

    /// <summary>
    /// Resolve um comentário (Questions/Concerns/Actions)
    /// </summary>
    /// <param name="commentId">ID do comentário a ser resolvido</param>
    /// <param name="dto">Dados de resolução do comentário</param>
    /// <returns>Resposta da operação de resolução do comentário</returns>
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

    /// <summary>
    /// Modera um comentário (apenas Manager/HR/Admin)
    /// </summary>
    /// <param name="commentId">ID do comentário a ser moderado</param>
    /// <param name="dto">Dados de moderação do comentário</param>
    /// <returns>Resposta da operação de moderação do comentário</returns>
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

    /// <summary>
    /// Destaca ou remove destaque de um comentário
    /// </summary>
    /// <param name="commentId">ID do comentário</param>
    /// <param name="highlight">Se o comentário deve ser destacado</param>
    /// <returns>Resposta da operação de destaque do comentário</returns>
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

    /// <summary>
    /// Obtém thread completa de discussão de um post
    /// </summary>
    /// <param name="postId">ID do post</param>
    /// <param name="includeModerated">Se deve incluir comentários moderados</param>
    /// <param name="filterByType">Filtro por tipo de comentário</param>
    /// <param name="orderBy">Critério de ordenação dos comentários</param>
    /// <returns>Thread completa de discussão do post</returns>
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

    /// <summary>
    /// Busca comentários por critérios específicos
    /// </summary>
    /// <param name="searchTerm">Termo de busca</param>
    /// <param name="postId">ID do post para filtrar</param>
    /// <param name="commentType">Tipo de comentário</param>
    /// <param name="moderationStatus">Status de moderação</param>
    /// <param name="fromDate">Data inicial</param>
    /// <param name="toDate">Data final</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de comentários encontrados</returns>
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

    /// <summary>
    /// Obtém comentários pendentes de moderação (Manager/HR/Admin)
    /// </summary>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <returns>Lista paginada de comentários pendentes de moderação</returns>
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

    /// <summary>
    /// Obtém menções do usuário atual ou especificado
    /// </summary>
    /// <param name="employeeId">ID do funcionário (opcional)</param>
    /// <param name="onlyUnread">Se deve retornar apenas menções não lidas</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de menções do usuário</returns>
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

    /// <summary>
    /// Marca uma menção como lida
    /// </summary>
    /// <param name="mentionId">ID da menção</param>
    /// <returns>Confirmação da operação</returns>
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