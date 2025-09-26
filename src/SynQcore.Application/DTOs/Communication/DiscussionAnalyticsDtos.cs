namespace SynQcore.Application.DTOs.Communication;

/// <summary>
/// Resposta paginada para comentários
/// </summary>
public class PagedCommentsResponse
{
    public List<DiscussionCommentDto> Comments { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// Resposta paginada para menções
/// </summary>
public class PagedMentionsResponse
{
    public List<MentionNotificationDto> Mentions { get; set; } = [];
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Analytics detalhado de participação de usuário
/// </summary>
public class UserDiscussionAnalyticsDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    // Estatísticas de comentários
    public int TotalComments { get; set; }
    public int QuestionsAsked { get; set; }
    public int QuestionsAnswered { get; set; }
    public int SuggestionsGiven { get; set; }
    public int ConcernsRaised { get; set; }

    // Engagement
    public int LikesReceived { get; set; }
    public int EndorsementsReceived { get; set; }
    public int MentionsReceived { get; set; }
    public int MentionsMade { get; set; }

    // Moderação
    public int CommentsModerated { get; set; }
    public int CommentsResolved { get; set; }
    public int CommentsHighlighted { get; set; }

    // Trending
    public double EngagementScore { get; set; }
    public int ActiveDays { get; set; }
    public Dictionary<string, int> ActivityByDay { get; set; } = [];
    public Dictionary<string, int> CommentsByType { get; set; } = [];
}

/// <summary>
/// Métricas de moderação corporativa
/// </summary>
public class ModerationMetricsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? ModeratorId { get; set; }
    public string? ModeratorName { get; set; }

    // Estatísticas gerais
    public int TotalCommentsModerated { get; set; }
    public int AverageResponseTime { get; set; } // em minutos
    public double ApprovalRate { get; set; } // percentual

    // Por status
    public int Approved { get; set; }
    public int Flagged { get; set; }
    public int Hidden { get; set; }
    public int Rejected { get; set; }
    public int UnderReview { get; set; }

    // Tendências
    public Dictionary<string, int> ModerationByDay { get; set; } = [];
    public Dictionary<string, int> ModerationByType { get; set; } = [];
    public List<ModerationTrendItem> Trends { get; set; } = [];

    // Top moderadores (se não filtrado por moderador específico)
    public List<ModeratorStats> TopModerators { get; set; } = [];
}

/// <summary>
/// Estatísticas de engajamento por período
/// </summary>
public class EngagementStatisticsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string GroupBy { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }

    // Métricas principais
    public int TotalComments { get; set; }
    public int UniqueParticipants { get; set; }
    public int TotalThreads { get; set; }
    public double AverageResponseTime { get; set; } // em horas

    // Engagement detalhado
    public int TotalLikes { get; set; }
    public int TotalEndorsements { get; set; }
    public int TotalMentions { get; set; }
    public int ResolvedQuestions { get; set; }

    // Dados temporais
    public List<EngagementDataPoint> TimeSeries { get; set; } = [];
    public Dictionary<string, int> EngagementByType { get; set; } = [];
    public Dictionary<string, double> PeakHours { get; set; } = [];
}

/// <summary>
/// Resposta para discussões em trending
/// </summary>
public class PagedTrendingDiscussionsResponse
{
    public List<TrendingDiscussionDto> Discussions { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int AnalysisPeriodHours { get; set; }
}

/// <summary>
/// DTO para discussão em trending
/// </summary>
public class TrendingDiscussionDto
{
    public Guid PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public string PostContent { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }

    // Métricas de trending
    public int CommentCount { get; set; }
    public int UniqueParticipants { get; set; }
    public int LikesCount { get; set; }
    public int EndorsementCount { get; set; }
    public double TrendingScore { get; set; }
    public double GrowthRate { get; set; }

    // Timing
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public int HoursSinceLastActivity { get; set; }

    // Analytics adicionais
    public Dictionary<string, int> CommentsByType { get; set; } = [];
    public int UnresolvedQuestions { get; set; }
    public bool HasHighPriorityItems { get; set; }
}

// Classes auxiliares
public class ModerationTrendItem
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ModeratorStats
{
    public Guid ModeratorId { get; set; }
    public string ModeratorName { get; set; } = string.Empty;
    public int CommentsModerated { get; set; }
    public double AverageResponseTime { get; set; }
    public double ApprovalRate { get; set; }
}

public class EngagementDataPoint
{
    public string Date { get; set; } = string.Empty;
    public int Comments { get; set; }
    public int Participants { get; set; }
    public int Likes { get; set; }
    public int Endorsements { get; set; }
    public double EngagementScore { get; set; }
}