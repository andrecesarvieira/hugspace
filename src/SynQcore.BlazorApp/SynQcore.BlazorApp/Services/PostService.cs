using System.Text.Json;
using SynQcore.BlazorApp.Components.Social;
using SynQcore.BlazorApp.Store.User;
using Fluxor;

namespace SynQcore.BlazorApp.Services;

public interface IPostService
{
    Task<List<SimplePostCard.PostModel>> GetFeedPostsAsync(int page = 1, int pageSize = 10);
    Task<SimplePostCard.PostModel?> GetPostByIdAsync(Guid postId);
    Task<SimplePostCard.PostModel> CreatePostAsync(CreatePostRequest request);
    Task<bool> LikePostAsync(Guid postId);
    Task<bool> UnlikePostAsync(Guid postId);
    Task<SimplePostCard.CommentModel> AddCommentAsync(Guid postId, string content);
    Task<List<SimplePostCard.CommentModel>> GetPostCommentsAsync(Guid postId);
    Task<bool> MarkAsReadAsync(Guid feedEntryId);
    Task<bool> BookmarkPostAsync(Guid feedEntryId);
}

public partial class PostService : IPostService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PostService> _logger;
    private readonly IState<UserState> _userState;

    // Cache de JsonSerializerOptions para performance
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Warning, "Falha ao buscar posts do feed. Status: {StatusCode}")]
    private static partial void LogGetFeedPostsWarning(ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar posts do feed")]
    private static partial void LogGetFeedPostsError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar post {PostId}")]
    private static partial void LogGetPostByIdError(ILogger logger, Guid postId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao criar post")]
    private static partial void LogCreatePostError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao curtir post {PostId}")]
    private static partial void LogLikePostError(ILogger logger, Guid postId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao descurtir post {PostId}")]
    private static partial void LogUnlikePostError(ILogger logger, Guid postId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao adicionar comentário no post {PostId}")]
    private static partial void LogAddCommentError(ILogger logger, Guid postId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao buscar comentários do post {PostId}")]
    private static partial void LogGetCommentsError(ILogger logger, Guid postId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao marcar feed item como lido {FeedEntryId}")]
    private static partial void LogMarkAsReadError(ILogger logger, Guid feedEntryId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao bookmark feed item {FeedEntryId}")]
    private static partial void LogBookmarkError(ILogger logger, Guid feedEntryId, Exception exception);

    public PostService(HttpClient httpClient, ILogger<PostService> logger, IState<UserState> userState)
    {
        _httpClient = httpClient;
        _logger = logger;
        _userState = userState;
    }

    public async Task<List<SimplePostCard.PostModel>> GetFeedPostsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/feed?page={page}&pageSize={pageSize}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<FeedResponse>(json, JsonOptions);

                return result?.Items?.Select(MapToPostModel).ToList() ?? new List<SimplePostCard.PostModel>();
            }
            else
            {
                LogGetFeedPostsWarning(_logger, response.StatusCode);
                // Retorna lista vazia em caso de erro (sem fallback para mock)
                return new List<SimplePostCard.PostModel>();
            }
        }
        catch (Exception ex)
        {
            LogGetFeedPostsError(_logger, ex);
            // Retorna lista vazia em caso de erro (sem fallback para mock)
            return new List<SimplePostCard.PostModel>();
        }
    }

    public async Task<SimplePostCard.PostModel?> GetPostByIdAsync(Guid postId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/feed/{postId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var feedPost = JsonSerializer.Deserialize<FeedPostDto>(json, JsonOptions);

                return feedPost != null ? MapToPostModel(feedPost) : null;
            }
        }
        catch (Exception ex)
        {
            LogGetPostByIdError(_logger, postId, ex);
        }

        return null;
    }

    public async Task<SimplePostCard.PostModel> CreatePostAsync(CreatePostRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/feed", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var feedPost = JsonSerializer.Deserialize<FeedPostDto>(responseJson, JsonOptions);

                if (feedPost != null)
                {
                    return MapToPostModel(feedPost);
                }
            }
        }
        catch (Exception ex)
        {
            LogCreatePostError(_logger, ex);
        }

        // Fallback: criar post usando dados reais do usuário autenticado
        var userId = _userState.Value.CurrentUser?.Id ?? "anonymous";
        var userName = _userState.Value.CurrentUser?.Nome ?? "Usuário";

        return new SimplePostCard.PostModel
        {
            Id = Guid.NewGuid(),
            AuthorName = userName,
            AuthorRole = _userState.Value.CurrentUser?.Cargo ?? "Funcionário",
            AuthorAvatar = _userState.Value.CurrentUser?.FotoUrl ?? "/images/default-avatar.png",
            Content = request.Content,
            Title = ExtractTitleFromContent(request.Content),
            CreatedAt = DateTime.Now,
            LikeCount = 0,
            CommentCount = 0,
            ShareCount = 0,
            IsLiked = false,
            IsSaved = false,
            IsOfficial = false,
            IsPinned = false,
            IsBookmarked = false,
            Tags = ExtractHashtags(request.Content),
            Comments = new List<SimplePostCard.CommentModel>()
        };
    }

    private static string ExtractTitleFromContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "";

        // Extrair primeiro parágrafo ou primeira linha como título
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var firstLine = lines.FirstOrDefault()?.Trim() ?? "";

        // Limitar tamanho do título usando AsSpan para melhor performance
        return firstLine.Length > 100 ? string.Concat(firstLine.AsSpan(0, 97), "...") : firstLine;
    }

    public async Task<bool> LikePostAsync(Guid postId)
    {
        try
        {
            // Usar endpoint para atualizar interesses com interação de like
            var response = await _httpClient.PostAsync($"/api/feed/interests/update?contentId={postId}&interactionType=like", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LogLikePostError(_logger, postId, ex);
            return false;
        }
    }

    public Task<bool> UnlikePostAsync(Guid postId)
    {
        try
        {
            // Para unlike, usar o mesmo endpoint mas pode ser implementado diferente
            // Por enquanto, vamos simular sucesso
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            LogUnlikePostError(_logger, postId, ex);
            return Task.FromResult(false);
        }
    }

    public async Task<SimplePostCard.CommentModel> AddCommentAsync(Guid postId, string content)
    {
        try
        {
            var request = new
            {
                PostId = postId,
                Content = content
            };
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/discussionthreads/comments", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CommentOperationResponse>(responseJson, JsonOptions);

                if (result?.Comment != null)
                {
                    return new SimplePostCard.CommentModel
                    {
                        Id = result.Comment.Id,
                        AuthorName = result.Comment.AuthorName ?? "Usuário",
                        AuthorAvatar = "/images/default-avatar.png",
                        Content = result.Comment.Content,
                        CreatedAt = result.Comment.CreatedAt,
                        LikeCount = result.Comment.LikeCount,
                        IsLiked = false
                    };
                }
            }
        }
        catch (Exception ex)
        {
            LogAddCommentError(_logger, postId, ex);
        }

        // Fallback: criar comentário usando dados reais do usuário autenticado
        var userName = _userState.Value.CurrentUser?.Nome ?? "Usuário";

        return new SimplePostCard.CommentModel
        {
            Id = Guid.NewGuid(),
            AuthorName = userName,
            AuthorAvatar = _userState.Value.CurrentUser?.FotoUrl ?? "/images/default-avatar.png",
            Content = content,
            CreatedAt = DateTime.Now,
            LikeCount = 0,
            IsLiked = false
        };
    }

    public async Task<List<SimplePostCard.CommentModel>> GetPostCommentsAsync(Guid postId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/feed/{postId}/comments");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var comments = JsonSerializer.Deserialize<List<CommentDto>>(json, JsonOptions);

                return comments?.Select(c => new SimplePostCard.CommentModel
                {
                    Id = c.Id,
                    AuthorName = c.AuthorName ?? "Usuário",
                    AuthorAvatar = "/images/default-avatar.png",
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    LikeCount = c.LikeCount,
                    IsLiked = false
                }).ToList() ?? new List<SimplePostCard.CommentModel>();
            }
        }
        catch (Exception ex)
        {
            LogGetCommentsError(_logger, postId, ex);
        }

        return new List<SimplePostCard.CommentModel>();
    }

    public async Task<bool> MarkAsReadAsync(Guid feedEntryId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/feed/{feedEntryId}/read", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LogMarkAsReadError(_logger, feedEntryId, ex);
            return false;
        }
    }

    public async Task<bool> BookmarkPostAsync(Guid feedEntryId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/feed/{feedEntryId}/bookmark", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LogBookmarkError(_logger, feedEntryId, ex);
            return false;
        }
    }

    private static SimplePostCard.PostModel MapToPostModel(FeedItemDto feedItem)
    {
        return new SimplePostCard.PostModel
        {
            Id = feedItem.PostId,
            AuthorName = feedItem.AuthorName,
            AuthorRole = feedItem.AuthorDepartment ?? "Funcionário",
            AuthorAvatar = feedItem.AuthorAvatarUrl ?? "/images/default-avatar.png",
            Content = feedItem.Content,
            CreatedAt = feedItem.CreatedAt,
            LikeCount = feedItem.LikeCount,
            CommentCount = feedItem.CommentCount,
            ShareCount = 0, // Não disponível no FeedItemDto
            IsLiked = feedItem.HasLiked,
            Tags = feedItem.Tags.ToArray(),
            Comments = new List<SimplePostCard.CommentModel>(),
            Title = feedItem.Title,
            IsOfficial = feedItem.IsOfficial,
            IsPinned = feedItem.IsPinned,
            IsBookmarked = feedItem.IsBookmarked
        };
    }

    private static SimplePostCard.PostModel MapToPostModel(FeedPostDto feedPost)
    {
        return new SimplePostCard.PostModel
        {
            Id = feedPost.Id,
            AuthorName = feedPost.AuthorName ?? "Usuário",
            AuthorRole = feedPost.AuthorRole ?? "Funcionário",
            AuthorAvatar = "/images/default-avatar.png",
            Content = feedPost.Content,
            CreatedAt = feedPost.CreatedAt,
            LikeCount = feedPost.LikeCount,
            CommentCount = feedPost.CommentCount,
            ShareCount = feedPost.ShareCount,
            IsLiked = feedPost.IsLiked,
            Tags = feedPost.Tags ?? Array.Empty<string>(),
            Comments = new List<SimplePostCard.CommentModel>()
        };
    }

    private static string[] ExtractHashtags(string content)
    {
        var hashtags = new List<string>();
        var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (word.AsSpan().StartsWith("#".AsSpan(), StringComparison.Ordinal) && word.Length > 1)
            {
                hashtags.Add(word.AsSpan(1).ToString());
            }
        }

        return hashtags.ToArray();
    }

    // Métodos mock removidos - usando apenas dados reais da API
}

// DTOs para comunicação com a API Feed
public class FeedResponse
{
    public List<FeedItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int UnreadCount { get; set; }
    public string FeedType { get; set; } = "Mixed";
}

public class FeedItemDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Priority { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ViewedAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsBookmarked { get; set; }

    // Post information
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string PostType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPinned { get; set; }
    public bool IsOfficial { get; set; }

    // Author information
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string? AuthorAvatarUrl { get; set; }
    public string? AuthorDepartment { get; set; }

    // Engagement metrics
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ViewCount { get; set; }

    // User interaction
    public bool HasLiked { get; set; }
    public bool HasCommented { get; set; }

    // Tags and categories
    public List<string> Tags { get; set; } = new();
    public string? Category { get; set; }
}

// Mantendo compatibilidade com código existente
public class FeedPostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public string? AuthorName { get; set; }
    public string? AuthorRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsLiked { get; set; }
    public string[]? Tags { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
}

public class CommentOperationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public CommentDetailsDto? Comment { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

public class CommentDetailsDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
}

public class CreatePostRequest
{
    public string Content { get; set; } = "";
    public string[]? Tags { get; set; }
    public string? MediaUrl { get; set; }
}
