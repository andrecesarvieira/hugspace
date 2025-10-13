using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para curtir posts no feed
/// </summary>
public partial class LikePostHandler : IRequestHandler<LikePostCommand, PostLikeResponseDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<LikePostHandler> _logger;

    public LikePostHandler(ISynQcoreDbContext context, ILogger<LikePostHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PostLikeResponseDto> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        LogLikingPost(_logger, request.PostId, request.UserId, request.ReactionType);

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

            // Verificar se o usuário existe
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.UserId && !e.IsDeleted, cancellationToken);

            if (employee == null)
            {
                LogEmployeeNotFound(_logger, request.UserId);
                return new PostLikeResponseDto
                {
                    Success = false,
                    Message = "Usuário não encontrado",
                    IsLiked = false,
                    TotalLikes = 0
                };
            }

            // Verificar se já existe like
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == request.PostId && l.EmployeeId == request.UserId, cancellationToken);

            if (existingLike != null)
            {
                LogLikeAlreadyExists(_logger, request.PostId, request.UserId);
                
                // Se é o mesmo tipo de reação, retorna sucesso
                if (existingLike.ReactionType.ToString().Equals(request.ReactionType, StringComparison.OrdinalIgnoreCase))
                {
                    var currentLikeCount = await _context.PostLikes
                        .CountAsync(l => l.PostId == request.PostId, cancellationToken);

                    return new PostLikeResponseDto
                    {
                        Success = true,
                        Message = "Post já curtido",
                        IsLiked = true,
                        TotalLikes = currentLikeCount,
                        ReactionType = existingLike.ReactionType.ToString(),
                        LikedAt = existingLike.LikedAt
                    };
                }

                // Atualizar tipo de reação
                existingLike.ReactionType = Enum.Parse<ReactionType>(request.ReactionType, true);
                existingLike.LikedAt = DateTime.UtcNow;
                
                LogLikeUpdated(_logger, request.PostId, request.UserId, request.ReactionType);
            }
            else
            {
                // Criar novo like
                var newLike = new PostLike
                {
                    Id = Guid.NewGuid(),
                    PostId = request.PostId,
                    EmployeeId = request.UserId,
                    ReactionType = Enum.Parse<ReactionType>(request.ReactionType, true),
                    LikedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.PostLikes.Add(newLike);
                existingLike = newLike;
                
                LogLikeCreated(_logger, request.PostId, request.UserId, request.ReactionType);
            }

            // Atualizar contador no post
            var likeCount = await _context.PostLikes
                .CountAsync(l => l.PostId == request.PostId, cancellationToken);
            
            post.LikeCount = likeCount;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            LogLikeProcessed(_logger, request.PostId, request.UserId, likeCount);

            return new PostLikeResponseDto
            {
                Success = true,
                Message = "Post curtido com sucesso",
                IsLiked = true,
                TotalLikes = likeCount,
                ReactionType = existingLike.ReactionType.ToString(),
                LikedAt = existingLike.LikedAt
            };
        }
        catch (Exception ex)
        {
            LogErrorLikingPost(_logger, ex, request.PostId, request.UserId);
            return new PostLikeResponseDto
            {
                Success = false,
                Message = "Erro interno ao curtir post",
                IsLiked = false,
                TotalLikes = 0
            };
        }
    }

    [LoggerMessage(EventId = 8001, Level = LogLevel.Information,
        Message = "Curtindo post - PostId: {PostId}, UserId: {UserId}, ReactionType: {ReactionType}")]
    private static partial void LogLikingPost(ILogger logger, Guid postId, Guid userId, string reactionType);

    [LoggerMessage(EventId = 8002, Level = LogLevel.Warning,
        Message = "Post não encontrado: {PostId}")]
    private static partial void LogPostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 8003, Level = LogLevel.Warning,
        Message = "Funcionário não encontrado: {UserId}")]
    private static partial void LogEmployeeNotFound(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 8004, Level = LogLevel.Information,
        Message = "Like já existe - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogLikeAlreadyExists(ILogger logger, Guid postId, Guid userId);

    [LoggerMessage(EventId = 8005, Level = LogLevel.Information,
        Message = "Like atualizado - PostId: {PostId}, UserId: {UserId}, NewReactionType: {ReactionType}")]
    private static partial void LogLikeUpdated(ILogger logger, Guid postId, Guid userId, string reactionType);

    [LoggerMessage(EventId = 8006, Level = LogLevel.Information,
        Message = "Like criado - PostId: {PostId}, UserId: {UserId}, ReactionType: {ReactionType}")]
    private static partial void LogLikeCreated(ILogger logger, Guid postId, Guid userId, string reactionType);

    [LoggerMessage(EventId = 8007, Level = LogLevel.Information,
        Message = "Like processado com sucesso - PostId: {PostId}, UserId: {UserId}, TotalLikes: {TotalLikes}")]
    private static partial void LogLikeProcessed(ILogger logger, Guid postId, Guid userId, int totalLikes);

    [LoggerMessage(EventId = 8008, Level = LogLevel.Error,
        Message = "Erro ao curtir post - PostId: {PostId}, UserId: {UserId}")]
    private static partial void LogErrorLikingPost(ILogger logger, Exception ex, Guid postId, Guid userId);
}