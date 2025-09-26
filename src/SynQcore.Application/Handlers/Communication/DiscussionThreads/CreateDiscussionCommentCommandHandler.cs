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

/// Handler para criação de comentários em discussion threads corporativas
public partial class CreateDiscussionCommentCommandHandler : IRequestHandler<CreateDiscussionCommentCommand, CommentOperationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly DiscussionThreadHelper _threadHelper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateDiscussionCommentCommandHandler> _logger;

    public CreateDiscussionCommentCommandHandler(
        ISynQcoreDbContext context,
        IMapper mapper,
        DiscussionThreadHelper threadHelper,
        ICurrentUserService currentUserService,
        ILogger<CreateDiscussionCommentCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _threadHelper = threadHelper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CommentOperationResponse> Handle(CreateDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        LogCreatingComment(_logger, request.PostId, request.Type);

        try
        {
            // Verifica se o post existe
            var postExists = await _context.Posts
                .AnyAsync(p => p.Id == request.PostId, cancellationToken);

            if (!postExists)
            {
                LogPostNotFound(_logger, request.PostId);
                return new CommentOperationResponse
                {
                    Success = false,
                    Message = "Post não encontrado."
                };
            }

            // Verifica se o comentário pai existe (se especificado)
            if (request.ParentCommentId.HasValue)
            {
                var parentExists = await _context.Comments
                    .AnyAsync(c => c.Id == request.ParentCommentId.Value, cancellationToken);

                if (!parentExists)
                {
                    return new CommentOperationResponse
                    {
                        Success = false,
                        Message = "Comentário pai não encontrado."
                    };
                }
            }

            var currentUserId = _currentUserService.UserId;

            // Calcula posição na thread
            var (threadLevel, threadPath) = await _threadHelper
                .CalculateThreadPositionAsync(request.ParentCommentId, cancellationToken);

            // Converte enums
            var commentType = Enum.Parse<CommentType>(request.Type, true);
            var visibility = Enum.Parse<CommentVisibility>(request.Visibility, true);
            var priority = Enum.Parse<CommentPriority>(request.Priority, true);

            // Cria o comentário
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                PostId = request.PostId,
                AuthorId = currentUserId,
                ParentCommentId = request.ParentCommentId,
                Type = commentType,
                Visibility = visibility,
                IsConfidential = request.IsConfidential,
                Priority = priority,
                ThreadLevel = threadLevel,
                ThreadPath = threadPath,
                ModerationStatus = ModerationStatus.Approved, // Auto-aprovado por padrão
                LastActivityAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            // Processa menções se fornecidas
            if (request.Mentions?.Count > 0)
            {
                foreach (var mentionDto in request.Mentions)
                {
                    var mentionContext = Enum.Parse<MentionContext>(mentionDto.Context, true);
                    var mentionUrgency = Enum.Parse<MentionUrgency>(mentionDto.Urgency, true);

                    var mention = new CommentMention
                    {
                        Id = Guid.NewGuid(),
                        CommentId = comment.Id,
                        MentionedEmployeeId = mentionDto.MentionedEmployeeId,
                        MentionedById = currentUserId,
                        MentionText = mentionDto.MentionText,
                        StartPosition = mentionDto.StartPosition,
                        Length = mentionDto.Length,
                        Context = mentionContext,
                        Urgency = mentionUrgency,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.CommentMentions.Add(mention);
                }
            }

            // Atualiza contadores se é um reply
            if (request.ParentCommentId.HasValue)
            {
                await _threadHelper.UpdateReplyCountsAsync(request.ParentCommentId.Value, 1, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            LogCommentCreated(_logger, comment.Id, comment.Type.ToString());

            // Retorna o comentário criado
            var createdComment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedEmployee)
                .FirstOrDefaultAsync(c => c.Id == comment.Id, cancellationToken);

            var commentDto = _mapper.Map<DiscussionCommentDto>(createdComment);

            return new CommentOperationResponse
            {
                Success = true,
                Message = "Comentário criado com sucesso.",
                Comment = commentDto
            };
        }
        catch (Exception ex)
        {
            LogErrorCreatingComment(_logger, ex, request.PostId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro interno do servidor."
            };
        }
    }

    [LoggerMessage(EventId = 1101, Level = LogLevel.Information,
        Message = "Criando comentário no post: {PostId}, tipo: {Type}")]
    private static partial void LogCreatingComment(ILogger logger, Guid postId, string type);

    [LoggerMessage(EventId = 1102, Level = LogLevel.Warning,
        Message = "Post não encontrado: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 1103, Level = LogLevel.Information,
        Message = "Comentário criado: {CommentId}, tipo: {Type}")]
    private static partial void LogCommentCreated(ILogger logger, Guid commentId, string type);

    [LoggerMessage(EventId = 1104, Level = LogLevel.Error,
        Message = "Erro ao criar comentário no post: {PostId}")]
    private static partial void LogErrorCreatingComment(ILogger logger, Exception ex, Guid postId);
}