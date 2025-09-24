using AspNetCoreRateLimit;
using System.Security.Claims;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Corporate rate limiting middleware with department and role-based limits
/// </summary>
public class CorporateRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorporateRateLimitMiddleware> _logger;
    
    // LoggerMessage delegate for high-performance logging
    private static readonly Action<ILogger, string?, string, Exception?> LogRateLimitContext =
        LoggerMessage.Define<string?, string>(
            LogLevel.Debug,
            new EventId(3001, "CorporateRateLimit"),
            "Corporate Rate Limit - ClientId: {ClientId} | Path: {Path}");

    public CorporateRateLimitMiddleware(RequestDelegate next, ILogger<CorporateRateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for health checks and swagger
        if (ShouldSkipRateLimit(context))
        {
            await _next(context);
            return;
        }

        // Set client ID based on corporate structure
        var clientId = DetermineClientId(context);
        if (!string.IsNullOrEmpty(clientId))
        {
            context.Request.Headers["X-ClientId"] = clientId;
        }

        // Log rate limiting context for audit
        using var logContext = Serilog.Context.LogContext.PushProperty("RateLimitClientId", clientId);
        LogRateLimitContext(_logger, clientId, context.Request.Path, null);

        await _next(context);
    }

    private static bool ShouldSkipRateLimit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        return path switch
        {
            "/health" or "/health/ready" or "/health/live" => true,
            "/swagger" or "/swagger/index.html" => true,
            _ when path?.StartsWith("/swagger/", StringComparison.OrdinalIgnoreCase) == true => true,
            _ when path?.StartsWith("/_vs/", StringComparison.OrdinalIgnoreCase) == true => true,
            _ => false
        };
    }

    private static string DetermineClientId(HttpContext context)
    {
        // 1. Check for existing client ID header (for service-to-service calls)
        var existingClientId = context.Request.Headers["X-ClientId"].FirstOrDefault();
        if (!string.IsNullOrEmpty(existingClientId))
        {
            return existingClientId;
        }

        // 2. Determine client ID based on user claims (when authentication is implemented)
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var department = context.User.FindFirst("department")?.Value;
            
            return (role?.ToLowerInvariant(), department?.ToLowerInvariant()) switch
            {
                ("admin", _) => "admin-app",
                ("hr", _) or (_, "human resources") or (_, "hr") => "hr-app",
                ("manager", _) or ("supervisor", _) => "manager-app",
                ("employee", _) => "employee-app",
                _ => "employee-app" // Default fallback
            };
        }

        // 3. Determine based on User-Agent patterns (for different client apps)
        var userAgent = context.Request.Headers.UserAgent.ToString().ToLowerInvariant();
        
        if (userAgent.Contains("synqcore-admin"))
            return "admin-app";
        
        if (userAgent.Contains("synqcore-hr"))
            return "hr-app";
        
        if (userAgent.Contains("synqcore-manager"))
            return "manager-app";

        if (userAgent.Contains("synqcore-mobile") || userAgent.Contains("synqcore-employee"))
            return "employee-app";

        // 4. Check for special API keys or service accounts
        var apiKey = context.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKey))
        {
            return apiKey switch
            {
                _ when apiKey.StartsWith("admin_", StringComparison.OrdinalIgnoreCase) => "admin-app",
                _ when apiKey.StartsWith("hr_", StringComparison.OrdinalIgnoreCase) => "hr-app",
                _ when apiKey.StartsWith("manager_", StringComparison.OrdinalIgnoreCase) => "manager-app",
                _ when apiKey.StartsWith("monitoring_", StringComparison.OrdinalIgnoreCase) => "monitoring-client",
                _ => "employee-app"
            };
        }

        // 5. Default to employee-app for anonymous requests
        return "employee-app";
    }
}

/// <summary>
/// Extensions for configuring corporate rate limiting
/// </summary>
public static class CorporateRateLimitExtensions
{
    /// <summary>
    /// Adds corporate rate limiting services to the DI container
    /// </summary>
    public static IServiceCollection AddCorporateRateLimit(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure IP rate limiting
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
        
        // Configure client rate limiting  
        services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
        services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

        // Add rate limiting services
        services.AddMemoryCache();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

        return services;
    }

    /// <summary>
    /// Uses corporate rate limiting middleware in the pipeline
    /// </summary>
    public static IApplicationBuilder UseCorporateRateLimit(this IApplicationBuilder app)
    {
        // Corporate client ID middleware (must come before rate limiting)
        app.UseMiddleware<CorporateRateLimitMiddleware>();
        
        // IP rate limiting
        app.UseIpRateLimiting();
        
        // Client rate limiting  
        app.UseClientRateLimiting();
        
        return app;
    }
}