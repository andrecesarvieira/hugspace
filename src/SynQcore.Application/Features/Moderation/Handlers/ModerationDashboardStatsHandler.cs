using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Application.Features.Moderation.Queries;
using SynQcore.Application.Features.Moderation.Utilities;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Moderation.Handlers;

/// <summary>
/// Handler para estatísticas do dashboard de moderação
/// </summary>
public partial class GetModerationDashboardStatsQueryHandler : IRequestHandler<GetModerationDashboardStatsQuery, ModerationDashboardStatsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetModerationDashboardStatsQueryHandler> _logger;

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Calculando estatísticas do dashboard de moderação - Data: {referenceDate}")]
    private static partial void LogCalculatingDashboardStats(ILogger logger, DateTime referenceDate, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Estatísticas calculadas - Total logs hoje: {totalToday}, Críticos: {critical}, Usuários ativos: {activeUsers}")]
    private static partial void LogDashboardStatsCalculated(ILogger logger, int totalToday, int critical, int activeUsers, Exception? exception);

    public GetModerationDashboardStatsQueryHandler(ISynQcoreDbContext context, ILogger<GetModerationDashboardStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ModerationDashboardStatsDto> Handle(GetModerationDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        LogCalculatingDashboardStats(_logger, request.ReferenceDate, null);

        var today = request.ReferenceDate.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var yesterday = today.AddDays(-1);

        // Calcular estatísticas básicas
        var totalLogsToday = await _context.AuditLogs
            .CountAsync(log => log.CreatedAt >= today && log.CreatedAt < today.AddDays(1), cancellationToken);

        var totalLogsThisWeek = await _context.AuditLogs
            .CountAsync(log => log.CreatedAt >= weekStart && log.CreatedAt < today.AddDays(1), cancellationToken);

        var criticalLogsUnresolved = await _context.AuditLogs
            .CountAsync(log => log.Severity == AuditSeverity.Critical &&
                              log.RequiresAttention &&
                              log.ReviewedAt == null, cancellationToken);

        var failedLoginsToday = await _context.AuditLogs
            .CountAsync(log => log.ActionType == AuditActionType.LoginFailed &&
                              log.CreatedAt >= today &&
                              log.CreatedAt < today.AddDays(1), cancellationToken);

        var activeUsersToday = await _context.AuditLogs
            .Where(log => log.CreatedAt >= today &&
                         log.CreatedAt < today.AddDays(1) &&
                         !string.IsNullOrEmpty(log.UserId))
            .Select(log => log.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var securityEventsToday = await _context.AuditLogs
            .CountAsync(log => log.Severity == AuditSeverity.Security &&
                              log.CreatedAt >= today &&
                              log.CreatedAt < today.AddDays(1), cancellationToken);

        // Calcular taxa de crescimento
        var totalLogsYesterday = await _context.AuditLogs
            .CountAsync(log => log.CreatedAt >= yesterday && log.CreatedAt < today, cancellationToken);

        var logGrowthRate = totalLogsYesterday > 0
            ? ((decimal)(totalLogsToday - totalLogsYesterday) / totalLogsYesterday) * 100
            : 0;

        // Logs por severidade
        var logsBySeverity = await _context.AuditLogs
            .Where(log => log.CreatedAt >= today && log.CreatedAt < today.AddDays(1))
            .GroupBy(log => log.Severity)
            .Select(g => new { Severity = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Severity.ToString(), x => x.Count, cancellationToken);

        // Logs por categoria
        var logsByCategory = await _context.AuditLogs
            .Where(log => log.CreatedAt >= today && log.CreatedAt < today.AddDays(1))
            .GroupBy(log => log.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category.ToString(), x => x.Count, cancellationToken);

        // Top 5 IPs mais ativos
        var topActiveIps = await _context.AuditLogs
            .Where(log => log.CreatedAt >= today &&
                         log.CreatedAt < today.AddDays(1) &&
                         !string.IsNullOrEmpty(log.ClientIpAddress))
            .GroupBy(log => log.ClientIpAddress)
            .Select(g => new IpActivityDto
            {
                IpAddress = g.Key!,
                RequestCount = g.Count(),
                FailureCount = g.Count(log => !log.Success),
                LastActivity = g.Max(log => log.CreatedAt)
            })
            .OrderByDescending(ip => ip.RequestCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        // Calcular taxa de falha e marcar IPs suspeitos
        foreach (var ip in topActiveIps)
        {
            ip.FailureRate = ip.RequestCount > 0 ? ((decimal)ip.FailureCount / ip.RequestCount) * 100 : 0;
            ip.IsSuspicious = ip.FailureRate > 25 || ip.FailureCount > 10; // Critérios configuráveis
        }

        // Últimas ações críticas
        var recentCriticalActions = await _context.AuditLogs
            .Where(log => log.Severity == AuditSeverity.Critical &&
                         log.CreatedAt >= today.AddDays(-7)) // Última semana
            .OrderByDescending(log => log.CreatedAt)
            .Take(10)
            .Select(log => ModerationMappingUtilities.MapToModerationDto(log))
            .ToListAsync(cancellationToken);

        var result = new ModerationDashboardStatsDto
        {
            TotalLogsToday = totalLogsToday,
            TotalLogsThisWeek = totalLogsThisWeek,
            CriticalLogsUnresolved = criticalLogsUnresolved,
            FailedLoginsToday = failedLoginsToday,
            ActiveUsersToday = activeUsersToday,
            SecurityEventsToday = securityEventsToday,
            LogGrowthRate = logGrowthRate,
            LogsBySeverity = logsBySeverity,
            LogsByCategory = logsByCategory,
            TopActiveIps = topActiveIps,
            RecentCriticalActions = recentCriticalActions
        };

        LogDashboardStatsCalculated(_logger, totalLogsToday, criticalLogsUnresolved, activeUsersToday, null);

        return result;
    }
}
