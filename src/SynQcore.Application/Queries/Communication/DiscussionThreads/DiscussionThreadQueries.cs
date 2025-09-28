using MediatR;
using SynQcore.Application.DTOs.Communication;

namespace SynQcore.Application.Queries.Communication.DiscussionThreads;

/// <summary>
/// Query para obter thread de discussão completa
/// </summary>
public record GetDiscussionThreadQuery(
    Guid PostId,
    bool IncludeModerated = false,
    string? FilterByType = null,
    string OrderBy = "CreatedAt"
) : IRequest<DiscussionThreadDto>;

/// <summary>
/// Query para obter comentário específico com replies
/// </summary>
public record GetDiscussionCommentQuery(
    Guid CommentId,
    bool IncludeReplies = true,
    int MaxReplyDepth = 5
) : IRequest<DiscussionCommentDto?>;

/// <summary>
/// Query para obter comentários pendentes de moderação
/// </summary>
public record GetPendingModerationCommentsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? DepartmentId = null
) : IRequest<List<DiscussionCommentDto>>;

/// <summary>
/// Query para obter comentários não resolvidos (Questions/Concerns)
/// </summary>
public record GetUnresolvedCommentsQuery(
    Guid PostId,
    string? FilterByType = null
) : IRequest<List<DiscussionCommentDto>>;

/// <summary>
/// Query para obter menções do usuário
/// </summary>
public record GetUserMentionsQuery(
    Guid EmployeeId,
    bool OnlyUnread = false,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<MentionNotificationDto>>;

/// <summary>
/// Query para analytics de discussion threads
/// </summary>
public record GetDiscussionAnalyticsQuery(
    Guid? PostId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? DepartmentId = null
) : IRequest<DiscussionAnalyticsDto>;

/// <summary>
/// Query para trending discussions (discussões em alta)
/// </summary>
public record GetTrendingDiscussionsQuery(
    int Hours = 24,
    int Page = 1,
    int PageSize = 10,
    string? Department = null,
    string? Category = null
) : IRequest<PagedTrendingDiscussionsResponse>;

/// <summary>
/// Query para comentários que precisam de atenção
/// </summary>
public record GetCommentsNeedingAttentionQuery(
    Guid? UserId = null,
    bool UnresolvedOnly = true,
    bool HighPriorityOnly = false,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedCommentsResponse>;

/// <summary>
/// Query para estatísticas de engagement por período
/// </summary>
public record GetEngagementStatisticsQuery(
    DateTime FromDate,
    DateTime ToDate,
    string GroupBy = "Day", // Day, Week, Month
    Guid? DepartmentId = null,
    Guid? TeamId = null
) : IRequest<EngagementStatisticsDto>;

/// <summary>
/// Query para buscar comentários por critérios avançados
/// </summary>
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

/// <summary>
/// DTO para analytics de discussions
/// </summary>
public class DiscussionAnalyticsDto
{
    /// <summary>
    /// Total de comentários em todas as discussions.
    /// </summary>
    public int TotalComments { get; set; }

    /// <summary>
    /// Total de threads de discussão ativas.
    /// </summary>
    public int TotalThreads { get; set; }

    /// <summary>
    /// Número de questões ainda não resolvidas.
    /// </summary>
    public int UnresolvedQuestions { get; set; }

    /// <summary>
    /// Número de comentários aguardando moderação.
    /// </summary>
    public int PendingModeration { get; set; }

    /// <summary>
    /// Total de menções em comentários.
    /// </summary>
    public int TotalMentions { get; set; }

    /// <summary>
    /// Distribuição de comentários por tipo (Questão, Preocupação, etc.).
    /// </summary>
    public Dictionary<string, int> CommentsByType { get; set; } = [];

    /// <summary>
    /// Distribuição de comentários por nível de visibilidade.
    /// </summary>
    public Dictionary<string, int> CommentsByVisibility { get; set; } = [];

    /// <summary>
    /// Estatísticas de status de moderação de comentários.
    /// </summary>
    public Dictionary<string, int> ModerationStatusStats { get; set; } = [];

    /// <summary>
    /// Lista dos principais contribuidores das discussions.
    /// </summary>
    public List<TopContributor> TopContributors { get; set; } = [];

    /// <summary>
    /// Lista das threads mais ativas por participação.
    /// </summary>
    public List<ActiveThread> MostActiveThreads { get; set; } = [];
}

/// <summary>
/// Representa um contribuidor destacado em discussions.
/// </summary>
public class TopContributor
{
    /// <summary>
    /// ID do funcionário contribuidor.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Nome completo do contribuidor.
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Número total de comentários feitos.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Número de questões respondidas pelo contribuidor.
    /// </summary>
    public int QuestionsAnswered { get; set; }

    /// <summary>
    /// Número de endorsements recebidos pelo contribuidor.
    /// </summary>
    public int EndorsementsReceived { get; set; }
}

/// <summary>
/// Representa uma thread de discussão ativa com métricas de engajamento.
/// </summary>
public class ActiveThread
{
    /// <summary>
    /// ID do post que iniciou a thread.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Título do post da thread.
    /// </summary>
    public string PostTitle { get; set; } = string.Empty;

    /// <summary>
    /// Número total de comentários na thread.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Número de participantes únicos na thread.
    /// </summary>
    public int ParticipantCount { get; set; }

    /// <summary>
    /// Data e hora da última atividade na thread.
    /// </summary>
    public DateTime LastActivityAt { get; set; }
}
