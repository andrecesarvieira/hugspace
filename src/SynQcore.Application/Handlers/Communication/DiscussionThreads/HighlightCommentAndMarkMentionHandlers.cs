using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.Common.Extensions;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

/// <summary>
/// Handler para destacar/deshighlightar comentários importantes.
/// Permite destacar comentários relevantes para melhor visibilidade na thread.
/// </summary>
public partial class HighlightDiscussionCommentCommandHandler : IRequestHandler<HighlightDiscussionCommentCommand, CommentOperationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<HighlightDiscussionCommentCommandHandler> _logger;

    /// <summary>
    /// Inicializa nova instância do handler de destaque de comentários.
    /// </summary>
    /// <param name="context">Contexto de acesso a dados.</param>
    /// <param name="currentUserService">Serviço de usuário atual.</param>
    /// <param name="logger">Logger para rastreamento de operações.</param>
    public HighlightDiscussionCommentCommandHandler(
        ISynQcoreDbContext context,

        ICurrentUserService currentUserService,
        ILogger<HighlightDiscussionCommentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Processa destaque/remoção de destaque do comentário.
    /// </summary>
    /// <param name="request">Command contendo ID do comentário e status de destaque.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação com comentário destacado.</returns>
    public async Task<CommentOperationResponse> Handle(HighlightDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        LogHighlightingComment(_logger, request.CommentId, request.IsHighlighted);

        try
        {
            var currentUserId = _currentUserService.UserId;

            // Busca o comentário
            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

            if (comment == null)
            {
                LogCommentNotFound(_logger, request.CommentId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Comentário não encontrado."
                };
            }

            // Verifica permissões para destacar
            var canHighlight = await CanUserHighlightCommentAsync(currentUserId, comment, cancellationToken);
            if (!canHighlight)
            {
                LogUnauthorizedHighlight(_logger, request.CommentId, currentUserId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Você não tem permissão para destacar este comentário."
                };
            }

            // Verifica se já está no estado desejado
            if (comment.IsHighlighted == request.IsHighlighted)
            {
                var existingStateMessage = request.IsHighlighted
                    ? "Este comentário já está destacado."
                    : "Este comentário não está destacado.";

                return new CommentOperationResponse
                {
                    Success = false,
                    Message = existingStateMessage
                };
            }

            // Aplica o highlight
            comment.IsHighlighted = request.IsHighlighted;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var action = request.IsHighlighted ? "destacado" : "removido destaque";
            LogCommentHighlightChanged(_logger, comment.Id, action, currentUserId);

            // Retorna o comentário atualizado
            var updatedComment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedEmployee)
                .FirstOrDefaultAsync(c => c.Id == comment.Id, cancellationToken);

            var commentDto = updatedComment.ToDiscussionCommentDto();

            return new CommentOperationResponse
            {
                Success = true,
                Message = $"Comentário {action} com sucesso.",
                Comment = commentDto
            };
        }
        catch (Exception ex)
        {
            LogErrorHighlightingComment(_logger, ex, request.CommentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro interno do servidor."
            };
        }
    }

    /// Verifica se o usuário pode destacar o comentário
    private async Task<bool> CanUserHighlightCommentAsync(Guid userId, Domain.Entities.Communication.Comment comment, CancellationToken cancellationToken)
    {
        // Autor do comentário pode destacar
        if (comment.AuthorId == userId)
            return true;

        // Autor do post pode destacar comentários em seu post
        if (comment.Post.AuthorId == userId)
            return true;

        // Managers podem destacar comentários de subordinados
        var isSubordinate = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == comment.AuthorId)
            .AnyAsync(e => e.ManagerId == userId, cancellationToken);

        if (isSubordinate)
            return true;

        // HR e Admin poderiam destacar qualquer comentário
        // Esta validação seria feita através de roles
        return false;
    }

    [LoggerMessage(EventId = 1601, Level = LogLevel.Information,
        Message = "Alterando highlight do comentário: {CommentId}, destacado: {IsHighlighted}")]
    private static partial void LogHighlightingComment(ILogger logger, Guid commentId, bool isHighlighted);

    [LoggerMessage(EventId = 1602, Level = LogLevel.Warning,
        Message = "Comentário não encontrado: {CommentId}")]
    private static partial void LogCommentNotFound(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1603, Level = LogLevel.Warning,
        Message = "Usuário {UserId} sem permissão para destacar comentário: {CommentId}")]
    private static partial void LogUnauthorizedHighlight(ILogger logger, Guid commentId, Guid userId);

    [LoggerMessage(EventId = 1604, Level = LogLevel.Information,
        Message = "Comentário {Action}: {CommentId}, por: {UserId}")]
    private static partial void LogCommentHighlightChanged(ILogger logger, Guid commentId, string action, Guid userId);

    [LoggerMessage(EventId = 1605, Level = LogLevel.Error,
        Message = "Erro ao alterar highlight do comentário: {CommentId}")]
    private static partial void LogErrorHighlightingComment(ILogger logger, Exception ex, Guid commentId);
}

/// <summary>
/// Handler para marcar menção como lida.
/// Processa marcação de menções em comentários como visualizadas pelo usuário.
/// </summary>
public partial class MarkMentionAsReadCommandHandler : IRequestHandler<MarkMentionAsReadCommand, bool>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<MarkMentionAsReadCommandHandler> _logger;

    /// <summary>
    /// Inicializa nova instância do handler de marcação de menções.
    /// </summary>
    /// <param name="context">Contexto de acesso a dados.</param>
    /// <param name="currentUserService">Serviço de usuário atual.</param>
    /// <param name="logger">Logger para rastreamento de operações.</param>
    public MarkMentionAsReadCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<MarkMentionAsReadCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Processa marcação de menção como lida verificando autorização.
    /// </summary>
    /// <param name="request">Command contendo ID da menção.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se marcada com sucesso, false caso contrário.</returns>
    public async Task<bool> Handle(MarkMentionAsReadCommand request, CancellationToken cancellationToken)
    {
        LogMarkingMentionAsRead(_logger, request.MentionId);

        try
        {
            var currentUserId = _currentUserService.UserId;

            // Busca a menção
            var mention = await _context.CommentMentions
                .FirstOrDefaultAsync(m => m.Id == request.MentionId, cancellationToken);

            if (mention == null)
            {
                LogMentionNotFound(_logger, request.MentionId);
                return false;
            }

            // Verifica se é o usuário mencionado
            if (mention.MentionedEmployeeId != currentUserId)
            {
                LogUnauthorizedMarkAsRead(_logger, request.MentionId, currentUserId);
                return false;
            }

            // Verifica se já está marcada como lida
            if (mention.IsRead)
            {
                LogMentionAlreadyRead(_logger, request.MentionId);
                return true; // Retorna true pois o objetivo já foi alcançado
            }

            // Marca como lida
            mention.IsRead = true;
            mention.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogMentionMarkedAsRead(_logger, mention.Id, currentUserId);

            return true;
        }
        catch (Exception ex)
        {
            LogErrorMarkingMentionAsRead(_logger, ex, request.MentionId);
            return false;
        }
    }

    [LoggerMessage(EventId = 1701, Level = LogLevel.Information,
        Message = "Marcando menção como lida: {MentionId}")]
    private static partial void LogMarkingMentionAsRead(ILogger logger, Guid mentionId);

    [LoggerMessage(EventId = 1702, Level = LogLevel.Warning,
        Message = "Menção não encontrada: {MentionId}")]
    private static partial void LogMentionNotFound(ILogger logger, Guid mentionId);

    [LoggerMessage(EventId = 1703, Level = LogLevel.Warning,
        Message = "Usuário {UserId} sem permissão para marcar menção como lida: {MentionId}")]
    private static partial void LogUnauthorizedMarkAsRead(ILogger logger, Guid mentionId, Guid userId);

    [LoggerMessage(EventId = 1704, Level = LogLevel.Information,
        Message = "Menção já estava marcada como lida: {MentionId}")]
    private static partial void LogMentionAlreadyRead(ILogger logger, Guid mentionId);

    [LoggerMessage(EventId = 1705, Level = LogLevel.Information,
        Message = "Menção marcada como lida: {MentionId}, por: {UserId}")]
    private static partial void LogMentionMarkedAsRead(ILogger logger, Guid mentionId, Guid userId);

    [LoggerMessage(EventId = 1706, Level = LogLevel.Error,
        Message = "Erro ao marcar menção como lida: {MentionId}")]
    private static partial void LogErrorMarkingMentionAsRead(ILogger logger, Exception ex, Guid mentionId);
}
