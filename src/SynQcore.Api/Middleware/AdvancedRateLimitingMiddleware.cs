/*
 * SynQcore - Corporate Social Network API
 *
 * Advanced Rate Limiting Middleware - Middleware integrado com detecção de padrões
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SynQcore.Application.Services;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Middleware que aplica rate limiting avançado com detecção de padrões suspeitos
/// </summary>
public partial class AdvancedRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdvancedRateLimitingMiddleware> _logger;
    private readonly IAdvancedRateLimitingService _rateLimitingService;
    private readonly AdvancedRateLimitingMiddlewareOptions _options;

    // JsonSerializerOptions reutilizável
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AdvancedRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<AdvancedRateLimitingMiddleware> logger,
        IAdvancedRateLimitingService rateLimitingService,
        IOptions<AdvancedRateLimitingMiddlewareOptions> options)
    {
        _next = next;
        _logger = logger;
        _rateLimitingService = rateLimitingService;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting se desabilitado
        if (!_options.Enabled)
        {
            await _next(context);
            return;
        }

        // Skip para endpoints específicos
        if (ShouldSkipRateLimit(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var ipAddress = GetClientIpAddress(context);
        var endpoint = GetEndpoint(context);
        var userId = GetUserId(context);

        try
        {
            // Verificar rate limit
            var rateLimitResult = await _rateLimitingService.CheckRateLimitAsync(ipAddress, endpoint, userId);

            // Adicionar headers informativos
            AddRateLimitHeaders(context, rateLimitResult);

            if (!rateLimitResult.IsAllowed)
            {
                LogRateLimitExceeded(_logger, ipAddress, endpoint, rateLimitResult.Reason ?? "Unknown", null);

                await HandleRateLimitExceeded(context, rateLimitResult);
                return;
            }

            // Registrar requisição (vai ser registrada no finally se chegou até aqui)
            await _next(context);

            // Registrar como sucesso se chegou até aqui sem exceção
            await _rateLimitingService.RecordRequestAsync(ipAddress, endpoint, true);

            LogRequestAllowed(_logger, ipAddress, endpoint, rateLimitResult.RemainingRequests, null);
        }
        catch (Exception ex)
        {
            // Registrar como falha
            await _rateLimitingService.RecordRequestAsync(ipAddress, endpoint, false);

            LogRequestFailed(_logger, ipAddress, endpoint, ex.Message, ex);
            throw;
        }
    }

    private void AddRateLimitHeaders(HttpContext context, RateLimitResult result)
    {
        var response = context.Response;

        response.Headers.Append("X-RateLimit-Limit", _options.DisplayLimit.ToString(CultureInfo.InvariantCulture));
        response.Headers.Append("X-RateLimit-Remaining", result.RemainingRequests.ToString(CultureInfo.InvariantCulture));
        response.Headers.Append("X-RateLimit-Reset", new DateTimeOffset(result.ResetTime).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));

        if (!result.IsAllowed)
        {
            response.Headers.Append("Retry-After", ((int)(result.ResetTime - DateTime.UtcNow).TotalSeconds).ToString(CultureInfo.InvariantCulture));
        }
    }

    private static async Task HandleRateLimitExceeded(HttpContext context, RateLimitResult result)
    {
        context.Response.StatusCode = 429; // Too Many Requests
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "RATE_LIMIT_EXCEEDED",
            message = result.Reason ?? "Rate limit exceeded",
            remaining = result.RemainingRequests,
            resetTime = result.ResetTime,
            retryAfter = (int)(result.ResetTime - DateTime.UtcNow).TotalSeconds
        };

        var jsonResponse = JsonSerializer.Serialize(response, _jsonOptions);

        await context.Response.WriteAsync(jsonResponse);
    }

    private bool ShouldSkipRateLimit(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? "";

        // Skip para health checks
        if (pathValue.StartsWith("/health", StringComparison.OrdinalIgnoreCase) || pathValue.StartsWith("/_health", StringComparison.OrdinalIgnoreCase))
            return true;

        // Skip para swagger em development
        if (_options.SkipSwaggerInDevelopment &&
            (pathValue.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) || pathValue.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase)))
            return true;

        // Skip para endpoints específicos configurados
        return _options.SkipPaths.Any(skipPath =>
            pathValue.StartsWith(skipPath.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Verificar X-Forwarded-For (proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // Pegar o primeiro IP da lista (cliente original)
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // Verificar X-Real-IP
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp.Trim();
        }

        // Verificar CF-Connecting-IP (Cloudflare)
        var cfConnectingIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cfConnectingIp))
        {
            return cfConnectingIp.Trim();
        }

        // Fallback para IP da conexão
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string GetEndpoint(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;

        // Normalizar para análise de rate limit
        return $"{method}:{path}";
    }

    private static string? GetUserId(HttpContext context)
    {
        // Tentar extrair user ID do JWT claim
        var userIdClaim = context.User?.FindFirst("sub") ?? context.User?.FindFirst("userId");
        return userIdClaim?.Value;
    }

    // LoggerMessage delegates para performance
    [LoggerMessage(EventId = 4001, Level = LogLevel.Warning,
        Message = "Rate limit exceeded - IP: {IpAddress} | Endpoint: {Endpoint} | Reason: {Reason}")]
    private static partial void LogRateLimitExceeded(ILogger logger, string ipAddress, string endpoint, string reason, Exception? exception);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information,
        Message = "Request allowed - IP: {IpAddress} | Endpoint: {Endpoint} | Remaining: {Remaining}")]
    private static partial void LogRequestAllowed(ILogger logger, string ipAddress, string endpoint, int remaining, Exception? exception);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Error,
        Message = "Request failed - IP: {IpAddress} | Endpoint: {Endpoint} | Error: {ErrorMessage}")]
    private static partial void LogRequestFailed(ILogger logger, string ipAddress, string endpoint, string errorMessage, Exception? exception);
}

/// <summary>
/// Opções de configuração para o middleware de rate limiting avançado
/// </summary>
public class AdvancedRateLimitingMiddlewareOptions
{
    public const string SectionName = "AdvancedRateLimitingMiddleware";

    /// <summary>
    /// Habilitar middleware
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Limite a ser exibido nos headers (para não revelar valores internos)
    /// </summary>
    public int DisplayLimit { get; set; } = 100;

    /// <summary>
    /// Pular swagger em development
    /// </summary>
    public bool SkipSwaggerInDevelopment { get; set; } = true;

    /// <summary>
    /// Paths específicos para pular rate limiting
    /// </summary>
    public List<string> SkipPaths { get; set; } = new() { "/favicon.ico", "/robots.txt" };
}

/// <summary>
/// Extension methods para configuração do middleware
/// </summary>
public static class AdvancedRateLimitingMiddlewareExtensions
{
    public static IServiceCollection AddAdvancedRateLimitingMiddleware(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdvancedRateLimitingMiddlewareOptions>(
            configuration.GetSection(AdvancedRateLimitingMiddlewareOptions.SectionName));
        return services;
    }

    public static IApplicationBuilder UseAdvancedRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AdvancedRateLimitingMiddleware>();
    }
}
