using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Common.Helpers;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Domain.Entities.Communication;
using AutoMapper;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

/// Handler para moderação de comentários em discussion threads
public partial class ModerateDiscussionCommentCommandHandler : IRequestHandler<ModerateDiscussionCommentCommand, CommentOperationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly DiscussionThreadHelper _threadHelper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ModerateDiscussionCommentCommandHandler> _logger;

    public ModerateDiscussionCommentCommandHandler(
        ISynQcoreDbContext context,
        IMapper mapper,
        DiscussionThreadHelper threadHelper,
        ICurrentUserService currentUserService,
        ILogger<ModerateDiscussionCommentCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _threadHelper = threadHelper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CommentOperationResponse> Handle(ModerateDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        LogModeratingComment(_logger, request.CommentId, request.ModerationStatus);

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

            // Verifica permissões de moderação
            var canModerate = await _threadHelper.CanUserModerateCommentAsync(currentUserId, comment.Id, cancellationToken);
            if (!canModerate)
            {
                LogUnauthorizedModeration(_logger, request.CommentId, currentUserId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Você não tem permissão para moderar este comentário."
                };
            }

            // Converte status de moderação
            var moderationStatus = Enum.Parse<ModerationStatus>(request.ModerationStatus, true);

            // Valida transições de status
            var validTransition = IsValidModerationTransition(comment.ModerationStatus, moderationStatus);
            if (!validTransition)
            {
                LogInvalidModerationTransition(_logger, request.CommentId, comment.ModerationStatus.ToString(), request.ModerationStatus);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = $"Transição de '{comment.ModerationStatus}' para '{request.ModerationStatus}' não é permitida."
                };
            }

            // Aplica a moderação
            var previousStatus = comment.ModerationStatus;
            comment.ModerationStatus = moderationStatus;
            comment.ModeratedById = currentUserId;
            comment.ModeratedAt = DateTime.UtcNow;
            comment.ModerationReason = request.ModerationReason;
            comment.UpdatedAt = DateTime.UtcNow;

            // Ações específicas por status
            await ApplyModerationActionsAsync(comment, moderationStatus, previousStatus, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            LogCommentModerated(_logger, comment.Id, previousStatus.ToString(), moderationStatus.ToString(), currentUserId);

            // Retorna o comentário moderado
            var moderatedComment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.ModeratedBy)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedEmployee)
                .FirstOrDefaultAsync(c => c.Id == comment.Id, cancellationToken);

            var commentDto = _mapper.Map<DiscussionCommentDto>(moderatedComment);

            return new CommentOperationResponse
            {
                Success = true,
                Message = GetModerationMessage(moderationStatus),
                Comment = commentDto
            };
        }
        catch (Exception ex)
        {
            LogErrorModeratingComment(_logger, ex, request.CommentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro interno do servidor."
            };
        }
    }

    /// Verifica se a transição de status de moderação é válida
    private static bool IsValidModerationTransition(ModerationStatus currentStatus, ModerationStatus newStatus)
    {
        // Qualquer status pode ir para UnderReview
        if (newStatus == ModerationStatus.UnderReview)
            return true;

        // De Pending pode ir para qualquer lugar
        if (currentStatus == ModerationStatus.Pending)
            return true;

        // De UnderReview pode ir para qualquer lugar exceto Pending
        if (currentStatus == ModerationStatus.UnderReview)
            return newStatus != ModerationStatus.Pending;

        // Approved pode ser revogado para Flagged, Hidden ou UnderReview
        if (currentStatus == ModerationStatus.Approved)
            return newStatus is ModerationStatus.Flagged or ModerationStatus.Hidden or ModerationStatus.UnderReview;

        // Flagged pode ir para Approved, Hidden ou Rejected
        if (currentStatus == ModerationStatus.Flagged)
            return newStatus is ModerationStatus.Approved or ModerationStatus.Hidden or ModerationStatus.Rejected;

        // Hidden pode ser restaurado para Approved ou escalado para Rejected
        if (currentStatus == ModerationStatus.Hidden)
            return newStatus is ModerationStatus.Approved or ModerationStatus.Rejected;

        // Rejected é final, mas pode ser revertido para UnderReview em casos especiais
        if (currentStatus == ModerationStatus.Rejected)
            return newStatus == ModerationStatus.UnderReview;

        return false;
    }

    /// Aplica ações específicas baseadas no status de moderação
    private async Task ApplyModerationActionsAsync(Comment comment, ModerationStatus newStatus, ModerationStatus previousStatus, CancellationToken cancellationToken)
    {
        switch (newStatus)
        {
            case ModerationStatus.Hidden:
                // Ocultar conteúdo mas manter estrutura da thread
                comment.Content = "[Comentário oculto pela moderação]";
                break;

            case ModerationStatus.Rejected:
                // Marcar como rejeitado e ocultar
                comment.Content = "[Comentário rejeitado pela moderação]";
                comment.IsFlagged = true;
                break;

            case ModerationStatus.Approved:
                // Se estava oculto/rejeitado, seria necessário restaurar conteúdo original
                // Por simplicidade, mantemos como está
                comment.IsFlagged = false;
                break;

            case ModerationStatus.Flagged:
                comment.IsFlagged = true;
                break;

            default:
                comment.IsFlagged = false;
                break;
        }

        // Criar notificação para o autor se necessário
        if (newStatus is ModerationStatus.Hidden or ModerationStatus.Rejected or ModerationStatus.Flagged)
        {
            await CreateModerationNotificationAsync(comment, newStatus, cancellationToken);
        }
    }

    /// Cria notificação para o autor sobre moderação
    private Task CreateModerationNotificationAsync(Comment comment, ModerationStatus status, CancellationToken cancellationToken)
    {
        var message = status switch
        {
            ModerationStatus.Hidden => "Seu comentário foi oculto pela moderação.",
            ModerationStatus.Rejected => "Seu comentário foi rejeitado pela moderação.",
            ModerationStatus.Flagged => "Seu comentário foi sinalizado para revisão.",
            _ => "Status do seu comentário foi alterado."
        };

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = comment.AuthorId,
            SenderId = comment.ModeratedById!.Value,
            Type = NotificationType.ModerationAction,
            Title = "Ação de Moderação",
            Message = message,
            RelatedEntityId = comment.Id,
            RelatedEntityType = "Comment",
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        return Task.CompletedTask;
    }

    /// Retorna mensagem apropriada para o status de moderação
    private static string GetModerationMessage(ModerationStatus status)
    {
        return status switch
        {
            ModerationStatus.Approved => "Comentário aprovado.",
            ModerationStatus.Flagged => "Comentário sinalizado para revisão.",
            ModerationStatus.Hidden => "Comentário oculto.",
            ModerationStatus.Rejected => "Comentário rejeitado.",
            ModerationStatus.UnderReview => "Comentário sob análise.",
            ModerationStatus.Pending => "Comentário pendente de moderação.",
            _ => "Status de moderação alterado."
        };
    }

    [LoggerMessage(EventId = 1501, Level = LogLevel.Information,
        Message = "Moderando comentário: {CommentId}, novo status: {Status}")]
    private static partial void LogModeratingComment(ILogger logger, Guid commentId, string status);

    [LoggerMessage(EventId = 1502, Level = LogLevel.Warning,
        Message = "Comentário não encontrado: {CommentId}")]
    private static partial void LogCommentNotFound(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1503, Level = LogLevel.Warning,
        Message = "Usuário {UserId} sem permissão para moderar comentário: {CommentId}")]
    private static partial void LogUnauthorizedModeration(ILogger logger, Guid commentId, Guid userId);

    [LoggerMessage(EventId = 1504, Level = LogLevel.Warning,
        Message = "Transição de moderação inválida para {CommentId}: {FromStatus} -> {ToStatus}")]
    private static partial void LogInvalidModerationTransition(ILogger logger, Guid commentId, string fromStatus, string toStatus);

    [LoggerMessage(EventId = 1505, Level = LogLevel.Information,
        Message = "Comentário moderado: {CommentId}, {FromStatus} -> {ToStatus}, por: {UserId}")]
    private static partial void LogCommentModerated(ILogger logger, Guid commentId, string fromStatus, string toStatus, Guid userId);

    [LoggerMessage(EventId = 1506, Level = LogLevel.Error,
        Message = "Erro ao moderar comentário: {CommentId}")]
    private static partial void LogErrorModeratingComment(ILogger logger, Exception ex, Guid commentId);
}