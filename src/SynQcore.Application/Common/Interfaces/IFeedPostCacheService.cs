using SynQcore.Application.Features.Feed.DTOs;

namespace SynQcore.Application.Common.Interfaces;

/// <summary>
/// Interface para o serviço de cache de posts do feed
/// </summary>
public interface IFeedPostCacheService
{
    /// <summary>
    /// Obtém um post do cache
    /// </summary>
    Task<FeedPostDto?> GetPostAsync(Guid postId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Armazena um post no cache
    /// </summary>
    Task SetPostAsync(FeedPostDto post, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um post do cache
    /// </summary>
    Task RemovePostAsync(Guid postId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém posts populares do cache
    /// </summary>
    Task<List<FeedPostDto>?> GetPopularPostsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Armazena posts populares no cache
    /// </summary>
    Task SetPopularPostsAsync(List<FeedPostDto> posts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém feed personalizado do usuário do cache
    /// </summary>
    Task<List<FeedPostDto>?> GetUserFeedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Armazena feed personalizado do usuário no cache
    /// </summary>
    Task SetUserFeedAsync(Guid userId, int page, int pageSize, List<FeedPostDto> posts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalida o cache do feed de um usuário
    /// </summary>
    Task InvalidateUserFeedAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalida todos os caches relacionados a posts
    /// </summary>
    Task InvalidateAllPostCachesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Incrementa o contador de visualizações no cache
    /// </summary>
    Task IncrementViewCountAsync(Guid postId, CancellationToken cancellationToken = default);
}