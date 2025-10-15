using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.DTOs;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para remover curtida de posts no feed
/// </summary>
public partial class UnlikePostHandler : IRequestHandler<UnlikePostCommand, PostLikeResponseDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UnlikePostHandler> _logger;

    public UnlikePostHandler(ISynQcoreDbContext context, ILogger<UnlikePostHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PostLikeResponseDto> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
    {
        LogUnlikingPost(_logger, request.PostId, request.UserId);

        try
        {
            // Verificar se o post existe
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);

            if (post == null)
            {
                LogPostNotFound(_logger, request.PostId);
                return new PostLikeResponseDto
                {
                    Success = false,
                    Message = "Post não encontrado",
                    IsLiked = false,
                    TotalLikes = 0
                };
            }

            // Buscar like existente
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == request.PostId && l.EmployeeId == request.UserId, cancellationToken);

            if (existingLike == null)
            {
                LogLikeNotFound(_logger, request.PostId, request.UserId);

                // Retornar contagem atual mesmo se não havia like
                var currentLikeCount = await _context.PostLikes
                    .CountAsync(l => l.PostId == request.PostId, cancellationToken);

                return new PostLikeResponseDto
                {
                    Success = true,
                    Message = "Post não estava curtido",
                    IsLiked = false,
                    TotalLikes = currentLikeCount
                };
            }

            // Remover like
            _context.PostLikes.Remove(existingLike);

            // Atualizar contador no post
            var likeCount = await _context.PostLikes
                .CountAsync(l => l.PostId == request.PostId && l.Id != existingLike.Id, cancellationToken);

            post.LikeCount = likeCount;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogLikeRemoved(_logger, request.PostId, request.UserId, likeCount);

            return new PostLikeResponseDto
            {
                Success = true,
                Message = "Curtida removida com sucesso",
                IsLiked = false,
                TotalLikes = likeCount
            };
        }
        catch (Exception ex)
        {
            LogErrorUnlikingPost(_logger, ex, request.PostId, request.UserId);
            return new PostLikeResponseDto
            {
                Success = false,
                Message = "Erro interno ao remover curtida",
                IsLiked = false,
                TotalLikes = 0
            };
        }
    }

    [LoggerMessage(EventId = 8101, Level = LogLevel.Information,
        Message = "Removendo curtida - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogUnlikingPost(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 8102, Level = LogLevel.Warning,
        Message = "Post não encontrado: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 8103, Level = LogLevel.Information,
        Message = "Like não encontrado - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogLikeNotFound(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 8104, Level = LogLevel.Information,
        Message = "Like removido com sucesso - PostId: {PostId}, UserId: {UserId}, TotalLikes: {TotalLikes}")]
    private static partial void LogLikeRemoved(ILogger logger, Guid postId, Guid userId, int totalLikes);

    [LoggerMessage(EventId = 8105, Level = LogLevel.Error,
        Message = "Erro ao remover curtida - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogErrorUnlikingPost(ILogger logger, Exception ex, Guid postId, Guid userId);
}
