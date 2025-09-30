/*
 * SynQcore - Corporate Social Network
 *
 * Serviço de Discussion Threads
 * Interface para gerenciar threads de discussão e comentários corporativos
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SynQcore.BlazorApp.Models;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Interface do serviço de Discussion Threads
/// </summary>
public interface IDiscussionThreadService
{
    // Thread Management
    Task<DiscussionThreadDto?> GetDiscussionThreadAsync(Guid postId, bool includeModerated = false, string? filterByType = null, string orderBy = "CreatedAt");

    // Comment Operations
    Task<CommentOperationResponse?> CreateCommentAsync(CreateDiscussionCommentRequest request);
    Task<CommentOperationResponse?> UpdateCommentAsync(Guid commentId, UpdateDiscussionCommentRequest request);
    Task<CommentOperationResponse?> DeleteCommentAsync(Guid commentId);
    Task<DiscussionCommentDto?> GetCommentAsync(Guid commentId, bool includeReplies = true, int maxReplyDepth = 3);

    // Comment Actions
    Task<CommentOperationResponse?> ResolveCommentAsync(Guid commentId, string? resolutionNote);
    Task<CommentOperationResponse?> ModerateCommentAsync(Guid commentId, string moderationStatus, string? moderationReason);
    Task<CommentOperationResponse?> HighlightCommentAsync(Guid commentId, bool highlight = true);

    // Search and Filtering
    Task<PagedResult<DiscussionCommentDto>> SearchCommentsAsync(DiscussionCommentSearchRequest request);
    Task<PagedResult<DiscussionCommentDto>> GetPendingModerationCommentsAsync(int page = 1, int pageSize = 20, Guid? departmentId = null);

    // Mentions
    Task<PagedResult<MentionNotificationDto>> GetUserMentionsAsync(Guid? employeeId = null, bool onlyUnread = false, int page = 1, int pageSize = 20);
    Task<bool> MarkMentionAsReadAsync(Guid mentionId);
}

/// <summary>
/// Implementação do serviço de Discussion Threads
/// </summary>
public partial class DiscussionThreadService : IDiscussionThreadService
{
    private readonly IApiService _apiService;
    private readonly ILogger<DiscussionThreadService> _logger;

    public DiscussionThreadService(IApiService apiService, ILogger<DiscussionThreadService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    #region Thread Management

    [LoggerMessage(EventId = 6001, Level = LogLevel.Information,
        Message = "Carregando discussion thread para post {PostId} com parâmetros: includeModerated={IncludeModerated}, filterByType={FilterByType}, orderBy={OrderBy}")]
    private static partial void LogLoadingThread(ILogger logger, Guid postId, bool includeModerated, string? filterByType, string orderBy);

    public async Task<DiscussionThreadDto?> GetDiscussionThreadAsync(Guid postId, bool includeModerated = false, string? filterByType = null, string orderBy = "CreatedAt")
    {
        LogLoadingThread(_logger, postId, includeModerated, filterByType, orderBy);

        try
        {
            var queryParams = new List<string>
            {
                $"includeModerated={includeModerated.ToString().ToLowerInvariant()}",
                $"orderBy={orderBy}"
            };

            if (!string.IsNullOrEmpty(filterByType))
            {
                queryParams.Add($"filterByType={Uri.EscapeDataString(filterByType)}");
            }

            var queryString = string.Join("&", queryParams);
            var result = await _apiService.GetAsync<DiscussionThreadDto>($"/api/discussionthreads/posts/{postId}/thread?{queryString}");

            LogThreadLoaded(_logger, postId, result?.TotalComments ?? 0);
            return result;
        }
        catch (Exception ex)
        {
            LogErrorLoadingThread(_logger, ex, postId);
            return CreateFallbackThread(postId);
        }
    }

    [LoggerMessage(EventId = 6002, Level = LogLevel.Information,
        Message = "Thread carregada para post {PostId} com {CommentCount} comentários")]
    private static partial void LogThreadLoaded(ILogger logger, Guid postId, int commentCount);

    [LoggerMessage(EventId = 6003, Level = LogLevel.Error,
        Message = "Erro ao carregar thread do post {PostId}")]
    private static partial void LogErrorLoadingThread(ILogger logger, Exception ex, Guid postId);

    #endregion

    #region Comment Operations

    [LoggerMessage(EventId = 6004, Level = LogLevel.Information,
        Message = "Criando comentário no post {PostId} do tipo {Type}")]
    private static partial void LogCreatingComment(ILogger logger, Guid postId, string type);

    public async Task<CommentOperationResponse?> CreateCommentAsync(CreateDiscussionCommentRequest request)
    {
        LogCreatingComment(_logger, request.PostId, request.Type);

        try
        {
            var result = await _apiService.PostAsync<CommentOperationResponse>("/api/discussionthreads/comments", request);

            if (result?.Success == true)
            {
                LogCommentCreated(_logger, request.PostId, result.Comment?.Id ?? Guid.Empty);
            }
            else
            {
                LogCommentCreationFailed(_logger, request.PostId, result?.Message ?? "Erro desconhecido");
            }

            return result;
        }
        catch (Exception ex)
        {
            LogErrorCreatingComment(_logger, ex, request.PostId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro ao criar comentário. Tente novamente.",
                ValidationErrors = new List<string> { ex.Message }
            };
        }
    }

    [LoggerMessage(EventId = 6005, Level = LogLevel.Information,
        Message = "Comentário criado com sucesso no post {PostId} com ID {CommentId}")]
    private static partial void LogCommentCreated(ILogger logger, Guid postId, Guid commentId);

    [LoggerMessage(EventId = 6006, Level = LogLevel.Warning,
        Message = "Falha na criação de comentário no post {PostId}: {Message}")]
    private static partial void LogCommentCreationFailed(ILogger logger, Guid postId, string message);

    [LoggerMessage(EventId = 6007, Level = LogLevel.Error,
        Message = "Erro ao criar comentário no post {PostId}")]
    private static partial void LogErrorCreatingComment(ILogger logger, Exception ex, Guid postId);

    [LoggerMessage(EventId = 6008, Level = LogLevel.Information,
        Message = "Atualizando comentário {CommentId}")]
    private static partial void LogUpdatingComment(ILogger logger, Guid commentId);

    public async Task<CommentOperationResponse?> UpdateCommentAsync(Guid commentId, UpdateDiscussionCommentRequest request)
    {
        LogUpdatingComment(_logger, commentId);

        try
        {
            var result = await _apiService.PutAsync<CommentOperationResponse>($"/api/discussionthreads/comments/{commentId}", request);

            if (result?.Success == true)
            {
                LogCommentUpdated(_logger, commentId);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogErrorUpdatingComment(_logger, ex, commentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro ao atualizar comentário. Tente novamente.",
                ValidationErrors = new List<string> { ex.Message }
            };
        }
    }

    [LoggerMessage(EventId = 6009, Level = LogLevel.Information,
        Message = "Comentário {CommentId} atualizado com sucesso")]
    private static partial void LogCommentUpdated(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 6010, Level = LogLevel.Error,
        Message = "Erro ao atualizar comentário {CommentId}")]
    private static partial void LogErrorUpdatingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 6011, Level = LogLevel.Information,
        Message = "Excluindo comentário {CommentId}")]
    private static partial void LogDeletingComment(ILogger logger, Guid commentId);

    public async Task<CommentOperationResponse?> DeleteCommentAsync(Guid commentId)
    {
        LogDeletingComment(_logger, commentId);

        try
        {
            var success = await _apiService.DeleteAsync($"/api/discussionthreads/comments/{commentId}");

            if (success)
            {
                LogCommentDeleted(_logger, commentId);
                return new CommentOperationResponse { Success = true, Message = "Comentário excluído com sucesso" };
            }

            return new CommentOperationResponse { Success = false, Message = "Falha ao excluir comentário" };
        }
        catch (Exception ex)
        {
            LogErrorDeletingComment(_logger, ex, commentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro ao excluir comentário. Tente novamente.",
                ValidationErrors = new List<string> { ex.Message }
            };
        }
    }

    [LoggerMessage(EventId = 6012, Level = LogLevel.Information,
        Message = "Comentário {CommentId} excluído com sucesso")]
    private static partial void LogCommentDeleted(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 6013, Level = LogLevel.Error,
        Message = "Erro ao excluir comentário {CommentId}")]
    private static partial void LogErrorDeletingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 6014, Level = LogLevel.Information,
        Message = "Carregando comentário {CommentId} com includeReplies={IncludeReplies}, maxReplyDepth={MaxReplyDepth}")]
    private static partial void LogLoadingComment(ILogger logger, Guid commentId, bool includeReplies, int maxReplyDepth);

    public async Task<DiscussionCommentDto?> GetCommentAsync(Guid commentId, bool includeReplies = true, int maxReplyDepth = 3)
    {
        LogLoadingComment(_logger, commentId, includeReplies, maxReplyDepth);

        try
        {
            var queryString = $"includeReplies={includeReplies.ToString().ToLowerInvariant()}&maxReplyDepth={maxReplyDepth}";
            var result = await _apiService.GetAsync<DiscussionCommentDto>($"/api/discussionthreads/comments/{commentId}?{queryString}");

            LogCommentLoaded(_logger, commentId, result?.ReplyCount ?? 0);
            return result;
        }
        catch (Exception ex)
        {
            LogErrorLoadingComment(_logger, ex, commentId);
            return null;
        }
    }

    [LoggerMessage(EventId = 6015, Level = LogLevel.Information,
        Message = "Comentário {CommentId} carregado com {ReplyCount} respostas")]
    private static partial void LogCommentLoaded(ILogger logger, Guid commentId, int replyCount);

    [LoggerMessage(EventId = 6016, Level = LogLevel.Error,
        Message = "Erro ao carregar comentário {CommentId}")]
    private static partial void LogErrorLoadingComment(ILogger logger, Exception ex, Guid commentId);

    #endregion

    #region Comment Actions

    [LoggerMessage(EventId = 6017, Level = LogLevel.Information,
        Message = "Resolvendo comentário {CommentId}")]
    private static partial void LogResolvingComment(ILogger logger, Guid commentId);

    public async Task<CommentOperationResponse?> ResolveCommentAsync(Guid commentId, string? resolutionNote)
    {
        LogResolvingComment(_logger, commentId);

        try
        {
            var request = new { ResolutionNote = resolutionNote };
            var result = await _apiService.PostAsync<CommentOperationResponse>($"/api/discussionthreads/comments/{commentId}/resolve", request);

            if (result?.Success == true)
            {
                LogCommentResolved(_logger, commentId);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogErrorResolvingComment(_logger, ex, commentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro ao resolver comentário. Tente novamente.",
                ValidationErrors = new List<string> { ex.Message }
            };
        }
    }

    [LoggerMessage(EventId = 6018, Level = LogLevel.Information,
        Message = "Comentário {CommentId} resolvido com sucesso")]
    private static partial void LogCommentResolved(ILogger logger, Guid commentId);

    [LoggerMessage(EventId = 6019, Level = LogLevel.Error,
        Message = "Erro ao resolver comentário {CommentId}")]
    private static partial void LogErrorResolvingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 6020, Level = LogLevel.Information,
        Message = "Moderando comentário {CommentId} com status {ModerationStatus}")]
    private static partial void LogModeratingComment(ILogger logger, Guid commentId, string moderationStatus);

    public async Task<CommentOperationResponse?> ModerateCommentAsync(Guid commentId, string moderationStatus, string? moderationReason)
    {
        LogModeratingComment(_logger, commentId, moderationStatus);

        try
        {
            var request = new { ModerationStatus = moderationStatus, ModerationReason = moderationReason };
            var result = await _apiService.PostAsync<CommentOperationResponse>($"/api/discussionthreads/comments/{commentId}/moderate", request);

            if (result?.Success == true)
            {
                LogCommentModerated(_logger, commentId, moderationStatus);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogErrorModeratingComment(_logger, ex, commentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro ao moderar comentário. Tente novamente.",
                ValidationErrors = new List<string> { ex.Message }
            };
        }
    }

    [LoggerMessage(EventId = 6021, Level = LogLevel.Information,
        Message = "Comentário {CommentId} moderado com status {ModerationStatus}")]
    private static partial void LogCommentModerated(ILogger logger, Guid commentId, string moderationStatus);

    [LoggerMessage(EventId = 6022, Level = LogLevel.Error,
        Message = "Erro ao moderar comentário {CommentId}")]
    private static partial void LogErrorModeratingComment(ILogger logger, Exception ex, Guid commentId);

    [LoggerMessage(EventId = 6023, Level = LogLevel.Information,
        Message = "Alterando destaque do comentário {CommentId} para {Highlight}")]
    private static partial void LogHighlightingComment(ILogger logger, Guid commentId, bool highlight);

    public async Task<CommentOperationResponse?> HighlightCommentAsync(Guid commentId, bool highlight = true)
    {
        LogHighlightingComment(_logger, commentId, highlight);

        try
        {
            var result = await _apiService.PostAsync<CommentOperationResponse>($"/api/discussionthreads/comments/{commentId}/highlight?highlight={highlight.ToString().ToLowerInvariant()}", new { });

            if (result?.Success == true)
            {
                LogCommentHighlighted(_logger, commentId, highlight);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogErrorHighlightingComment(_logger, ex, commentId);
            return new CommentOperationResponse
            {
                Success = false,
                Message = "Erro ao alterar destaque do comentário. Tente novamente.",
                ValidationErrors = new List<string> { ex.Message }
            };
        }
    }

    [LoggerMessage(EventId = 6024, Level = LogLevel.Information,
        Message = "Destaque do comentário {CommentId} alterado para {Highlight}")]
    private static partial void LogCommentHighlighted(ILogger logger, Guid commentId, bool highlight);

    [LoggerMessage(EventId = 6025, Level = LogLevel.Error,
        Message = "Erro ao alterar destaque do comentário {CommentId}")]
    private static partial void LogErrorHighlightingComment(ILogger logger, Exception ex, Guid commentId);

    #endregion

    #region Search and Filtering

    [LoggerMessage(EventId = 6026, Level = LogLevel.Information,
        Message = "Buscando comentários com termo '{SearchTerm}' - Página {Page}, Tamanho {PageSize}")]
    private static partial void LogSearchingComments(ILogger logger, string searchTerm, int page, int pageSize);

    public async Task<PagedResult<DiscussionCommentDto>> SearchCommentsAsync(DiscussionCommentSearchRequest request)
    {
        LogSearchingComments(_logger, request.SearchTerm ?? "", request.Page, request.PageSize);

        try
        {
            var queryParams = new List<string>
            {
                $"page={request.Page}",
                $"pageSize={request.PageSize}"
            };

            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(request.SearchTerm)}");

            if (request.PostId.HasValue)
                queryParams.Add($"postId={request.PostId.Value}");

            if (!string.IsNullOrEmpty(request.CommentType))
                queryParams.Add($"commentType={Uri.EscapeDataString(request.CommentType)}");

            if (!string.IsNullOrEmpty(request.ModerationStatus))
                queryParams.Add($"moderationStatus={Uri.EscapeDataString(request.ModerationStatus)}");

            if (request.FromDate.HasValue)
                queryParams.Add($"fromDate={request.FromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");

            if (request.ToDate.HasValue)
                queryParams.Add($"toDate={request.ToDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");

            var queryString = string.Join("&", queryParams);
            var result = await _apiService.GetAsync<List<DiscussionCommentDto>>($"/api/discussionthreads/comments/search?{queryString}");

            // Simular paginação (em produção viria do backend)
            return new PagedResult<DiscussionCommentDto>
            {
                Items = result ?? new List<DiscussionCommentDto>(),
                TotalCount = result?.Count ?? 0,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            LogErrorSearchingComments(_logger, ex, request.SearchTerm ?? "");
            return CreateFallbackSearchResult();
        }
    }

    [LoggerMessage(EventId = 6027, Level = LogLevel.Information,
        Message = "Busca de comentários concluída para '{SearchTerm}' - {ResultCount} resultados encontrados")]
    private static partial void LogCommentsSearched(ILogger logger, string searchTerm, int resultCount);

    [LoggerMessage(EventId = 6028, Level = LogLevel.Error,
        Message = "Erro na busca de comentários com termo '{SearchTerm}'")]
    private static partial void LogErrorSearchingComments(ILogger logger, Exception ex, string searchTerm);

    [LoggerMessage(EventId = 6029, Level = LogLevel.Information,
        Message = "Carregando comentários pendentes de moderação - Página {Page}, Tamanho {PageSize}")]
    private static partial void LogLoadingPendingComments(ILogger logger, int page, int pageSize);

    public async Task<PagedResult<DiscussionCommentDto>> GetPendingModerationCommentsAsync(int page = 1, int pageSize = 20, Guid? departmentId = null)
    {
        LogLoadingPendingComments(_logger, page, pageSize);

        try
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (departmentId.HasValue)
                queryParams.Add($"departmentId={departmentId.Value}");

            var queryString = string.Join("&", queryParams);
            var result = await _apiService.GetAsync<List<DiscussionCommentDto>>($"/api/discussionthreads/moderation/pending?{queryString}");

            // Simular paginação (em produção viria do backend)
            return new PagedResult<DiscussionCommentDto>
            {
                Items = result ?? new List<DiscussionCommentDto>(),
                TotalCount = result?.Count ?? 0,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            LogErrorLoadingPendingComments(_logger, ex);
            return CreateFallbackSearchResult();
        }
    }

    [LoggerMessage(EventId = 6030, Level = LogLevel.Information,
        Message = "Carregados {Count} comentários pendentes de moderação")]
    private static partial void LogPendingCommentsLoaded(ILogger logger, int count);

    [LoggerMessage(EventId = 6031, Level = LogLevel.Error,
        Message = "Erro ao carregar comentários pendentes de moderação")]
    private static partial void LogErrorLoadingPendingComments(ILogger logger, Exception ex);

    #endregion

    #region Mentions

    [LoggerMessage(EventId = 6032, Level = LogLevel.Information,
        Message = "Carregando menções do usuário {EmployeeId} - onlyUnread={OnlyUnread}, Página {Page}")]
    private static partial void LogLoadingMentions(ILogger logger, Guid? employeeId, bool onlyUnread, int page);

    public async Task<PagedResult<MentionNotificationDto>> GetUserMentionsAsync(Guid? employeeId = null, bool onlyUnread = false, int page = 1, int pageSize = 20)
    {
        LogLoadingMentions(_logger, employeeId, onlyUnread, page);

        try
        {
            var queryParams = new List<string>
            {
                $"onlyUnread={onlyUnread.ToString().ToLowerInvariant()}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (employeeId.HasValue)
                queryParams.Add($"employeeId={employeeId.Value}");

            var queryString = string.Join("&", queryParams);
            var result = await _apiService.GetAsync<List<MentionNotificationDto>>($"/api/discussionthreads/mentions?{queryString}");

            // Simular paginação (em produção viria do backend)
            return new PagedResult<MentionNotificationDto>
            {
                Items = result ?? new List<MentionNotificationDto>(),
                TotalCount = result?.Count ?? 0,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            LogErrorLoadingMentions(_logger, ex, employeeId);
            return new PagedResult<MentionNotificationDto>
            {
                Items = new List<MentionNotificationDto>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    [LoggerMessage(EventId = 6033, Level = LogLevel.Information,
        Message = "Carregadas {Count} menções para usuário {EmployeeId}")]
    private static partial void LogMentionsLoaded(ILogger logger, Guid? employeeId, int count);

    [LoggerMessage(EventId = 6034, Level = LogLevel.Error,
        Message = "Erro ao carregar menções do usuário {EmployeeId}")]
    private static partial void LogErrorLoadingMentions(ILogger logger, Exception ex, Guid? employeeId);

    [LoggerMessage(EventId = 6035, Level = LogLevel.Error,
        Message = "Erro ao marcar menção {MentionId} como lida")]
    private static partial void LogErrorMarkingMentionAsRead(ILogger logger, Exception ex, Guid mentionId);

    public async Task<bool> MarkMentionAsReadAsync(Guid mentionId)
    {
        try
        {
            var result = await _apiService.PostAsync<bool>($"/api/discussionthreads/mentions/{mentionId}/mark-read", new { });
            return result;
        }
        catch (Exception ex)
        {
            LogErrorMarkingMentionAsRead(_logger, ex, mentionId);
            return false;
        }
    }

    #endregion

    #region Helper Methods

    private static DiscussionThreadDto CreateFallbackThread(Guid postId)
    {
        return new DiscussionThreadDto
        {
            PostId = postId,
            PostTitle = "Thread de Discussão",
            Comments = new List<DiscussionCommentDto>(),
            TotalComments = 0,
            UnresolvedQuestions = 0,
            FlaggedComments = 0,
            LastActivityAt = DateTime.Now
        };
    }

    private static PagedResult<DiscussionCommentDto> CreateFallbackSearchResult()
    {
        return new PagedResult<DiscussionCommentDto>
        {
            Items = new List<DiscussionCommentDto>(),
            TotalCount = 0,
            Page = 1,
            PageSize = 20
        };
    }

    #endregion
}

#region Request DTOs

/// <summary>
/// Request para criar comentário de discussão
/// </summary>
public class CreateDiscussionCommentRequest
{
    [Required(ErrorMessage = "O conteúdo é obrigatório")]
    [MinLength(1, ErrorMessage = "O conteúdo deve ter pelo menos 1 caractere")]
    [MaxLength(5000, ErrorMessage = "O conteúdo deve ter no máximo 5000 caracteres")]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid PostId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [Required]
    public string Type { get; set; } = "Regular";

    [Required]
    public string Visibility { get; set; } = "Public";

    public bool IsConfidential { get; set; }

    [Required]
    public string Priority { get; set; } = "Normal";

    public List<CreateCommentMentionRequest> Mentions { get; set; } = new();
}

/// <summary>
/// Request para atualizar comentário de discussão
/// </summary>
public class UpdateDiscussionCommentRequest
{
    [Required(ErrorMessage = "O conteúdo é obrigatório")]
    [MinLength(1, ErrorMessage = "O conteúdo deve ter pelo menos 1 caractere")]
    [MaxLength(5000, ErrorMessage = "O conteúdo deve ter no máximo 5000 caracteres")]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "Regular";

    [Required]
    public string Visibility { get; set; } = "Public";

    public bool IsConfidential { get; set; }

    [Required]
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// Request para busca de comentários
/// </summary>
public class DiscussionCommentSearchRequest
{
    public string? SearchTerm { get; set; }
    public Guid? PostId { get; set; }
    public string? CommentType { get; set; }
    public string? ModerationStatus { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Request para criar menção em comentário
/// </summary>
public class CreateCommentMentionRequest
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public string EmployeeName { get; set; } = string.Empty;

    public int StartPosition { get; set; }
    public int Length { get; set; }
}

#endregion

#region Response DTOs

/// <summary>
/// DTO da thread de discussão
/// </summary>
public class DiscussionThreadDto
{
    public Guid PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public List<DiscussionCommentDto> Comments { get; set; } = new();
    public int TotalComments { get; set; }
    public int UnresolvedQuestions { get; set; }
    public int FlaggedComments { get; set; }
    public DateTime LastActivityAt { get; set; }
}

/// <summary>
/// DTO do comentário de discussão
/// </summary>
public class DiscussionCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorJobTitle { get; set; } = string.Empty;
    public string? AuthorProfilePhotoUrl { get; set; }

    // Thread hierarchy
    public Guid? ParentCommentId { get; set; }
    public int ThreadLevel { get; set; }
    public string ThreadPath { get; set; } = string.Empty;
    public int ReplyCount { get; set; }
    public List<DiscussionCommentDto> Replies { get; set; } = new();

    // Discussion features
    public string Type { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? ResolvedByName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }

    // Moderation
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsFlagged { get; set; }
    public string ModerationStatus { get; set; } = string.Empty;
    public string? ModerationReason { get; set; }
    public DateTime? ModeratedAt { get; set; }

    // Visibility and priority
    public string Visibility { get; set; } = string.Empty;
    public bool IsConfidential { get; set; }
    public bool IsHighlighted { get; set; }
    public string Priority { get; set; } = string.Empty;

    // Engagement metrics
    public int LikeCount { get; set; }
    public int EndorsementCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public DateTime LastActivityAt { get; set; }

    // Mentions
    public List<CommentMentionDto> Mentions { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO de menção em comentário
/// </summary>
public class CommentMentionDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int StartPosition { get; set; }
    public int Length { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO de notificação de menção
/// </summary>
public class MentionNotificationDto
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public Guid MentionedById { get; set; }
    public string MentionedByName { get; set; } = string.Empty;
    public string CommentPreview { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

#endregion
