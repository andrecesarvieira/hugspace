/*
 * SynQcore - Corporate Social Network API
 *
 * Security Headers Middleware - Adds security headers for corporate compliance
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.Extensions.Options;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Middleware que adiciona headers de segurança corporativa
/// </summary>
public partial class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(
        RequestDelegate next,
        ILogger<SecurityHeadersMiddleware> logger,
        IOptions<SecurityHeadersOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Adicionar headers de segurança antes de processar a requisição
        AddSecurityHeaders(context);

        // Verificar proteção CSRF para requests POST/PUT/DELETE
        if (ShouldCheckCsrf(context))
        {
            if (!await ValidateCsrfToken(context))
            {
                LogCsrfValidationFailed(_logger, context.Request.Path, GetClientIp(context), null);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("CSRF token validation failed");
                return;
            }
        }

        LogSecurityHeadersApplied(_logger, context.Request.Path, GetClientIp(context), null);

        await _next(context);
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var response = context.Response;

        // Content Security Policy (CSP)
        if (!string.IsNullOrEmpty(_options.ContentSecurityPolicy))
        {
            response.Headers.Append("Content-Security-Policy", _options.ContentSecurityPolicy);
        }

        // HTTP Strict Transport Security (HSTS)
        if (_options.UseHsts && context.Request.IsHttps)
        {
            response.Headers.Append("Strict-Transport-Security",
                $"max-age={_options.HstsMaxAge}; includeSubDomains; preload");
        }

        // X-Frame-Options (clickjacking protection)
        response.Headers.Append("X-Frame-Options", _options.XFrameOptions);

        // X-Content-Type-Options (MIME sniffing protection)
        response.Headers.Append("X-Content-Type-Options", "nosniff");

        // X-XSS-Protection
        response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Referrer Policy
        response.Headers.Append("Referrer-Policy", _options.ReferrerPolicy);

        // Permissions Policy
        if (!string.IsNullOrEmpty(_options.PermissionsPolicy))
        {
            response.Headers.Append("Permissions-Policy", _options.PermissionsPolicy);
        }

        // Remove server headers for security
        response.Headers.Remove("Server");
        response.Headers.Remove("X-Powered-By");
        response.Headers.Remove("X-AspNet-Version");

        // Add custom corporate headers
        response.Headers.Append("X-SynQcore-Security", "enabled");
        response.Headers.Append("X-Content-Security-Framework", "SynQcore-Corporate");
    }

    private static bool ShouldCheckCsrf(HttpContext context)
    {
        var method = context.Request.Method.ToUpperInvariant();
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        // Skip CSRF check for safe methods
        if (method == "GET" || method == "HEAD" || method == "OPTIONS")
            return false;

        // Skip CSRF check for all API endpoints (JWT provides sufficient protection)
        if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return false;

        // Skip CSRF check for specific endpoints
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    private static Task<bool> ValidateCsrfToken(HttpContext context)
    {
        // For API endpoints with JWT, CSRF is not needed as JWT provides sufficient protection
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(true); // JWT is sufficient for API protection
        }

        // For web forms, check CSRF token
        var csrfToken = context.Request.Headers["X-CSRF-Token"].FirstOrDefault() ??
                       context.Request.Form["__RequestVerificationToken"].FirstOrDefault();

        if (string.IsNullOrEmpty(csrfToken))
        {
            return Task.FromResult(false);
        }

        // TODO: Implement proper CSRF token validation
        // For now, just check if token exists
        return Task.FromResult(true);
    }

    private static string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    // LoggerMessage delegates for high-performance logging
    [LoggerMessage(EventId = 3001, Level = LogLevel.Information,
        Message = "Security headers applied - Path: {Path} | IP: {ClientIP}")]
    private static partial void LogSecurityHeadersApplied(ILogger logger, string path, string clientIP, Exception? exception);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Warning,
        Message = "CSRF validation failed - Path: {Path} | IP: {ClientIP}")]
    private static partial void LogCsrfValidationFailed(ILogger logger, string path, string clientIP, Exception? exception);
}

/// <summary>
/// Opções de configuração para headers de segurança
/// </summary>
public class SecurityHeadersOptions
{
    public const string SectionName = "SecurityHeaders";

    /// <summary>
    /// Content Security Policy
    /// </summary>
    public string ContentSecurityPolicy { get; set; } =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https:; " +
        "connect-src 'self' ws: wss:; " +
        "frame-ancestors 'none';";

    /// <summary>
    /// X-Frame-Options header value
    /// </summary>
    public string XFrameOptions { get; set; } = "DENY";

    /// <summary>
    /// Referrer Policy
    /// </summary>
    public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";

    /// <summary>
    /// Permissions Policy
    /// </summary>
    public string PermissionsPolicy { get; set; } =
        "geolocation=(), camera=(), microphone=(), payment=()";

    /// <summary>
    /// Habilitar HSTS
    /// </summary>
    public bool UseHsts { get; set; } = true;

    /// <summary>
    /// HSTS max age em segundos (padrão: 1 ano)
    /// </summary>
    public int HstsMaxAge { get; set; } = 31536000;
}

/// <summary>
/// Extension methods para registro do middleware
/// </summary>
public static class SecurityHeadersExtensions
{
    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SecurityHeadersOptions>(configuration.GetSection(SecurityHeadersOptions.SectionName));
        return services;
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
