using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Features.Feed.DTOs;
using SynQcore.Application.Common.Interfaces;
using System.Text.Json;

namespace SynQcore.Infrastructure.Services;

/// <summary>
/// Serviço de cache Redis para posts do feed
/// </summary>
public partial class FeedPostCacheService : IFeedPostCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<FeedPostCacheService> _logger;
    
    private const int CacheExpirationInMinutes = 30;
    private const string PostKeyPrefix = "feed_post:";
    private const string PopularPostsKey = "feed:popular_posts";
    private const string UserFeedKey = "feed:user:";

    // LoggerMessage delegates para performance
    [LoggerMessage(LogLevel.Information, "Cache hit para post {PostId}")]
    private static partial void LogCacheHit(ILogger logger, Guid postId);

    [LoggerMessage(LogLevel.Information, "Cache miss para post {PostId}")]
    private static partial void LogCacheMiss(ILogger logger, Guid postId);

    [LoggerMessage(LogLevel.Information, "Invalidando cache para post {PostId}")]
    private static partial void LogCacheInvalidation(ILogger logger, Guid postId);

    [LoggerMessage(LogLevel.Warning, "Erro ao acessar cache para post {PostId}: {Error}")]
    private static partial void LogCacheError(ILogger logger, Guid postId, string error, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Erro ao obter posts populares do cache: {Error}")]
    private static partial void LogPopularPostsCacheError(ILogger logger, string error, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Erro ao armazenar posts populares no cache: {Error}")]
    private static partial void LogPopularPostsSetError(ILogger logger, string error, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Erro ao obter feed do usuário {UserId} do cache: {Error}")]
    private static partial void LogUserFeedCacheError(ILogger logger, Guid userId, string error, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Erro ao armazenar feed do usuário {UserId} no cache: {Error}")]
    private static partial void LogUserFeedSetError(ILogger logger, Guid userId, string error, Exception exception);

    [LoggerMessage(LogLevel.Information, "Cache do feed do usuário {UserId} invalidado")]
    private static partial void LogUserFeedInvalidated(ILogger logger, Guid userId);

    [LoggerMessage(LogLevel.Warning, "Erro ao invalidar cache do feed do usuário {UserId}: {Error}")]
    private static partial void LogUserFeedInvalidationError(ILogger logger, Guid userId, string error, Exception exception);

    [LoggerMessage(LogLevel.Information, "Cache de posts populares invalidado")]
    private static partial void LogPopularPostsInvalidated(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Erro ao invalidar caches de posts: {Error}")]
    private static partial void LogCacheInvalidationError(ILogger logger, string error, Exception exception);

    public FeedPostCacheService(IDistributedCache distributedCache, ILogger<FeedPostCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um post do cache
    /// </summary>
    public async Task<FeedPostDto?> GetPostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = GetPostKey(postId);
            var cachedPost = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (cachedPost != null)
            {
                LogCacheHit(_logger, postId);
                return JsonSerializer.Deserialize<FeedPostDto>(cachedPost);
            }

            LogCacheMiss(_logger, postId);
            return null;
        }
        catch (Exception ex)
        {
            LogCacheError(_logger, postId, ex.Message, ex);
            return null;
        }
    }

    /// <summary>
    /// Armazena um post no cache
    /// </summary>
    public async Task SetPostAsync(FeedPostDto post, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = GetPostKey(post.Id);
            var serializedPost = JsonSerializer.Serialize(post);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationInMinutes)
            };

            await _distributedCache.SetStringAsync(key, serializedPost, options, cancellationToken);
        }
        catch (Exception ex)
        {
            LogCacheError(_logger, post.Id, ex.Message, ex);
        }
    }

    /// <summary>
    /// Remove um post do cache
    /// </summary>
    public async Task RemovePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogCacheInvalidation(_logger, postId);
            var key = GetPostKey(postId);
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            LogCacheError(_logger, postId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Obtém posts populares do cache
    /// </summary>
    public async Task<List<FeedPostDto>?> GetPopularPostsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedPosts = await _distributedCache.GetStringAsync(PopularPostsKey, cancellationToken);

            if (cachedPosts != null)
            {
                return JsonSerializer.Deserialize<List<FeedPostDto>>(cachedPosts);
            }

            return null;
        }
        catch (Exception ex)
        {
            LogPopularPostsCacheError(_logger, ex.Message, ex);
            return null;
        }
    }

    /// <summary>
    /// Armazena posts populares no cache
    /// </summary>
    public async Task SetPopularPostsAsync(List<FeedPostDto> posts, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedPosts = JsonSerializer.Serialize(posts);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) // Posts populares expiram mais rápido
            };

            await _distributedCache.SetStringAsync(PopularPostsKey, serializedPosts, options, cancellationToken);
        }
        catch (Exception ex)
        {
            LogPopularPostsSetError(_logger, ex.Message, ex);
        }
    }

    /// <summary>
    /// Obtém feed personalizado do usuário do cache
    /// </summary>
    public async Task<List<FeedPostDto>?> GetUserFeedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = GetUserFeedKey(userId, page, pageSize);
            var cachedFeed = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (cachedFeed != null)
            {
                return JsonSerializer.Deserialize<List<FeedPostDto>>(cachedFeed);
            }

            return null;
        }
        catch (Exception ex)
        {
            LogUserFeedCacheError(_logger, userId, ex.Message, ex);
            return null;
        }
    }

    /// <summary>
    /// Armazena feed personalizado do usuário no cache
    /// </summary>
    public async Task SetUserFeedAsync(Guid userId, int page, int pageSize, List<FeedPostDto> posts, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = GetUserFeedKey(userId, page, pageSize);
            var serializedFeed = JsonSerializer.Serialize(posts);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Feed personalizado expira rapidamente
            };

            await _distributedCache.SetStringAsync(key, serializedFeed, options, cancellationToken);
        }
        catch (Exception ex)
        {
            LogUserFeedSetError(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Invalida o cache do feed de um usuário
    /// </summary>
    public async Task InvalidateUserFeedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Remove todas as páginas do feed do usuário
            var pattern = $"{UserFeedKey}{userId}:*";
            
            // Para uma implementação mais robusta, seria necessário usar Redis SCAN
            // Por ora, removemos apenas as primeiras páginas mais comuns
            var commonPages = new[] { 1, 2, 3, 4, 5 };
            var commonPageSizes = new[] { 10, 20, 50 };

            var tasks = new List<Task>();
            
            foreach (var page in commonPages)
            {
                foreach (var pageSize in commonPageSizes)
                {
                    var key = GetUserFeedKey(userId, page, pageSize);
                    tasks.Add(_distributedCache.RemoveAsync(key, cancellationToken));
                }
            }

            await Task.WhenAll(tasks);
            
            LogUserFeedInvalidated(_logger, userId);
        }
        catch (Exception ex)
        {
            LogUserFeedInvalidationError(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Invalida todos os caches relacionados a posts
    /// </summary>
    public async Task InvalidateAllPostCachesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(PopularPostsKey, cancellationToken);
            LogPopularPostsInvalidated(_logger);
        }
        catch (Exception ex)
        {
            LogCacheInvalidationError(_logger, ex.Message, ex);
        }
    }

    /// <summary>
    /// Incrementa o contador de visualizações no cache
    /// </summary>
    public async Task IncrementViewCountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            var post = await GetPostAsync(postId, cancellationToken);
            if (post != null)
            {
                // Criar novo DTO com ViewCount incrementado
                var updatedPost = post with { ViewCount = post.ViewCount + 1 };
                await SetPostAsync(updatedPost, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            LogCacheError(_logger, postId, ex.Message, ex);
        }
    }

    private static string GetPostKey(Guid postId) => $"{PostKeyPrefix}{postId}";
    
    private static string GetUserFeedKey(Guid userId, int page, int pageSize) => 
        $"{UserFeedKey}{userId}:{page}:{pageSize}";
}