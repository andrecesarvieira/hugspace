/*
 * SynQcore - Corporate Social Network API
 *
 * Advanced IP Rate Limiting Service - Rate limiting por IP com detecção de padrões suspeitos
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SynQcore.Application.Services;
using System.Collections.Concurrent;
using System.Net;

namespace SynQcore.Infrastructure.Services;

/// <summary>
/// Serviço avançado de rate limiting por IP com inteligência de padrões
/// </summary>
public partial class AdvancedRateLimitingService : IAdvancedRateLimitingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<AdvancedRateLimitingService> _logger;
    private readonly ISecurityMonitoringService _securityMonitoring;
    private readonly AdvancedRateLimitingOptions _options;

    // Tracking de requisições por IP
    private static readonly ConcurrentDictionary<string, List<DateTime>> _ipRequests = new();
    private static readonly ConcurrentDictionary<string, List<DateTime>> _endpointRequests = new();
    private static readonly ConcurrentDictionary<string, DateTime> _temporaryBlocks = new();
    private static readonly ConcurrentDictionary<string, int> _violationCounts = new();

    public AdvancedRateLimitingService(
        IMemoryCache cache,
        ILogger<AdvancedRateLimitingService> logger,
        ISecurityMonitoringService securityMonitoring,
        IOptions<AdvancedRateLimitingOptions> options)
    {
        _cache = cache;
        _logger = logger;
        _securityMonitoring = securityMonitoring;
        _options = options.Value;
    }

    public async Task<RateLimitResult> CheckRateLimitAsync(string ipAddress, string endpoint, string? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar se IP está temporariamente bloqueado
            if (IsTemporarilyBlocked(ipAddress))
            {
                LogRateLimitBlocked(_logger, ipAddress, endpoint, "Temporary block active", null);
                return new RateLimitResult
                {
                    IsAllowed = false,
                    Reason = "IP temporarily blocked due to excessive violations",
                    RemainingRequests = 0,
                    ResetTime = _temporaryBlocks.GetValueOrDefault(ipAddress, DateTime.UtcNow.AddMinutes(15))
                };
            }

            // Verificar rate limit por IP
            var ipLimit = GetIpLimitForEndpoint(endpoint);
            var ipResult = await CheckIpRateLimit(ipAddress, endpoint, ipLimit, cancellationToken);

            if (!ipResult.IsAllowed)
            {
                await HandleRateLimitViolation(ipAddress, endpoint, userId, "IP limit exceeded", cancellationToken);
                return ipResult;
            }

            // Verificar rate limit por endpoint
            var endpointLimit = GetEndpointLimit(endpoint);
            var endpointResult = await CheckEndpointRateLimit(endpoint, endpointLimit, cancellationToken);

            if (!endpointResult.IsAllowed)
            {
                await HandleRateLimitViolation(ipAddress, endpoint, userId, "Endpoint limit exceeded", cancellationToken);
                return endpointResult;
            }

            // Verificar padrões suspeitos
            await DetectSuspiciousPatternsAsync(ipAddress, endpoint, cancellationToken);

            LogRateLimitAllowed(_logger, ipAddress, endpoint, ipResult.RemainingRequests, null);

            return ipResult;
        }
        catch (Exception ex)
        {
            LogRateLimitError(_logger, ipAddress, endpoint, ex.Message, ex);
            // Em caso de erro, permite a requisição para não impactar o usuário
            return new RateLimitResult { IsAllowed = true, RemainingRequests = 1000, ResetTime = DateTime.UtcNow.AddMinutes(1) };
        }
    }

    public async Task RecordRequestAsync(string ipAddress, string endpoint, bool successful, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var cacheKey = $"request_record_{ipAddress}_{endpoint}";

            // Registrar requisição para este IP
            var ipRequests = _ipRequests.GetOrAdd(ipAddress, _ => new List<DateTime>());
            lock (ipRequests)
            {
                ipRequests.RemoveAll(r => now - r > TimeSpan.FromMinutes(_options.TimeWindowMinutes));
                ipRequests.Add(now);
            }

            // Registrar requisição para este endpoint
            var endpointKey = $"endpoint_{endpoint}";
            var endpointRequests = _endpointRequests.GetOrAdd(endpointKey, _ => new List<DateTime>());
            lock (endpointRequests)
            {
                endpointRequests.RemoveAll(r => now - r > TimeSpan.FromMinutes(_options.TimeWindowMinutes));
                endpointRequests.Add(now);
            }

            // Cache da métrica para dashboard
            _cache.Set(cacheKey, new RequestMetric
            {
                IpAddress = ipAddress,
                Endpoint = endpoint,
                Successful = successful,
                Timestamp = now
            }, TimeSpan.FromMinutes(_options.TimeWindowMinutes));

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            LogRecordRequestError(_logger, ipAddress, endpoint, ex.Message, ex);
        }
    }

    public async Task<List<string>> GetBlockedIpsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;
            var now = DateTime.UtcNow;

            return _temporaryBlocks
                .Where(kvp => kvp.Value > now)
                .Select(kvp => kvp.Key)
                .ToList();
        }
        catch (Exception ex)
        {
            LogGetBlockedIpsError(_logger, ex.Message, ex);
            return new List<string>();
        }
    }

    public async Task UnblockIpAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            _temporaryBlocks.TryRemove(ipAddress, out _);
            _violationCounts.TryRemove(ipAddress, out _);

            // Limpar cache relacionado
            _cache.Remove($"rate_limit_{ipAddress}");

            LogIpUnblocked(_logger, ipAddress, null);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            LogUnblockIpError(_logger, ipAddress, ex.Message, ex);
        }
    }

    public async Task<RateLimitStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-_options.TimeWindowMinutes);

            // Estatísticas de IPs únicos
            var uniqueIps = _ipRequests.Keys.Count;
            var blockedIps = _temporaryBlocks.Count(kvp => kvp.Value > now);

            // Top IPs por volume de requisições
            var topIpsByVolume = _ipRequests
                .Select(kvp => new
                {
                    IpAddress = kvp.Key,
                    RequestCount = kvp.Value.Count(r => r >= windowStart)
                })
                .Where(x => x.RequestCount > 0)
                .OrderByDescending(x => x.RequestCount)
                .Take(10)
                .ToDictionary(x => x.IpAddress, x => x.RequestCount);

            // Top endpoints por volume
            var topEndpointsByVolume = _endpointRequests
                .Select(kvp => new
                {
                    Endpoint = kvp.Key.Replace("endpoint_", ""),
                    RequestCount = kvp.Value.Count(r => r >= windowStart)
                })
                .Where(x => x.RequestCount > 0)
                .OrderByDescending(x => x.RequestCount)
                .Take(10)
                .ToDictionary(x => x.Endpoint, x => x.RequestCount);

            // IPs com mais violações
            var topViolatingIps = _violationCounts
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return new RateLimitStatistics
            {
                UniqueIpsInWindow = uniqueIps,
                BlockedIpsCount = blockedIps,
                TotalRequestsInWindow = _ipRequests.Values.Sum(list => list.Count(r => r >= windowStart)),
                TopIpsByVolume = topIpsByVolume,
                TopEndpointsByVolume = topEndpointsByVolume,
                TopViolatingIps = topViolatingIps,
                GeneratedAt = now,
                TimeWindowMinutes = _options.TimeWindowMinutes
            };
        }
        catch (Exception ex)
        {
            LogGetStatisticsError(_logger, ex.Message, ex);
            return new RateLimitStatistics();
        }
    }

    private Task<RateLimitResult> CheckIpRateLimit(string ipAddress, string endpoint, int limit, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddMinutes(-_options.TimeWindowMinutes);

        var requests = _ipRequests.GetOrAdd(ipAddress, _ => new List<DateTime>());

        int currentCount;
        lock (requests)
        {
            // Remover requisições antigas
            requests.RemoveAll(r => r < windowStart);
            currentCount = requests.Count;
        }

        var remaining = Math.Max(0, limit - currentCount);
        var resetTime = requests.Count > 0 ? requests.Min().AddMinutes(_options.TimeWindowMinutes) : now.AddMinutes(_options.TimeWindowMinutes);

        return Task.FromResult(new RateLimitResult
        {
            IsAllowed = currentCount < limit,
            RemainingRequests = remaining,
            ResetTime = resetTime,
            Reason = currentCount >= limit ? $"IP rate limit exceeded: {currentCount}/{limit}" : null
        });
    }

    private Task<RateLimitResult> CheckEndpointRateLimit(string endpoint, int limit, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddMinutes(-_options.TimeWindowMinutes);
        var endpointKey = $"endpoint_{endpoint}";

        var requests = _endpointRequests.GetOrAdd(endpointKey, _ => new List<DateTime>());

        int currentCount;
        lock (requests)
        {
            requests.RemoveAll(r => r < windowStart);
            currentCount = requests.Count;
        }

        var remaining = Math.Max(0, limit - currentCount);
        var resetTime = requests.Count > 0 ? requests.Min().AddMinutes(_options.TimeWindowMinutes) : now.AddMinutes(_options.TimeWindowMinutes);

        return Task.FromResult(new RateLimitResult
        {
            IsAllowed = currentCount < limit,
            RemainingRequests = remaining,
            ResetTime = resetTime,
            Reason = currentCount >= limit ? $"Endpoint rate limit exceeded: {currentCount}/{limit}" : null
        });
    }

    private async Task HandleRateLimitViolation(string ipAddress, string endpoint, string? userId, string reason, CancellationToken cancellationToken)
    {
        // Incrementar contador de violações
        var violations = _violationCounts.AddOrUpdate(ipAddress, 1, (key, oldValue) => oldValue + 1);

        // Notificar sistema de monitoramento
        await _securityMonitoring.MonitorRateLimitViolationsAsync(ipAddress, endpoint, userId, cancellationToken);

        // Bloquear temporariamente se exceder limite de violações
        if (violations >= _options.MaxViolationsBeforeBlock)
        {
            var blockDuration = TimeSpan.FromMinutes(_options.TemporaryBlockDurationMinutes);
            var blockUntil = DateTime.UtcNow.Add(blockDuration);

            _temporaryBlocks.AddOrUpdate(ipAddress, blockUntil, (key, oldValue) => blockUntil);

            LogIpTemporarilyBlocked(_logger, ipAddress, violations, blockDuration.TotalMinutes, null);
        }

        LogRateLimitViolation(_logger, ipAddress, endpoint, reason, violations, null);
    }

    private async Task DetectSuspiciousPatternsAsync(string ipAddress, string endpoint, CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var recentRequests = _ipRequests.GetValueOrDefault(ipAddress, new List<DateTime>())
                .Where(r => now - r <= TimeSpan.FromMinutes(5))
                .ToList();

            // Detectar rajadas de requisições (muitas requisições em pouco tempo)
            if (recentRequests.Count >= _options.BurstDetectionThreshold)
            {
                await _securityMonitoring.MonitorInputAttacksAsync("RateLimit", "RequestBurst", ipAddress, null, cancellationToken);
                LogSuspiciousBurstDetected(_logger, ipAddress, recentRequests.Count, null);
            }

            // Detectar padrões de timing suspeitos (intervalos muito regulares)
            if (recentRequests.Count >= 10)
            {
                var intervals = recentRequests
                    .OrderBy(r => r)
                    .Zip(recentRequests.Skip(1).OrderBy(r => r), (first, second) => (second - first).TotalMilliseconds)
                    .ToList();

                var averageInterval = intervals.Average();
                var variance = intervals.Select(i => Math.Pow(i - averageInterval, 2)).Average();
                var standardDeviation = Math.Sqrt(variance);

                // Se o desvio padrão for muito baixo, pode ser um bot
                if (standardDeviation < _options.BotDetectionVarianceThreshold && averageInterval < 1000)
                {
                    await _securityMonitoring.MonitorInputAttacksAsync("RateLimit", "BotPattern", ipAddress, null, cancellationToken);
                    LogSuspiciousBotPatternDetected(_logger, ipAddress, standardDeviation, averageInterval, null);
                }
            }
        }
        catch (Exception ex)
        {
            LogSuspiciousPatternDetectionError(_logger, ipAddress, ex.Message, ex);
        }
    }

    private static bool IsTemporarilyBlocked(string ipAddress)
    {
        if (!_temporaryBlocks.TryGetValue(ipAddress, out var blockUntil))
            return false;

        if (DateTime.UtcNow >= blockUntil)
        {
            _temporaryBlocks.TryRemove(ipAddress, out _);
            return false;
        }

        return true;
    }

    private int GetIpLimitForEndpoint(string endpoint)
    {
        // Rate limits diferenciados por tipo de endpoint
        return endpoint.ToLowerInvariant() switch
        {
            var path when path.Contains("/auth/") => _options.AuthEndpointLimit,
            var path when path.Contains("/admin/") => _options.AdminEndpointLimit,
            var path when path.Contains("/upload") => _options.UploadEndpointLimit,
            var path when path.Contains("/search") => _options.SearchEndpointLimit,
            _ => _options.DefaultIpLimit
        };
    }

    private int GetEndpointLimit(string endpoint)
    {
        return endpoint.ToLowerInvariant() switch
        {
            var path when path.Contains("/auth/") => _options.AuthEndpointLimit * 10, // Limite global maior
            var path when path.Contains("/admin/") => _options.AdminEndpointLimit * 5,
            var path when path.Contains("/upload") => _options.UploadEndpointLimit * 20,
            var path when path.Contains("/search") => _options.SearchEndpointLimit * 50,
            _ => _options.DefaultIpLimit * 100
        };
    }

    // LoggerMessage delegates para performance
    [LoggerMessage(EventId = 7001, Level = LogLevel.Information,
        Message = "Rate limit allowed - IP: {IpAddress} | Endpoint: {Endpoint} | Remaining: {Remaining}")]
    private static partial void LogRateLimitAllowed(ILogger logger, string ipAddress, string endpoint, int remaining, Exception? exception);

    [LoggerMessage(EventId = 7002, Level = LogLevel.Warning,
        Message = "Rate limit blocked - IP: {IpAddress} | Endpoint: {Endpoint} | Reason: {Reason}")]
    private static partial void LogRateLimitBlocked(ILogger logger, string ipAddress, string endpoint, string reason, Exception? exception);

    [LoggerMessage(EventId = 7003, Level = LogLevel.Warning,
        Message = "Rate limit violation - IP: {IpAddress} | Endpoint: {Endpoint} | Reason: {Reason} | Violations: {ViolationCount}")]
    private static partial void LogRateLimitViolation(ILogger logger, string ipAddress, string endpoint, string reason, int violationCount, Exception? exception);

    [LoggerMessage(EventId = 7004, Level = LogLevel.Warning,
        Message = "IP temporarily blocked - IP: {IpAddress} | Violations: {Violations} | Duration: {DurationMinutes} minutes")]
    private static partial void LogIpTemporarilyBlocked(ILogger logger, string ipAddress, int violations, double durationMinutes, Exception? exception);

    [LoggerMessage(EventId = 7005, Level = LogLevel.Information,
        Message = "IP unblocked - IP: {IpAddress}")]
    private static partial void LogIpUnblocked(ILogger logger, string ipAddress, Exception? exception);

    [LoggerMessage(EventId = 7006, Level = LogLevel.Warning,
        Message = "Suspicious burst detected - IP: {IpAddress} | Requests in 5min: {RequestCount}")]
    private static partial void LogSuspiciousBurstDetected(ILogger logger, string ipAddress, int requestCount, Exception? exception);

    [LoggerMessage(EventId = 7007, Level = LogLevel.Warning,
        Message = "Suspicious bot pattern detected - IP: {IpAddress} | StdDev: {StandardDeviation} | AvgInterval: {AverageInterval}ms")]
    private static partial void LogSuspiciousBotPatternDetected(ILogger logger, string ipAddress, double standardDeviation, double averageInterval, Exception? exception);

    [LoggerMessage(EventId = 7008, Level = LogLevel.Error,
        Message = "Rate limit error - IP: {IpAddress} | Endpoint: {Endpoint} | Error: {ErrorMessage}")]
    private static partial void LogRateLimitError(ILogger logger, string ipAddress, string endpoint, string errorMessage, Exception? exception);

    [LoggerMessage(EventId = 7009, Level = LogLevel.Error,
        Message = "Record request error - IP: {IpAddress} | Endpoint: {Endpoint} | Error: {ErrorMessage}")]
    private static partial void LogRecordRequestError(ILogger logger, string ipAddress, string endpoint, string errorMessage, Exception? exception);

    [LoggerMessage(EventId = 7010, Level = LogLevel.Error,
        Message = "Get blocked IPs error - Error: {ErrorMessage}")]
    private static partial void LogGetBlockedIpsError(ILogger logger, string errorMessage, Exception? exception);

    [LoggerMessage(EventId = 7011, Level = LogLevel.Error,
        Message = "Unblock IP error - IP: {IpAddress} | Error: {ErrorMessage}")]
    private static partial void LogUnblockIpError(ILogger logger, string ipAddress, string errorMessage, Exception? exception);

    [LoggerMessage(EventId = 7012, Level = LogLevel.Error,
        Message = "Get statistics error - Error: {ErrorMessage}")]
    private static partial void LogGetStatisticsError(ILogger logger, string errorMessage, Exception? exception);

    [LoggerMessage(EventId = 7013, Level = LogLevel.Error,
        Message = "Suspicious pattern detection error - IP: {IpAddress} | Error: {ErrorMessage}")]
    private static partial void LogSuspiciousPatternDetectionError(ILogger logger, string ipAddress, string errorMessage, Exception? exception);
}

/// <summary>
/// Opções de configuração para rate limiting avançado
/// </summary>
public class AdvancedRateLimitingOptions
{
    public const string SectionName = "AdvancedRateLimiting";

    /// <summary>
    /// Limite padrão de requisições por IP por janela de tempo
    /// </summary>
    public int DefaultIpLimit { get; set; } = 100;

    /// <summary>
    /// Limite para endpoints de autenticação
    /// </summary>
    public int AuthEndpointLimit { get; set; } = 10;

    /// <summary>
    /// Limite para endpoints administrativos
    /// </summary>
    public int AdminEndpointLimit { get; set; } = 50;

    /// <summary>
    /// Limite para endpoints de upload
    /// </summary>
    public int UploadEndpointLimit { get; set; } = 20;

    /// <summary>
    /// Limite para endpoints de busca
    /// </summary>
    public int SearchEndpointLimit { get; set; } = 30;

    /// <summary>
    /// Janela de tempo em minutos para contagem de requisições
    /// </summary>
    public int TimeWindowMinutes { get; set; } = 60;

    /// <summary>
    /// Número máximo de violações antes de bloquear temporariamente
    /// </summary>
    public int MaxViolationsBeforeBlock { get; set; } = 3;

    /// <summary>
    /// Duração do bloqueio temporário em minutos
    /// </summary>
    public int TemporaryBlockDurationMinutes { get; set; } = 15;

    /// <summary>
    /// Limite para detecção de rajadas (burst detection)
    /// </summary>
    public int BurstDetectionThreshold { get; set; } = 50;

    /// <summary>
    /// Threshold de variância para detecção de bots (menor = mais provável bot)
    /// </summary>
    public double BotDetectionVarianceThreshold { get; set; } = 50.0;

    /// <summary>
    /// Habilitar rate limiting avançado
    /// </summary>
    public bool Enabled { get; set; } = true;
}
