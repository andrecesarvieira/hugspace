using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using System.Globalization;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

public partial class GetUserDiscussionAnalyticsQueryHandler : IRequestHandler<GetUserDiscussionAnalyticsQuery, UserDiscussionAnalyticsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetUserDiscussionAnalyticsQueryHandler> _logger;

    public GetUserDiscussionAnalyticsQueryHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<GetUserDiscussionAnalyticsQueryHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UserDiscussionAnalyticsDto> Handle(GetUserDiscussionAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        LogGeneratingUserAnalytics(_logger, userId, fromDate, toDate);

        try
        {
            // Busca informações do usuário
            var user = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

            if (user == null)
            {
                LogUserNotFound(_logger, userId);
                throw new InvalidOperationException("Usuário não encontrado.");
            }

            // Query base para comentários do usuário no período
            var userComments = await _context.Comments
                .Where(c => c.AuthorId == userId && 
                           c.CreatedAt >= fromDate && 
                           c.CreatedAt <= toDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Estatísticas básicas de comentários
            var totalComments = userComments.Count;
            var questionsByType = CalculateCommentsByType(userComments);

            // Calcula engagement recebido
            var engagementMetrics = await CalculateEngagementMetricsAsync(userComments, cancellationToken);

            // Calcula menções
            var mentionMetrics = await CalculateMentionMetricsAsync(userId, fromDate, toDate, cancellationToken);

            // Calcula atividades de moderação
            var moderationMetrics = await CalculateModerationMetricsAsync(userId, fromDate, toDate, cancellationToken);

            // Calcula score de engajamento
            var engagementScore = CalculateEngagementScore(totalComments, engagementMetrics.LikesReceived, engagementMetrics.EndorsementsReceived);

            // Calcula atividade por dia
            var activityByDay = CalculateActivityByDay(userComments, fromDate, toDate);

            // Conta dias ativos
            var activeDays = userComments
                .Select(c => c.CreatedAt.Date)
                .Distinct()
                .Count();

            LogUserAnalyticsGenerated(_logger, userId, totalComments, engagementScore);

            return new UserDiscussionAnalyticsDto
            {
                UserId = userId,
                UserName = user.FullName,
                FromDate = fromDate,
                ToDate = toDate,
                TotalComments = totalComments,
                QuestionsAsked = questionsByType.GetValueOrDefault("Question", 0),
                QuestionsAnswered = questionsByType.GetValueOrDefault("Answer", 0),
                SuggestionsGiven = questionsByType.GetValueOrDefault("Suggestion", 0),
                ConcernsRaised = questionsByType.GetValueOrDefault("Concern", 0),
                LikesReceived = engagementMetrics.LikesReceived,
                EndorsementsReceived = engagementMetrics.EndorsementsReceived,
                MentionsReceived = mentionMetrics.Received,
                MentionsMade = mentionMetrics.Made,
                CommentsModerated = moderationMetrics.Moderated,
                CommentsResolved = moderationMetrics.Resolved,
                CommentsHighlighted = moderationMetrics.Highlighted,
                EngagementScore = engagementScore,
                ActiveDays = activeDays,
                ActivityByDay = activityByDay,
                CommentsByType = questionsByType
            };
        }
        catch (Exception ex)
        {
            LogErrorGeneratingUserAnalytics(_logger, ex, userId);
            throw;
        }
    }

    private static Dictionary<string, int> CalculateCommentsByType(List<Domain.Entities.Communication.Comment> comments)
    {
        return comments
            .GroupBy(c => c.Type.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<(int LikesReceived, int EndorsementsReceived)> CalculateEngagementMetricsAsync(
        List<Domain.Entities.Communication.Comment> comments, 
        CancellationToken cancellationToken)
    {
        var commentIds = comments.Select(c => c.Id).ToList();

        var likesReceived = await _context.CommentLikes
            .Where(l => commentIds.Contains(l.CommentId))
            .CountAsync(cancellationToken);

        var endorsementsReceived = await _context.Endorsements
            .Where(e => e.CommentId.HasValue && commentIds.Contains(e.CommentId.Value))
            .CountAsync(cancellationToken);

        return (likesReceived, endorsementsReceived);
    }

    private async Task<(int Received, int Made)> CalculateMentionMetricsAsync(
        Guid userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var mentionsReceived = await _context.CommentMentions
            .Where(m => m.MentionedEmployeeId == userId &&
                       m.CreatedAt >= fromDate &&
                       m.CreatedAt <= toDate)
            .CountAsync(cancellationToken);

        var mentionsMade = await _context.CommentMentions
            .Where(m => m.MentionedById == userId &&
                       m.CreatedAt >= fromDate &&
                       m.CreatedAt <= toDate)
            .CountAsync(cancellationToken);

        return (mentionsReceived, mentionsMade);
    }

    private async Task<(int Moderated, int Resolved, int Highlighted)> CalculateModerationMetricsAsync(
        Guid userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var moderated = await _context.Comments
            .Where(c => c.ModeratedById == userId &&
                       c.ModeratedAt >= fromDate &&
                       c.ModeratedAt <= toDate)
            .CountAsync(cancellationToken);

        var resolved = await _context.Comments
            .Where(c => c.ResolvedById == userId &&
                       c.ResolvedAt >= fromDate &&
                       c.ResolvedAt <= toDate)
            .CountAsync(cancellationToken);

        // Para highlighted, assumimos que foi atualizado no período
        var highlighted = await _context.Comments
            .Where(c => c.AuthorId == userId &&
                       c.IsHighlighted &&
                       c.UpdatedAt >= fromDate &&
                       c.UpdatedAt <= toDate)
            .CountAsync(cancellationToken);

        return (moderated, resolved, highlighted);
    }

    private static double CalculateEngagementScore(int totalComments, int likesReceived, int endorsementsReceived)
    {
        if (totalComments == 0) return 0;

        // Fórmula: (comentários * 1) + (likes * 2) + (endorsements * 3) / comentários
        var rawScore = totalComments + (likesReceived * 2) + (endorsementsReceived * 3);
        var normalizedScore = (double)rawScore / totalComments;

        // Normaliza para escala de 0-100
        return Math.Min(normalizedScore * 10, 100);
    }

    private static Dictionary<string, int> CalculateActivityByDay(
        List<Domain.Entities.Communication.Comment> comments, 
        DateTime fromDate, 
        DateTime toDate)
    {
        var result = new Dictionary<string, int>();

        // Inicializa todos os dias no período com 0
        for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
        {
            result[date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)] = 0;
        }

        // Conta comentários por dia
        var commentsByDay = comments
            .GroupBy(c => c.CreatedAt.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            .ToDictionary(g => g.Key, g => g.Count());

        // Atualiza com dados reais
        foreach (var kvp in commentsByDay)
        {
            if (result.ContainsKey(kvp.Key))
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        return result;
    }

    [LoggerMessage(EventId = 2201, Level = LogLevel.Information,
        Message = "Gerando analytics para usuário: {UserId}, período: {FromDate} - {ToDate}")]
    private static partial void LogGeneratingUserAnalytics(ILogger logger, Guid userId, DateTime fromDate, DateTime toDate);

    [LoggerMessage(EventId = 2202, Level = LogLevel.Warning,
        Message = "Usuário não encontrado: {UserId}")]
    private static partial void LogUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 2203, Level = LogLevel.Information,
        Message = "Analytics gerado para usuário: {UserId}, {TotalComments} comentários, score: {EngagementScore:F1}")]
    private static partial void LogUserAnalyticsGenerated(ILogger logger, Guid userId, int totalComments, double engagementScore);

    [LoggerMessage(EventId = 2204, Level = LogLevel.Error,
        Message = "Erro ao gerar analytics para usuário: {UserId}")]
    private static partial void LogErrorGeneratingUserAnalytics(ILogger logger, Exception ex, Guid userId);
}

public record GetUserDiscussionAnalyticsQuery(
    Guid UserId,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<UserDiscussionAnalyticsDto>;