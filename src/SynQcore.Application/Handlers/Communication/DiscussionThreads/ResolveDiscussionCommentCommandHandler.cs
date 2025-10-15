using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

public partial class ResolveDiscussionCommentCommandHandler : IRequestHandler<ResolveDiscussionCommentCommand, CommentOperationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ResolveDiscussionCommentCommandHandler> _logger;

    public ResolveDiscussionCommentCommandHandler(
        ISynQcoreDbContext context,

        ICurrentUserService currentUserService,
        ILogger<ResolveDiscussionCommentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CommentOperationResponse> Handle(ResolveDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        LogResolvingComment(_logger, request.CommentId);

        try
        {
            var currentUserId = _currentUserService.UserId;

            // Busca o comentário
            var comment = await _context.Comments
                .Include(c => c.Author)
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

            // Verifica se é um tipo de comentário que pode ser resolvido
            var resolvableTypes = new[] { CommentType.Question, CommentType.Concern, CommentType.Action };
            if (!resolvableTypes.Contains(comment.Type))
            {
                LogInvalidTypeForResolution(_logger, request.CommentId, comment.Type.ToString());
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = $"Comentários do tipo '{comment.Type}' não podem ser marcados como resolvidos."
                };
            }

            // Verifica se já está resolvido
            if (comment.IsResolved)
            {
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Este comentário já foi resolvido."
                };
            }

            // Verifica permissões para resolver
            var canResolve = await CanUserResolveCommentAsync(currentUserId, comment, cancellationToken);
            if (!canResolve)
            {
                LogUnauthorizedResolve(_logger, request.CommentId, currentUserId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Você não tem permissão para resolver este comentário."
                };
            }

            // Marca como resolvido
            comment.IsResolved = true;
            comment.ResolvedById = currentUserId;
            comment.ResolvedAt = DateTime.UtcNow;
            comment.ResolutionNote = request.ResolutionNote;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogCommentResolved(_logger, comment.Id, comment.Type.ToString(), currentUserId);

            // Retorna o comentário atualizado
            var resolvedComment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.ResolvedBy)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedEmployee)
                .FirstOrDefaultAsync(c => c.Id == comment.Id, cancellationToken);

            var commentDto = resolvedComment.ToDiscussionCommentDto();

            return new CommentOperationResponse
            {
                Success = true,
                Message = $"Comentário do tipo '{comment.Type}' foi marcado como resolvido.",
                Comment = commentDto
            };
        }
        catch (Exception ex)
        {
            LogErrorResolvingComment(_logger, ex, request.CommentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro interno do servidor."
            };
        }
    }

    private async Task<bool> CanUserResolveCommentAsync(Guid userId, Comment comment, CancellationToken cancellationToken)
    {
        // Autor do comentário sempre pode marcar como resolvido
        if (comment.AuthorId == userId)
            return true;

        // Autor do post pode resolver comentários em seu post
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == comment.PostId, cancellationToken);

        if (post?.AuthorId == userId)
            return true;

        // Managers podem resolver comentários de subordinados
        var isSubordinate = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == comment.AuthorId)
            .AnyAsync(e => e.ManagerId == userId, cancellationToken);

        if (isSubordinate)
            return true;

        // Para comentários do tipo Action, qualquer pessoa mencionada pode resolver
        if (comment.Type == CommentType.Action)
        {
            var isMentioned = await _context.CommentMentions
                .AsNoTracking()
                .AnyAsync(m => m.CommentId == comment.Id && m.MentionedEmployeeId == userId, cancellationToken);

            if (isMentioned)
                return true;
        }

        return false;
    }

    [LoggerMessage(EventId = 1401, Level = LogLevel.Information,
        Message = "Resolvendo comentário: {CommentId}")]
    private static partial void LogResolvingComment(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1402, Level = LogLevel.Warning,
        Message = "Comentário não encontrado: {CommentId}")]
    private static partial void LogCommentNotFound(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1403, Level = LogLevel.Warning,
        Message = "Tipo de comentário {Type} não pode ser resolvido: {CommentId}")]
    private static partial void LogInvalidTypeForResolution(ILogger logger, Guid commentId, string type);

    [LoggerMessage(EventId = 1404, Level = LogLevel.Warning,
        Message = "Usuário {UserId} sem permissão para resolver comentário: {CommentId}")]
    private static partial void LogUnauthorizedResolve(ILogger logger, Guid commentId, Guid userId);

    [LoggerMessage(EventId = 1405, Level = LogLevel.Information,
        Message = "Comentário resolvido: {CommentId}, tipo: {Type}, resolvido por: {UserId}")]
    private static partial void LogCommentResolved(ILogger logger, Guid commentId, string type, Guid userId);

    [LoggerMessage(EventId = 1406, Level = LogLevel.Error,
        Message = "Erro ao resolver comentário: {CommentId}")]
    private static partial void LogErrorResolvingComment(ILogger logger, Exception ex, Guid commentId);
}
