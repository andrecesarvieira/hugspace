using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.Queries.Communication.DiscussionThreads;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

public partial class GetDiscussionAnalyticsQueryHandler : IRequestHandler<GetDiscussionAnalyticsQuery, DiscussionAnalyticsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDiscussionAnalyticsQueryHandler> _logger;

    public GetDiscussionAnalyticsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetDiscussionAnalyticsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DiscussionAnalyticsDto> Handle(GetDiscussionAnalyticsQuery request, CancellationToken cancellationToken)
    {
        LogGeneratingAnalytics(_logger, request.PostId, request.FromDate, request.ToDate);

        try
        {
            // Query base para comentários
            var commentsQuery = _context.Comments.AsQueryable();

            // Aplica filtros
            if (request.PostId.HasValue)
            {
                commentsQuery = commentsQuery.Where(c => c.PostId == request.PostId.Value);
            }

            if (request.FromDate.HasValue)
            {
                commentsQuery = commentsQuery.Where(c => c.CreatedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                commentsQuery = commentsQuery.Where(c => c.CreatedAt <= request.ToDate.Value);
            }

            if (request.DepartmentId.HasValue)
            {
                commentsQuery = commentsQuery
                    .Include(c => c.Author)
                        .ThenInclude(a => a.EmployeeDepartments)
                    .Where(c => c.Author.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId.Value));
            }

            // Carrega dados necessários
            var comments = await commentsQuery
                .Include(c => c.Author)
                .Include(c => c.Post)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Calcula estatísticas principais
            var totalComments = comments.Count;
            var totalThreads = comments.Where(c => !c.ParentCommentId.HasValue).Count();
            var unresolvedQuestions = comments.Count(c =>
                (c.Type == Domain.Entities.Communication.CommentType.Question ||
                 c.Type == Domain.Entities.Communication.CommentType.Concern) &&
                !c.IsResolved);
            var pendingModeration = comments.Count(c => c.ModerationStatus == Domain.Entities.Communication.ModerationStatus.Pending);

            // Calcula total de menções
            var totalMentions = await CalculateTotalMentionsAsync(commentsQuery, cancellationToken);

            // Estatísticas por tipo
            var commentsByType = comments
                .GroupBy(c => c.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Estatísticas por visibilidade
            var commentsByVisibility = comments
                .GroupBy(c => c.Visibility.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Estatísticas de moderação
            var moderationStats = comments
                .GroupBy(c => c.ModerationStatus.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Top contributors
            var topContributors = await CalculateTopContributorsAsync(comments, cancellationToken);

            // Threads mais ativas
            var mostActiveThreads = await CalculateMostActiveThreadsAsync(request, cancellationToken);

            LogAnalyticsGenerated(_logger, totalComments, totalThreads, unresolvedQuestions);

            return new DiscussionAnalyticsDto
            {
                TotalComments = totalComments,
                TotalThreads = totalThreads,
                UnresolvedQuestions = unresolvedQuestions,
                PendingModeration = pendingModeration,
                TotalMentions = totalMentions,
                CommentsByType = commentsByType,
                CommentsByVisibility = commentsByVisibility,
                ModerationStatusStats = moderationStats,
                TopContributors = topContributors,
                MostActiveThreads = mostActiveThreads
            };
        }
        catch (Exception ex)
        {
            LogErrorGeneratingAnalytics(_logger, ex, request.PostId);
            throw;
        }
    }

    private async Task<int> CalculateTotalMentionsAsync(IQueryable<Domain.Entities.Communication.Comment> commentsQuery, CancellationToken cancellationToken)
    {
        var commentIds = await commentsQuery.Select(c => c.Id).ToListAsync(cancellationToken);

        return await _context.CommentMentions
            .Where(m => commentIds.Contains(m.CommentId))
            .CountAsync(cancellationToken);
    }

    private async Task<List<TopContributor>> CalculateTopContributorsAsync(
        List<Domain.Entities.Communication.Comment> comments,
        CancellationToken cancellationToken)
    {
        var contributorStats = comments
            .GroupBy(c => new { c.AuthorId, c.Author.FullName })
            .Select(g => new
            {
                g.Key.AuthorId,
                g.Key.FullName,
                CommentCount = g.Count(),
                QuestionsAnswered = g.Count(c => c.Type == Domain.Entities.Communication.CommentType.Answer),
                Comments = g.ToList()
            })
            .OrderByDescending(x => x.CommentCount)
            .Take(10)
            .ToList();

        var result = new List<TopContributor>();

        foreach (var contributor in contributorStats)
        {
            // Calcula endorsements recebidos
            var commentIds = contributor.Comments.Select(c => c.Id).ToList();
            var endorsementsReceived = await _context.Endorsements
                .Where(e => commentIds.Contains(e.CommentId!.Value))
                .CountAsync(cancellationToken);

            result.Add(new TopContributor
            {
                EmployeeId = contributor.AuthorId,
                EmployeeName = contributor.FullName,
                CommentCount = contributor.CommentCount,
                QuestionsAnswered = contributor.QuestionsAnswered,
                EndorsementsReceived = endorsementsReceived
            });
        }

        return result;
    }

    private async Task<List<ActiveThread>> CalculateMostActiveThreadsAsync(
        GetDiscussionAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var threadsQuery = _context.Posts.AsQueryable();

        // Aplica filtros de data aos posts
        if (request.FromDate.HasValue)
        {
            threadsQuery = threadsQuery.Where(p => p.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            threadsQuery = threadsQuery.Where(p => p.CreatedAt <= request.ToDate.Value);
        }

        var activeThreads = await threadsQuery
            .Include(p => p.Comments)
            .AsNoTracking()
            .Select(p => new
            {
                p.Id,
                p.Title,
                CommentCount = p.Comments.Count(),
                ParticipantCount = p.Comments.Select(c => c.AuthorId).Distinct().Count(),
                LastActivityAt = p.Comments.Any() ? p.Comments.Max(c => c.LastActivityAt) : p.CreatedAt
            })
            .Where(p => p.CommentCount > 0) // Apenas posts com comentários
            .OrderByDescending(p => p.CommentCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        return activeThreads.Select(t => new ActiveThread
        {
            PostId = t.Id,
            PostTitle = t.Title,
            CommentCount = t.CommentCount,
            ParticipantCount = t.ParticipantCount,
            LastActivityAt = t.LastActivityAt
        }).ToList();
    }

    [LoggerMessage(EventId = 2101, Level = LogLevel.Information,
        Message = "Gerando analytics para PostId: {PostId}, De: {FromDate}, Até: {ToDate}")]
    private static partial void LogGeneratingAnalytics(ILogger logger, Guid? postId, DateTime? fromDate, DateTime? toDate);

    [LoggerMessage(EventId = 2102, Level = LogLevel.Information,
        Message = "Analytics gerado: {TotalComments} comentários, {TotalThreads} threads, {UnresolvedQuestions} questões não resolvidas")]
    private static partial void LogAnalyticsGenerated(ILogger logger, int totalComments, int totalThreads, int unresolvedQuestions);

    [LoggerMessage(EventId = 2103, Level = LogLevel.Error,
        Message = "Erro ao gerar analytics para PostId: {PostId}")]
    private static partial void LogErrorGeneratingAnalytics(ILogger logger, Exception ex, Guid? postId);
}
