using MediatR;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Application.Queries.Communication.DiscussionThreads;

public record GetDiscussionThreadQuery(
    Guid PostId,
    bool IncludeModerated = false,
    string? FilterByType = null,
    string OrderBy = "CreatedAt"
) : IRequest<DiscussionThreadDto>;

public record GetDiscussionCommentQuery(
    Guid CommentId,
    bool IncludeReplies = true,
    int MaxReplyDepth = 5
) : IRequest<DiscussionCommentDto?>;

public record GetPendingModerationCommentsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? DepartmentId = null
) : IRequest<List<DiscussionCommentDto>>;

public record GetUnresolvedCommentsQuery(
    Guid PostId,
    string? FilterByType = null
) : IRequest<List<DiscussionCommentDto>>;

public record GetUserMentionsQuery(
    Guid EmployeeId,
    bool OnlyUnread = false,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<MentionNotificationDto>>;

public record GetDiscussionAnalyticsQuery(
    Guid? PostId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? DepartmentId = null
) : IRequest<DiscussionAnalyticsDto>;

public record GetTrendingDiscussionsQuery(
    int Hours = 24,
    int Page = 1,
    int PageSize = 10,
    string? Department = null,
    string? Category = null
) : IRequest<PagedTrendingDiscussionsResponse>;

public record GetCommentsNeedingAttentionQuery(
    Guid? UserId = null,
    bool UnresolvedOnly = true,
    bool HighPriorityOnly = false,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedCommentsResponse>;

public record GetEngagementStatisticsQuery(
    DateTime FromDate,
    DateTime ToDate,
    string GroupBy = "Day", // Day, Week, Month
    Guid? DepartmentId = null,
    Guid? TeamId = null
) : IRequest<EngagementStatisticsDto>;

public record SearchDiscussionCommentsQuery(
    string SearchTerm,
    Guid? PostId = null,
    string? CommentType = null,
    string? ModerationStatus = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<DiscussionCommentDto>>;

public class DiscussionAnalyticsDto
{
    public int TotalComments { get; set; }

    public int TotalThreads { get; set; }

    public int UnresolvedQuestions { get; set; }

    public int PendingModeration { get; set; }

    public int TotalMentions { get; set; }

    public Dictionary<string, int> CommentsByType { get; set; } = [];

    public Dictionary<string, int> CommentsByVisibility { get; set; } = [];

    public Dictionary<string, int> ModerationStatusStats { get; set; } = [];

    public List<TopContributor> TopContributors { get; set; } = [];

    public List<ActiveThread> MostActiveThreads { get; set; } = [];
}

public class TopContributor
{
    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int CommentCount { get; set; }

    public int QuestionsAnswered { get; set; }

    public int EndorsementsReceived { get; set; }
}

public class ActiveThread
{
    public Guid PostId { get; set; }

    public string PostTitle { get; set; } = string.Empty;

    public int CommentCount { get; set; }

    public int ParticipantCount { get; set; }

    public DateTime LastActivityAt { get; set; }
}
