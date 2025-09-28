using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Application.Common.Extensions;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

/// <summary>
/// Handler para atualização de comentários em discussion threads.
/// Gerencia edições de conteúdo e análise de mudanças significativas.
/// </summary>
public partial class UpdateDiscussionCommentCommandHandler : IRequestHandler<UpdateDiscussionCommentCommand, CommentOperationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateDiscussionCommentCommandHandler> _logger;

    /// <summary>
    /// Inicializa nova instância do handler de atualização de comentários.
    /// </summary>
    /// <param name="context">Contexto de acesso a dados.</param>
    /// <param name="currentUserService">Serviço de usuário atual.</param>
    /// <param name="logger">Logger para rastreamento de operações.</param>
    public UpdateDiscussionCommentCommandHandler(
        ISynQcoreDbContext context,

        ICurrentUserService currentUserService,
        ILogger<UpdateDiscussionCommentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Processa atualização de comentário com validações de autorização.
    /// </summary>
    /// <param name="request">Command contendo ID e novos dados do comentário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação com comentário atualizado.</returns>
    public async Task<CommentOperationResponse> Handle(UpdateDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingComment(_logger, request.CommentId);

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

            // Verifica permissões - só o autor pode editar
            if (comment.AuthorId != currentUserId)
            {
                LogUnauthorizedUpdate(_logger, request.CommentId, currentUserId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Você não tem permissão para editar este comentário."
                };
            }

            // Verifica se o comentário já foi moderado (não pode editar se rejeitado/oculto)
            if (comment.ModerationStatus is ModerationStatus.Rejected or ModerationStatus.Hidden)
            {
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Não é possível editar comentário moderado."
                };
            }

            // Salva estado anterior para auditoria
            var originalContent = comment.Content;

            // Converte enums
            var commentType = Enum.Parse<CommentType>(request.Type, true);
            var visibility = Enum.Parse<CommentVisibility>(request.Visibility, true);
            var priority = Enum.Parse<CommentPriority>(request.Priority, true);

            // Atualiza propriedades
            comment.Content = request.Content;
            comment.Type = commentType;
            comment.Visibility = visibility;
            comment.IsConfidential = request.IsConfidential;
            comment.Priority = priority;
            comment.IsEdited = true;
            comment.EditedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            // Se conteúdo foi significativamente alterado, pode precisar re-moderação
            if (HasSignificantContentChange(originalContent, request.Content))
            {
                comment.ModerationStatus = ModerationStatus.Pending;
                LogContentChangeRequiresModeration(_logger, request.CommentId);
            }

            await _context.SaveChangesAsync(cancellationToken);

            LogCommentUpdated(_logger, comment.Id, comment.Type.ToString());

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
                Message = "Comentário atualizado com sucesso.",
                Comment = commentDto
            };
        }
        catch (Exception ex)
        {
            LogErrorUpdatingComment(_logger, ex, request.CommentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro interno do servidor."
            };
        }
    }

    /// Verifica se a mudança no conteúdo é significativa para requerer nova moderação
    private static bool HasSignificantContentChange(string originalContent, string newContent)
    {
        if (string.IsNullOrWhiteSpace(originalContent) || string.IsNullOrWhiteSpace(newContent))
            return true;

        // Considera significativa se mudança > 30% do conteúdo original
        var originalWords = originalContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var newWords = newContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var similarity = CalculateStringSimilarity(originalWords, newWords);
        return similarity < 0.7; // Menos que 70% de similaridade
    }

    /// Calcula similaridade simples entre arrays de palavras
    private static double CalculateStringSimilarity(string[] words1, string[] words2)
    {
        if (words1.Length == 0 && words2.Length == 0) return 1.0;
        if (words1.Length == 0 || words2.Length == 0) return 0.0;

        var commonWords = words1.Intersect(words2).Count();
        var totalWords = Math.Max(words1.Length, words2.Length);

        return (double)commonWords / totalWords;
    }

    [LoggerMessage(EventId = 1201, Level = LogLevel.Information,
        Message = "Atualizando comentário: {CommentId}")]
    private static partial void LogUpdatingComment(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1202, Level = LogLevel.Warning,
        Message = "Comentário não encontrado: {CommentId}")]
    private static partial void LogCommentNotFound(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1203, Level = LogLevel.Warning,
        Message = "Usuário {UserId} sem permissão para editar comentário: {CommentId}")]
    private static partial void LogUnauthorizedUpdate(ILogger logger, Guid commentId, Guid userId);

    [LoggerMessage(EventId = 1204, Level = LogLevel.Information,
        Message = "Mudança significativa no conteúdo requer moderação: {CommentId}")]
    private static partial void LogContentChangeRequiresModeration(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 1205, Level = LogLevel.Information,
        Message = "Comentário atualizado: {CommentId}, tipo: {Type}")]
    private static partial void LogCommentUpdated(ILogger logger, Guid commentId, string type);

    [LoggerMessage(EventId = 1206, Level = LogLevel.Error,
        Message = "Erro ao atualizar comentário: {CommentId}")]
    private static partial void LogErrorUpdatingComment(ILogger logger, Exception ex, Guid commentId);
}
