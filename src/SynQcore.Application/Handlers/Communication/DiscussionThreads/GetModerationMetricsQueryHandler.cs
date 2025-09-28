using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Communication;
using System.Globalization;

namespace SynQcore.Application.Handlers.Communication.DiscussionThreads;

public partial class GetModerationMetricsQueryHandler : IRequestHandler<GetModerationMetricsQuery, ModerationMetricsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetModerationMetricsQueryHandler> _logger;

    public GetModerationMetricsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetModerationMetricsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ModerationMetricsDto> Handle(GetModerationMetricsQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        LogGeneratingModerationMetrics(_logger, request.ModeratorId, fromDate, toDate);

        try
        {
            // Query base para comentários moderados no período
            var moderatedCommentsQuery = _context.Comments
                .Where(c => c.ModeratedAt.HasValue &&
                           c.ModeratedAt >= fromDate &&
                           c.ModeratedAt <= toDate);

            // Filtrar por moderador específico se solicitado
            if (request.ModeratorId.HasValue)
            {
                moderatedCommentsQuery = moderatedCommentsQuery
                    .Where(c => c.ModeratedById == request.ModeratorId.Value);
            }

            var moderatedComments = await moderatedCommentsQuery
                .Include(c => c.ModeratedBy)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Estatísticas básicas
            var totalModerated = moderatedComments.Count;
            var averageResponseTime = CalculateAverageResponseTime(moderatedComments);
            var approvalRate = CalculateApprovalRate(moderatedComments);

            // Estatísticas por status
            var statusStats = CalculateStatusStatistics(moderatedComments);

            // Tendências por dia
            var moderationByDay = CalculateModerationByDay(moderatedComments, fromDate, toDate);

            // Moderação por tipo de comentário
            var moderationByType = CalculateModerationByType(moderatedComments);

            // Trends detalhados
            var trends = CalculateModerationTrends(moderatedComments);

            // Top moderadores (apenas se não filtrado por moderador específico)
            var topModerators = new List<ModeratorStats>();
            if (!request.ModeratorId.HasValue)
            {
                topModerators = await CalculateTopModeratorsAsync(fromDate, toDate, cancellationToken);
            }

            // Informações do moderador específico se solicitado
            string? moderatorName = null;
            if (request.ModeratorId.HasValue)
            {
                var moderator = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == request.ModeratorId.Value, cancellationToken);
                moderatorName = moderator?.FullName;
            }

            LogModerationMetricsGenerated(_logger, totalModerated, approvalRate, averageResponseTime);

            return new ModerationMetricsDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                ModeratorId = request.ModeratorId,
                ModeratorName = moderatorName,
                TotalCommentsModerated = totalModerated,
                AverageResponseTime = averageResponseTime,
                ApprovalRate = approvalRate,
                Approved = statusStats.GetValueOrDefault("Approved", 0),
                Flagged = statusStats.GetValueOrDefault("Flagged", 0),
                Hidden = statusStats.GetValueOrDefault("Hidden", 0),
                Rejected = statusStats.GetValueOrDefault("Rejected", 0),
                UnderReview = statusStats.GetValueOrDefault("UnderReview", 0),
                ModerationByDay = moderationByDay,
                ModerationByType = moderationByType,
                Trends = trends,
                TopModerators = topModerators
            };
        }
        catch (Exception ex)
        {
            LogErrorGeneratingModerationMetrics(_logger, ex, request.ModeratorId);
            throw;
        }
    }

    private static int CalculateAverageResponseTime(List<Domain.Entities.Communication.Comment> comments)
    {
        if (comments.Count == 0) return 0;

        var responseTimes = comments
            .Where(c => c.ModeratedAt.HasValue)
            .Select(c => (c.ModeratedAt!.Value - c.CreatedAt).TotalMinutes)
            .Where(t => t >= 0)
            .ToList();

        return responseTimes.Count > 0 ? (int)responseTimes.Average() : 0;
    }

    private static double CalculateApprovalRate(List<Domain.Entities.Communication.Comment> comments)
    {
        if (comments.Count == 0) return 0;

        var approved = comments.Count(c => c.ModerationStatus == Domain.Entities.Communication.ModerationStatus.Approved);
        return (double)approved / comments.Count * 100;
    }

    private static Dictionary<string, int> CalculateStatusStatistics(List<Domain.Entities.Communication.Comment> comments)
    {
        return comments
            .GroupBy(c => c.ModerationStatus.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private static Dictionary<string, int> CalculateModerationByDay(
        List<Domain.Entities.Communication.Comment> comments,
        DateTime fromDate,
        DateTime toDate)
    {
        var result = new Dictionary<string, int>();

        // Inicializa todos os dias no período
        for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
        {
            result[date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)] = 0;
        }

        // Conta moderações por dia
        var moderationsByDay = comments
            .Where(c => c.ModeratedAt.HasValue)
            .GroupBy(c => c.ModeratedAt!.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            .ToDictionary(g => g.Key, g => g.Count());

        // Atualiza com dados reais
        foreach (var kvp in moderationsByDay)
        {
            if (result.ContainsKey(kvp.Key))
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        return result;
    }

    private static Dictionary<string, int> CalculateModerationByType(List<Domain.Entities.Communication.Comment> comments)
    {
        return comments
            .GroupBy(c => c.Type.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private static List<ModerationTrendItem> CalculateModerationTrends(List<Domain.Entities.Communication.Comment> comments)
    {
        return comments
            .Where(c => c.ModeratedAt.HasValue)
            .GroupBy(c => new
            {
                Date = c.ModeratedAt!.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Status = c.ModerationStatus.ToString()
            })
            .Select(g => new ModerationTrendItem
            {
                Date = g.Key.Date,
                Status = g.Key.Status,
                Count = g.Count()
            })
            .OrderBy(t => t.Date)
            .ThenBy(t => t.Status)
            .ToList();
    }

    private async Task<List<ModeratorStats>> CalculateTopModeratorsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken)
    {
        var moderatorStats = await _context.Comments
            .Where(c => c.ModeratedAt.HasValue &&
                       c.ModeratedAt >= fromDate &&
                       c.ModeratedAt <= toDate &&
                       c.ModeratedById.HasValue)
            .GroupBy(c => new { c.ModeratedById, c.ModeratedBy!.FullName })
            .Select(g => new
            {
                ModeratorId = g.Key.ModeratedById!.Value,
                ModeratorName = g.Key.FullName,
                Comments = g.ToList()
            })
            .ToListAsync(cancellationToken);

        return moderatorStats
            .Select(stat => new ModeratorStats
            {
                ModeratorId = stat.ModeratorId,
                ModeratorName = stat.ModeratorName,
                CommentsModerated = stat.Comments.Count,
                AverageResponseTime = CalculateAverageResponseTime(stat.Comments),
                ApprovalRate = CalculateApprovalRate(stat.Comments)
            })
            .OrderByDescending(s => s.CommentsModerated)
            .Take(10)
            .ToList();
    }

    [LoggerMessage(EventId = 2301, Level = LogLevel.Information,
        Message = "Gerando métricas de moderação para ModeratorId: {ModeratorId}, período: {FromDate} - {ToDate}")]
    private static partial void LogGeneratingModerationMetrics(ILogger logger, Guid? moderatorId, DateTime fromDate, DateTime toDate);

    [LoggerMessage(EventId = 2302, Level = LogLevel.Information,
        Message = "Métricas de moderação geradas: {TotalModerated} comentários, taxa de aprovação: {ApprovalRate:F1}%, tempo médio: {AverageTime}min")]
    private static partial void LogModerationMetricsGenerated(ILogger logger, int totalModerated, double approvalRate, int averageTime);

    [LoggerMessage(EventId = 2303, Level = LogLevel.Error,
        Message = "Erro ao gerar métricas de moderação para ModeratorId: {ModeratorId}")]
    private static partial void LogErrorGeneratingModerationMetrics(ILogger logger, Exception ex, Guid? moderatorId);
}

public record GetModerationMetricsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? ModeratorId = null
) : IRequest<ModerationMetricsDto>;