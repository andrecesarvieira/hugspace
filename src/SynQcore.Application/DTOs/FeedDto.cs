using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.DTOs;

/// <summary>
/// DTO para item do feed corporativo
/// Representa uma entrada no timeline personalizado do usuário
/// </summary>
public record FeedItemDto
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public string Priority { get; init; } = string.Empty;
    public double RelevanceScore { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ViewedAt { get; init; }
    public bool IsRead { get; init; }
    public bool IsBookmarked { get; init; }
    
    // Post information
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Summary { get; init; }
    public string PostType { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public bool IsPinned { get; init; }
    public bool IsOfficial { get; init; }
    
    // Author information
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string AuthorEmail { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public string? AuthorDepartment { get; init; }
    
    // Engagement metrics
    public int LikeCount { get; init; }
    public int CommentCount { get; init; }
    public int ViewCount { get; init; }
    
    // User interaction
    public bool HasLiked { get; init; }
    public bool HasCommented { get; init; }
    
    // Tags and categories
    public List<string> Tags { get; init; } = [];
    public string? Category { get; init; }
}

/// <summary>
/// DTO para resposta de feed paginado
/// </summary>
public record CorporateFeedResponseDto
{
    public List<FeedItemDto> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public string? NextPageToken { get; init; }
    
    // Feed metadata
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
    public int UnreadCount { get; init; }
    public string FeedType { get; init; } = "Mixed"; // Mixed, Department, Team, Following
}

/// <summary>
/// DTO para filtros de feed
/// </summary>
public record FeedFiltersDto
{
    public List<string>? PostTypes { get; init; }
    public List<string>? Departments { get; init; }
    public List<string>? Tags { get; init; }
    public List<string>? Categories { get; init; }
    public List<string>? Authors { get; init; }
    public string? Priority { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public bool? OnlyUnread { get; init; }
    public bool? OnlyBookmarked { get; init; }
}

/// <summary>
/// DTO para estatísticas do feed
/// </summary>
public record FeedStatsDto
{
    public int TotalItems { get; init; }
    public int UnreadCount { get; init; }
    public int BookmarkedCount { get; init; }
    public int HiddenCount { get; init; }
    public Dictionary<string, int> PriorityBreakdown { get; init; } = new();
    public List<TopEngagedPostDto> TopEngagedPosts { get; init; } = [];
    public List<RecentInteractionDto> RecentInteractions { get; init; } = [];
    public DateTime LastUpdated { get; init; }
}

/// <summary>
/// Post com maior engajamento no feed
/// </summary>
public record TopEngagedPostDto
{
    public Guid PostId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int EngagementCount { get; init; }
    public double RelevanceScore { get; init; }
}

/// <summary>
/// Interação recente do usuário
/// </summary>
public record RecentInteractionDto
{
    public Guid FeedEntryId { get; init; }
    public string PostTitle { get; init; } = string.Empty;
    public string InteractionType { get; init; } = string.Empty;
    public DateTime InteractionDate { get; init; }
}

/// <summary>
/// Resposta com interesses do usuário
/// </summary>
public record UserInterestsResponseDto
{
    public List<UserInterestDto> TopInterests { get; init; } = [];
    public Dictionary<string, List<UserInterestDto>> InterestsByType { get; init; } = new();
    public int TotalInterests { get; init; }
    public double AverageScore { get; init; }
    public int TotalInteractions { get; init; }
    public DateTime LastUpdated { get; init; }
}

/// <summary>
/// DTO para interesse do usuário
/// </summary>
public record UserInterestDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public double Score { get; init; }
    public int InteractionCount { get; init; }
    public string Source { get; init; } = string.Empty;
    public DateTime FirstInteractionAt { get; init; }
    public DateTime LastInteractionAt { get; init; }
}