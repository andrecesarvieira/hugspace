/*
 * SynQcore - Corporate Social Network
 *
 * Security Monitoring Service Implementation - Real-time threat detection and response
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Services;
using SynQcore.Domain.Entities;
using SynQcore.Infrastructure.Data;

namespace SynQcore.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de monitoramento de segurança
/// </summary>
public partial class SecurityMonitoringService : ISecurityMonitoringService
{
    private readonly SynQcoreDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SecurityMonitoringService> _logger;
    private readonly IAuditService _auditService;

    // Cache para tracking de tentativas suspeitas
    private static readonly ConcurrentDictionary<string, List<DateTime>> _loginAttempts = new();
    private static readonly ConcurrentDictionary<string, List<DateTime>> _rateLimitViolations = new();
    private static readonly ConcurrentDictionary<string, List<DateTime>> _inputAttacks = new();
    private static readonly ConcurrentDictionary<string, DateTime> _blockedIps = new();
    private static readonly ConcurrentDictionary<Guid, SecurityAlert> _activeAlerts = new();

    // Configurações de segurança
    private const int MAX_LOGIN_ATTEMPTS_PER_HOUR = 5;
    private const int MAX_RATE_LIMIT_VIOLATIONS_PER_HOUR = 10;
    private const int MAX_INPUT_ATTACKS_PER_HOUR = 3;
    private const int SUSPICIOUS_ACTIVITY_THRESHOLD = 15;
    private readonly TimeSpan DEFAULT_BLOCK_DURATION = TimeSpan.FromHours(1);
    private readonly TimeSpan CACHE_EXPIRY = TimeSpan.FromHours(24);

    public SecurityMonitoringService(
        SynQcoreDbContext context,
        IMemoryCache cache,
        ILogger<SecurityMonitoringService> logger,
        IAuditService auditService)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _auditService = auditService;
    }

    public async Task MonitorLoginAttemptsAsync(string email, string ipAddress, bool success, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!success)
            {
                var attempts = _loginAttempts.GetOrAdd(ipAddress, _ => new List<DateTime>());
                var now = DateTime.UtcNow;

                lock (attempts)
                {
                    // Remove tentativas antigas (mais de 1 hora)
                    attempts.RemoveAll(a => now - a > TimeSpan.FromHours(1));
                    attempts.Add(now);
                }

                LogLoginAttemptMonitored(_logger, ipAddress, email, attempts.Count, null);

                // Verifica se excedeu o limite
                if (attempts.Count >= MAX_LOGIN_ATTEMPTS_PER_HOUR)
                {
                    await CreateSecurityAlert(
                        "Suspicious Login Activity",
                        $"Multiple failed login attempts from IP {ipAddress} for email {email}",
                        "High",
                        ipAddress,
                        null,
                        new Dictionary<string, object> { { "attemptCount", attempts.Count }, { "email", email } });

                    await BlockIpTemporarilyAsync(ipAddress, DEFAULT_BLOCK_DURATION, $"Excessive login attempts for {email}", cancellationToken);

                    LogSecurityThreatDetected(_logger, "Login Brute Force", ipAddress, attempts.Count, null);
                }
            }
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "LoginAttempts", ex.Message, ex);
        }
    }

    public async Task MonitorRateLimitViolationsAsync(string ipAddress, string endpoint, string? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var violations = _rateLimitViolations.GetOrAdd(ipAddress, _ => new List<DateTime>());
            var now = DateTime.UtcNow;

            lock (violations)
            {
                violations.RemoveAll(v => now - v > TimeSpan.FromHours(1));
                violations.Add(now);
            }

            await _auditService.LogRateLimitViolationAsync(ipAddress, endpoint, userId, cancellationToken);

            LogRateLimitViolationMonitored(_logger, ipAddress, endpoint, violations.Count, null);

            if (violations.Count >= MAX_RATE_LIMIT_VIOLATIONS_PER_HOUR)
            {
                await CreateSecurityAlert(
                    "Rate Limit Abuse",
                    $"Excessive rate limit violations from IP {ipAddress}",
                    "Medium",
                    ipAddress,
                    userId,
                    new Dictionary<string, object> { { "violationCount", violations.Count }, { "endpoint", endpoint } });

                await BlockIpTemporarilyAsync(ipAddress, DEFAULT_BLOCK_DURATION, "Excessive rate limit violations", cancellationToken);

                LogSecurityThreatDetected(_logger, "Rate Limit Abuse", ipAddress, violations.Count, null);
            }
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "RateLimitViolations", ex.Message, ex);
        }
    }

    public async Task MonitorInputAttacksAsync(string inputType, string threatType, string ipAddress, string? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var attacks = _inputAttacks.GetOrAdd(ipAddress, _ => new List<DateTime>());
            var now = DateTime.UtcNow;

            lock (attacks)
            {
                attacks.RemoveAll(a => now - a > TimeSpan.FromHours(1));
                attacks.Add(now);
            }

            await _auditService.LogInputSanitizationAsync(inputType, threatType, ipAddress, userId, cancellationToken);

            LogInputAttackMonitored(_logger, ipAddress, threatType, inputType, attacks.Count, null);

            if (attacks.Count >= MAX_INPUT_ATTACKS_PER_HOUR)
            {
                await CreateSecurityAlert(
                    "Input Attack Detected",
                    $"Multiple {threatType} attacks from IP {ipAddress}",
                    "High",
                    ipAddress,
                    userId,
                    new Dictionary<string, object> { { "attackCount", attacks.Count }, { "threatType", threatType }, { "inputType", inputType } });

                await BlockIpTemporarilyAsync(ipAddress, TimeSpan.FromHours(6), $"Multiple {threatType} attacks", cancellationToken);

                LogSecurityThreatDetected(_logger, threatType, ipAddress, attacks.Count, null);
            }
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "InputAttacks", ex.Message, ex);
        }
    }

    public async Task<bool> ShouldBlockIpAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verifica se está na lista de bloqueios temporários
            if (_blockedIps.TryGetValue(ipAddress, out var blockExpiry))
            {
                if (DateTime.UtcNow < blockExpiry)
                {
                    return true;
                }
                else
                {
                    // Remove IP expirado
                    _blockedIps.TryRemove(ipAddress, out _);
                }
            }

            // Verifica cache para decisão rápida
            var cacheKey = $"should_block_{ipAddress}";
            if (_cache.TryGetValue(cacheKey, out bool shouldBlock))
            {
                return shouldBlock;
            }

            // Verifica histórico de atividade suspeita nas últimas 24 horas
            var recentViolations = await _context.AuditLogs
                .Where(a => a.ClientIpAddress == ipAddress &&
                           a.CreatedAt >= DateTime.UtcNow.AddHours(-24) &&
                           (a.ActionType == AuditActionType.SecurityViolation ||
                            a.ActionType == AuditActionType.RateLimitExceeded ||
                            a.ActionType == AuditActionType.SuspiciousActivity))
                .CountAsync(cancellationToken);

            shouldBlock = recentViolations >= SUSPICIOUS_ACTIVITY_THRESHOLD;

            // Cache a decisão por 15 minutos
            _cache.Set(cacheKey, shouldBlock, TimeSpan.FromMinutes(15));

            if (shouldBlock)
            {
                LogIpBlockDecision(_logger, ipAddress, recentViolations, null);
            }

            return shouldBlock;
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "ShouldBlockIp", ex.Message, ex);
            return false; // Em caso de erro, não bloqueia para não impactar usuários legítimos
        }
    }

    public async Task BlockIpTemporarilyAsync(string ipAddress, TimeSpan duration, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var blockUntil = DateTime.UtcNow.Add(duration);
            _blockedIps.AddOrUpdate(ipAddress, blockUntil, (key, oldValue) => blockUntil);

            await _auditService.LogSuspiciousActivityAsync("IP Blocked", $"IP {ipAddress} blocked for {duration.TotalMinutes} minutes. Reason: {reason}", ipAddress, null, cancellationToken);

            LogIpBlocked(_logger, ipAddress, duration.TotalMinutes, reason, null);

            // Remove do cache de decisão para forçar reavaliação
            _cache.Remove($"should_block_{ipAddress}");
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "BlockIp", ex.Message, ex);
        }
    }

    public async Task UnblockIpAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            _blockedIps.TryRemove(ipAddress, out _);
            _cache.Remove($"should_block_{ipAddress}");

            await _auditService.LogSecurityEventAsync("IP Unblocked", $"IP {ipAddress} manually unblocked", null, AuditSeverity.Information, cancellationToken);

            LogIpUnblocked(_logger, ipAddress, null);
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "UnblockIp", ex.Message, ex);
        }
    }

    public async Task<SecurityMetrics> GetSecurityMetricsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var yesterday = now.AddHours(-24);

            var failedLogins = await _context.AuditLogs
                .Where(a => a.ActionType == AuditActionType.LoginFailed && a.CreatedAt >= yesterday)
                .CountAsync(cancellationToken);

            var rateLimitViolations = await _context.AuditLogs
                .Where(a => a.ActionType == AuditActionType.RateLimitExceeded && a.CreatedAt >= yesterday)
                .CountAsync(cancellationToken);

            var inputAttacks = await _context.AuditLogs
                .Where(a => a.ActionType == AuditActionType.SecurityViolation &&
                           a.ResourceType == "InputSanitization" &&
                           a.CreatedAt >= yesterday)
                .CountAsync(cancellationToken);

            var topAttackingIps = await _context.AuditLogs
                .Where(a => a.ClientIpAddress != null &&
                           a.CreatedAt >= yesterday &&
                           (a.ActionType == AuditActionType.SecurityViolation ||
                            a.ActionType == AuditActionType.RateLimitExceeded))
                .GroupBy(a => a.ClientIpAddress)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key!)
                .ToListAsync(cancellationToken);

            var mostTargetedEndpoints = await _context.AuditLogs
                .Where(a => a.ResourceId != null &&
                           a.CreatedAt >= yesterday &&
                           a.ActionType == AuditActionType.RateLimitExceeded)
                .GroupBy(a => a.ResourceId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key!)
                .ToListAsync(cancellationToken);

            return new SecurityMetrics
            {
                FailedLoginsLast24Hours = failedLogins,
                RateLimitViolationsLast24Hours = rateLimitViolations,
                InputAttacksDetectedLast24Hours = inputAttacks,
                BlockedIpsCount = _blockedIps.Count,
                ActiveSecurityAlertsCount = _activeAlerts.Count(a => a.Value.IsActive),
                TopAttackingIps = topAttackingIps,
                MostTargetedEndpoints = mostTargetedEndpoints,
                LastUpdated = now
            };
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "GetSecurityMetrics", ex.Message, ex);
            return new SecurityMetrics();
        }
    }

    public async Task<List<SecurityAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask; // Para compliance async
            return _activeAlerts.Values.Where(a => a.IsActive).OrderByDescending(a => a.CreatedAt).ToList();
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "GetActiveAlerts", ex.Message, ex);
            return new List<SecurityAlert>();
        }
    }

    public async Task DetectSuspiciousPatterns(CancellationToken cancellationToken = default)
    {
        try
        {
            // Detecta padrões suspeitos nos últimos dados
            var now = DateTime.UtcNow;
            var lastHour = now.AddHours(-1);

            // Detecta IPs com múltiplos tipos de ataques
            var suspiciousIps = await _context.AuditLogs
                .Where(a => a.ClientIpAddress != null &&
                           a.CreatedAt >= lastHour &&
                           (a.ActionType == AuditActionType.SecurityViolation ||
                            a.ActionType == AuditActionType.RateLimitExceeded ||
                            a.ActionType == AuditActionType.LoginFailed))
                .GroupBy(a => a.ClientIpAddress)
                .Where(g => g.Select(x => x.ActionType).Distinct().Count() >= 2) // Múltiplos tipos de violação
                .Select(g => new { IpAddress = g.Key!, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var suspiciousIp in suspiciousIps)
            {
                await CreateSecurityAlert(
                    "Multi-Vector Attack",
                    $"IP {suspiciousIp.IpAddress} showing multiple attack vectors in the last hour",
                    "Critical",
                    suspiciousIp.IpAddress,
                    null,
                    new Dictionary<string, object>
                    {
                        { "attackCount", suspiciousIp.Count },
                        { "timeWindow", "1 hour" },
                        { "attackTypes", "multiple" }
                    });

                LogSuspiciousPatternDetected(_logger, "Multi-Vector Attack", suspiciousIp.IpAddress!, suspiciousIp.Count, null);
            }
        }
        catch (Exception ex)
        {
            LogMonitoringError(_logger, "DetectSuspiciousPatterns", ex.Message, ex);
        }
    }

    private async Task CreateSecurityAlert(string type, string message, string severity, string? ipAddress, string? userId, Dictionary<string, object> metadata)
    {
        var alert = new SecurityAlert
        {
            Type = type,
            Message = message,
            Severity = severity,
            IpAddress = ipAddress,
            UserId = userId,
            Metadata = metadata
        };

        _activeAlerts.AddOrUpdate(alert.Id, alert, (key, oldValue) => alert);

        await _auditService.LogSecurityEventAsync(type, message, userId != null ? Guid.Parse(userId) : null, AuditSeverity.Warning);

        LogSecurityAlertCreated(_logger, type, severity, ipAddress ?? "unknown", null);
    }

    // LoggerMessage delegates para performance
    [LoggerMessage(EventId = 6001, Level = LogLevel.Information,
        Message = "Login attempt monitored - IP: {IpAddress} | Email: {Email} | Attempts: {AttemptCount}")]
    private static partial void LogLoginAttemptMonitored(ILogger logger, string ipAddress, string email, int attemptCount, Exception? exception);

    [LoggerMessage(EventId = 6002, Level = LogLevel.Warning,
        Message = "Security threat detected - Type: {ThreatType} | IP: {IpAddress} | Count: {Count}")]
    private static partial void LogSecurityThreatDetected(ILogger logger, string threatType, string ipAddress, int count, Exception? exception);

    [LoggerMessage(EventId = 6003, Level = LogLevel.Information,
        Message = "Rate limit violation monitored - IP: {IpAddress} | Endpoint: {Endpoint} | Violations: {ViolationCount}")]
    private static partial void LogRateLimitViolationMonitored(ILogger logger, string ipAddress, string endpoint, int violationCount, Exception? exception);

    [LoggerMessage(EventId = 6004, Level = LogLevel.Information,
        Message = "Input attack monitored - IP: {IpAddress} | Threat: {ThreatType} | Input: {InputType} | Attacks: {AttackCount}")]
    private static partial void LogInputAttackMonitored(ILogger logger, string ipAddress, string threatType, string inputType, int attackCount, Exception? exception);

    [LoggerMessage(EventId = 6005, Level = LogLevel.Warning,
        Message = "IP blocked temporarily - IP: {IpAddress} | Duration: {DurationMinutes} minutes | Reason: {Reason}")]
    private static partial void LogIpBlocked(ILogger logger, string ipAddress, double durationMinutes, string reason, Exception? exception);

    [LoggerMessage(EventId = 6006, Level = LogLevel.Information,
        Message = "IP unblocked - IP: {IpAddress}")]
    private static partial void LogIpUnblocked(ILogger logger, string ipAddress, Exception? exception);

    [LoggerMessage(EventId = 6007, Level = LogLevel.Information,
        Message = "IP block decision - IP: {IpAddress} | Recent violations: {ViolationCount}")]
    private static partial void LogIpBlockDecision(ILogger logger, string ipAddress, int violationCount, Exception? exception);

    [LoggerMessage(EventId = 6008, Level = LogLevel.Information,
        Message = "Security alert created - Type: {Type} | Severity: {Severity} | IP: {IpAddress}")]
    private static partial void LogSecurityAlertCreated(ILogger logger, string type, string severity, string ipAddress, Exception? exception);

    [LoggerMessage(EventId = 6009, Level = LogLevel.Warning,
        Message = "Suspicious pattern detected - Pattern: {Pattern} | IP: {IpAddress} | Count: {Count}")]
    private static partial void LogSuspiciousPatternDetected(ILogger logger, string pattern, string ipAddress, int count, Exception? exception);

    [LoggerMessage(EventId = 6010, Level = LogLevel.Error,
        Message = "Security monitoring error - Operation: {Operation} | Error: {ErrorMessage}")]
    private static partial void LogMonitoringError(ILogger logger, string operation, string errorMessage, Exception? exception);
}
