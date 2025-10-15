using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.DTOs;
using SynQcore.Domain.Entities;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Feed.Handlers;

/// <summary>
/// Handler para obter status de curtida de um post
/// </summary>
public partial class GetPostLikeStatusHandler : IRequestHandler<GetPostLikeStatusQuery, PostLikeStatusDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetPostLikeStatusHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Obtendo status de curtidas do post {PostId} para usuário {UserId}")]
    private static partial void LogGettingLikeStatus(ILogger logger, Guid postId, Guid userId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Status de curtidas obtido - Post: {PostId}, HasLiked: {HasLiked}, TotalLikes: {TotalLikes}")]
    private static partial void LogLikeStatusObtained(ILogger logger, Guid postId, bool hasLiked, int totalLikes, Exception? exception);

    public GetPostLikeStatusHandler(ISynQcoreDbContext context, ILogger<GetPostLikeStatusHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PostLikeStatusDto> Handle(GetPostLikeStatusQuery request, CancellationToken cancellationToken)
    {
        LogGettingLikeStatus(_logger, request.PostId, request.UserId, null);

        // Verifica se o post existe
        var postExists = await _context.Posts
            .AnyAsync(p => p.Id == request.PostId, cancellationToken);

        if (!postExists)
        {
            throw new InvalidOperationException($"Post {request.PostId} não encontrado");
        }

        // Obtém total de likes do post
        var totalLikes = await _context.PostLikes
            .CountAsync(pl => pl.PostId == request.PostId, cancellationToken);

        // Verifica se o usuário curtiu o post
        var userLike = await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == request.PostId && pl.EmployeeId == request.UserId, cancellationToken);

        var hasLiked = userLike != null;
        var userReactionType = userLike?.ReactionType.ToString();
        var likedAt = userLike?.LikedAt;

        // Obtém contadores por tipo de reação
        var reactionCounts = await _context.PostLikes
            .Where(pl => pl.PostId == request.PostId)
            .GroupBy(pl => pl.ReactionType)
            .Select(g => new { ReactionType = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = new PostLikeStatusDto
        {
            PostId = request.PostId,
            IsLiked = hasLiked,
            ReactionType = userReactionType,
            LikedAt = likedAt,
            TotalLikes = totalLikes
        };

        LogLikeStatusObtained(_logger, request.PostId, hasLiked, totalLikes, null);

        return result;
    }
}

/// <summary>
/// Handler para obter lista de curtidas de um post
/// </summary>
public partial class GetPostLikesHandler : IRequestHandler<GetPostLikesQuery, PagedResult<PostLikeDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetPostLikesHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Obtendo curtidas do post {PostId} - Página: {Page}, Tamanho: {PageSize}, Tipo: {ReactionType}")]
    private static partial void LogGettingPostLikes(ILogger logger, Guid postId, int page, int pageSize, string? reactionType, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Curtidas obtidas - Post: {PostId}, Total: {Total}, Retornados: {Count}")]
    private static partial void LogPostLikesObtained(ILogger logger, Guid postId, int total, int count, Exception? exception);

    public GetPostLikesHandler(ISynQcoreDbContext context, ILogger<GetPostLikesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<PostLikeDto>> Handle(GetPostLikesQuery request, CancellationToken cancellationToken)
    {
        LogGettingPostLikes(_logger, request.PostId, request.Page, request.PageSize, request.ReactionType, null);

        // Verifica se o post existe
        var postExists = await _context.Posts
            .AnyAsync(p => p.Id == request.PostId, cancellationToken);

        if (!postExists)
        {
            throw new InvalidOperationException($"Post {request.PostId} não encontrado");
        }

        var query = _context.PostLikes
            .Include(pl => pl.Employee)
            .Where(pl => pl.PostId == request.PostId);

        // Filtro por tipo de reação se especificado
        if (!string.IsNullOrEmpty(request.ReactionType))
        {
            if (Enum.TryParse<ReactionType>(request.ReactionType, true, out var reactionType))
            {
                query = query.Where(pl => pl.ReactionType == reactionType);
            }
        }

        // Ordenação por data de criação (mais recentes primeiro)
        query = query.OrderByDescending(pl => pl.LikedAt);

        // Paginação
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pl => new PostLikeDto
            {
                Id = pl.Id,
                PostId = pl.PostId,
                EmployeeId = pl.EmployeeId,
                EmployeeName = pl.Employee.FullName ?? "Usuário",
                EmployeeAvatar = null, // TODO: Implementar avatars
                EmployeeJobTitle = pl.Employee.JobTitle,
                ReactionType = pl.ReactionType.ToString(),
                LikedAt = pl.LikedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<PostLikeDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = request.PageSize
        };

        LogPostLikesObtained(_logger, request.PostId, totalCount, items.Count, null);

        return result;
    }
}
