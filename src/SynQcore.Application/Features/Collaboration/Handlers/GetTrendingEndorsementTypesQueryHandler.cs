using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Collaboration.Helpers;
using SynQcore.Application.Features.Collaboration.Queries;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Handlers;

public partial class GetTrendingEndorsementTypesQueryHandler : IRequestHandler<GetTrendingEndorsementTypesQuery, List<EndorsementTypeTrendDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetTrendingEndorsementTypesQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(EventId = 3071, Level = LogLevel.Information,
        Message = "Calculando trending de endorsements para período {StartDate} - {EndDate}")]
    private static partial void LogCalculatingTrends(ILogger logger, DateTime startDate, DateTime endDate);

    [LoggerMessage(EventId = 3072, Level = LogLevel.Information,
        Message = "Trending calculado: {TrendCount} tipos em tendência")]
    private static partial void LogTrendsCalculated(ILogger logger, int trendCount);

    [LoggerMessage(EventId = 3073, Level = LogLevel.Information,
        Message = "Aplicando filtro por departamento para trending: {DepartmentId}")]
    private static partial void LogTrendingDepartmentFilter(ILogger logger, Guid departmentId);

    [LoggerMessage(EventId = 3074, Level = LogLevel.Information,
        Message = "Calculando crescimento comparado ao período anterior")]
    private static partial void LogCalculatingGrowthRate(ILogger logger);

    [LoggerMessage(EventId = 3075, Level = LogLevel.Error,
        Message = "Erro ao calcular trending de endorsements")]
    private static partial void LogTrendingError(ILogger logger, Exception ex);

    public GetTrendingEndorsementTypesQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetTrendingEndorsementTypesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EndorsementTypeTrendDto>> Handle(GetTrendingEndorsementTypesQuery request, CancellationToken cancellationToken)
    {
        LogCalculatingTrends(_logger, request.StartDate, request.EndDate);

        try
        {
            // Query base para endorsements no período
            var endorsementsQuery = _context.Endorsements
                .Where(e => e.EndorsedAt >= request.StartDate && e.EndorsedAt <= request.EndDate);

            // Aplicar filtro de departamento se especificado
            if (request.DepartmentId.HasValue)
            {
                LogTrendingDepartmentFilter(_logger, request.DepartmentId.Value);

                endorsementsQuery = endorsementsQuery.Where(e =>
                    (e.Post != null && e.Post.DepartmentId == request.DepartmentId.Value) ||
                    (e.Comment != null && e.Comment.Post.DepartmentId == request.DepartmentId.Value) ||
                    (e.Endorser.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId.Value)));
            }

            // Contar endorsements por tipo no período atual
            var currentPeriodStats = await endorsementsQuery
                .GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count, cancellationToken);

            var totalCurrentEndorsements = currentPeriodStats.Values.Sum();

            // Calcular período anterior para growth rate
            LogCalculatingGrowthRate(_logger);

            var periodDuration = request.EndDate - request.StartDate;
            var previousPeriodStart = request.StartDate - periodDuration;
            var previousPeriodEnd = request.StartDate;

            var previousEndorsementsQuery = _context.Endorsements
                .Where(e => e.EndorsedAt >= previousPeriodStart && e.EndorsedAt < previousPeriodEnd);

            // Aplicar o mesmo filtro de departamento no período anterior
            if (request.DepartmentId.HasValue)
            {
                previousEndorsementsQuery = previousEndorsementsQuery.Where(e =>
                    (e.Post != null && e.Post.DepartmentId == request.DepartmentId.Value) ||
                    (e.Comment != null && e.Comment.Post.DepartmentId == request.DepartmentId.Value) ||
                    (e.Endorser.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId.Value)));
            }

            var previousPeriodStats = await previousEndorsementsQuery
                .GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count, cancellationToken);

            // Montar resultado com trending data
            var trendingTypes = new List<EndorsementTypeTrendDto>();

            foreach (var (type, currentCount) in currentPeriodStats)
            {
                var typeInfo = EndorsementTypeHelper.GetTypeInfo(type);
                var previousCount = previousPeriodStats.GetValueOrDefault(type, 0);

                // Calcular growth rate
                double growthRate = 0;
                if (previousCount > 0)
                {
                    growthRate = ((double)(currentCount - previousCount) / previousCount) * 100;
                }
                else if (currentCount > 0)
                {
                    growthRate = 100; // 100% crescimento se não havia no período anterior
                }

                var trendDto = new EndorsementTypeTrendDto
                {
                    Type = type,
                    TypeDisplayName = typeInfo.DisplayName,
                    TypeIcon = typeInfo.Icon,
                    Count = currentCount,
                    PercentageOfTotal = totalCurrentEndorsements > 0 ?
                        Math.Round(((double)currentCount / totalCurrentEndorsements) * 100, 2) : 0,
                    GrowthRate = Math.Round(growthRate, 2)
                };

                trendingTypes.Add(trendDto);
            }

            // Ordenar por contagem (mais populares primeiro) e limitar resultado
            var topTrending = trendingTypes
                .OrderByDescending(t => t.Count)
                .ThenByDescending(t => t.GrowthRate)
                .Take(request.TopCount)
                .ToList();

            LogTrendsCalculated(_logger, topTrending.Count);
            return topTrending;
        }
        catch (Exception ex)
        {
            LogTrendingError(_logger, ex);
            throw;
        }
    }
}
