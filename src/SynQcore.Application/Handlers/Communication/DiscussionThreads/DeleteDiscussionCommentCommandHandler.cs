using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Common.Helpers;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

public partial class DeleteDiscussionCommentCommandHandler : IRequestHandler<DeleteDiscussionCommentCommand, CommentOperationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly DiscussionThreadHelper _threadHelper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteDiscussionCommentCommandHandler> _logger;

    public DeleteDiscussionCommentCommandHandler(
        ISynQcoreDbContext context,
        DiscussionThreadHelper threadHelper,
        ICurrentUserService currentUserService,
        ILogger<DeleteDiscussionCommentCommandHandler> logger)
    {
        _context = context;
        _threadHelper = threadHelper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CommentOperationResponse> Handle(DeleteDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        LogDeletingComment(_logger, request.CommentId);

        try
        {
            var currentUserId = _currentUserService.UserId;

            // Busca o comentário com seus replies
            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Replies)
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

            // Verifica permissões
            var canDelete = await CanUserDeleteCommentAsync(currentUserId, comment, cancellationToken);
            if (!canDelete)
            {
                LogUnauthorizedDelete(_logger, request.CommentId, currentUserId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Você não tem permissão para excluir este comentário."
                };
            }

            // Verifica se tem replies - soft delete se tem, hard delete se não tem
            var hasReplies = comment.Replies.Count > 0;

            if (hasReplies)
            {
                // Soft delete - mantém a estrutura da thread
                comment.IsDeleted = true;
                comment.Content = "[Comentário removido]";
                comment.UpdatedAt = DateTime.UtcNow;

                LogCommentSoftDeleted(_logger, comment.Id, comment.Replies.Count);
            }
            else
            {
                // Hard delete - remove completamente

                // Remove menções relacionadas
                var mentions = await _context.CommentMentions
                    .Where(m => m.CommentId == comment.Id)
                    .ToListAsync(cancellationToken);

                _context.CommentMentions.RemoveRange(mentions);

                // Remove likes relacionados
                var likes = await _context.CommentLikes
                    .Where(l => l.CommentId == comment.Id)
                    .ToListAsync(cancellationToken);

                _context.CommentLikes.RemoveRange(likes);

                // Remove endorsements relacionados
                var endorsements = await _context.Endorsements
                    .Where(e => e.CommentId == comment.Id)
                    .ToListAsync(cancellationToken);

                _context.Endorsements.RemoveRange(endorsements);

                // Remove o comentário
                _context.Comments.Remove(comment);

                LogCommentHardDeleted(_logger, comment.Id);
            }

            // Atualiza contadores do comentário pai
            if (comment.ParentCommentId.HasValue)
            {
                await _threadHelper.UpdateReplyCountsAsync(comment.ParentCommentId.Value, -1, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new CommentOperationResponse
            {
                Success = true,
                Message = hasReplies
                    ? "Comentário removido da discussão."
                    : "Comentário excluído permanentemente."
            };
        }
        catch (Exception ex)
        {
            LogErrorDeletingComment(_logger, ex, request.CommentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro interno do servidor."
            };
        }
    }

    private async Task<bool> CanUserDeleteCommentAsync(Guid userId, Domain.Entities.Communication.Comment comment, CancellationToken cancellationToken)
    {
        // Autor sempre pode deletar próprio comentário
        if (comment.AuthorId == userId)
            return true;

        // Usa o helper para verificar permissões de moderação
        return await _threadHelper.CanUserModerateCommentAsync(userId, comment.Id, cancellationToken);
    }

    [LoggerMessage(EventId = 1301, Level = LogLevel.Information,
        Message = "Excluindo comentário: {CommentId}")]
    private static partial void LogDeletingComment(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1302, Level = LogLevel.Warning,
        Message = "Comentário não encontrado: {CommentId}")]
    private static partial void LogCommentNotFound(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1303, Level = LogLevel.Warning,
        Message = "Usuário {UserId} sem permissão para excluir comentário: {CommentId}")]
    private static partial void LogUnauthorizedDelete(ILogger logger, Guid commentId, Guid userId);

    [LoggerMessage(EventId = 1304, Level = LogLevel.Information,
        Message = "Comentário soft deleted: {CommentId}, tinha {ReplyCount} replies")]
    private static partial void LogCommentSoftDeleted(ILogger logger, Guid commentId, int replyCount);

    [LoggerMessage(EventId = 1305, Level = LogLevel.Information,
        Message = "Comentário hard deleted: {CommentId}")]
    private static partial void LogCommentHardDeleted(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1306, Level = LogLevel.Error,
        Message = "Erro ao excluir comentário: {CommentId}")]
    private static partial void LogErrorDeletingComment(ILogger logger, Exception ex, Guid commentId);
}
