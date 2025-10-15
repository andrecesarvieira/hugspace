/*
 * SynQcore - Corporate Social Network API
 *
 * Corporate Rate Limit Middleware - Determines client ID based on corporate structure
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using AspNetCoreRateLimit;
using Microsoft.Extensions.Logging;

namespace SynQcore.Api.Middleware;

public partial class CorporateRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorporateRateLimitMiddleware> _logger;

    public CorporateRateLimitMiddleware(RequestDelegate next, ILogger<CorporateRateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
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

    private static string DetermineClientId(HttpContext context)
    {
        // 1. Check JWT token for role claims (when authentication is implemented)
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Parse JWT and extract role claims when authentication is implemented
            // For now, simulate role detection based on API patterns

            var endpoint = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            // Admin endpoints
            if (endpoint.Contains("/admin/") || endpoint.Contains("/system/"))
                return "admin-app";

            // HR endpoints
            if (endpoint.Contains("/hr/") || endpoint.Contains("/employees/bulk") || endpoint.Contains("/departments/"))
                return "hr-app";

            // Manager endpoints
            if (endpoint.Contains("/manager/") || endpoint.Contains("/teams/") || endpoint.Contains("/reports/"))
                return "manager-app";
        }

        // 2. Check custom client headers for service-to-service communication
        var clientHeader = context.Request.Headers["X-Client-Type"].FirstOrDefault();
        if (!string.IsNullOrEmpty(clientHeader))
        {
            return clientHeader.ToLowerInvariant() switch
            {
                "admin" or "admin-app" => "admin-app",
                "hr" or "hr-app" => "hr-app",
                "manager" or "manager-app" => "manager-app",
                "employee" or "employee-app" => "employee-app",
                "mobile" or "mobile-app" => "employee-app",
                _ => "employee-app"
            };
        }

        // 3. Check User-Agent for mobile/desktop client identification
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault()?.ToLowerInvariant() ?? "";
        if (userAgent.Contains("synqcore-admin") || userAgent.Contains("postman") || userAgent.Contains("insomnia"))
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

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information,
        Message = "Rate limit context - Client: {ClientId}, Path: {Path}, IP: {RemoteIP}")]
    private static partial void LogRateLimitContext(ILogger logger, string? clientId, string? path, string? remoteIP);
}

public static partial class CorporateRateLimitExtensions
{
    public static IServiceCollection AddCorporateRateLimit(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure IP Rate Limiting
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        // Configure Client Rate Limiting
        services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));

        // Check if rate limiting should be enabled for current environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var ipRateLimitConfig = configuration.GetSection("IpRateLimiting");
        var isEnabled = ipRateLimitConfig.GetValue<bool>("EnableEndpointRateLimiting");

        // Skip rate limiting setup for Docker environment if disabled
        if (environment?.Equals("Docker", StringComparison.OrdinalIgnoreCase) == true && !isEnabled)
        {
            // Add dummy services to prevent DI errors
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            return services;
        }

        // Check if Redis should be used for rate limiting store
        var storeConfig = configuration.GetSection("RateLimitingStore");
        var storeType = storeConfig["Type"];

        if (storeType?.Equals("Redis", StringComparison.OrdinalIgnoreCase) == true)
        {
            // Add Redis for distributed rate limiting
            var redisConnection = storeConfig["RedisConfiguration"];
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "SynQcore_RateLimit";
                });

                // Use distributed rate limiting with Redis
                services.AddDistributedRateLimiting();
            }
        }
        else
        {
            // Use in-memory rate limiting for development
            services.AddInMemoryRateLimiting();
        }

        // Register required services
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    public static IApplicationBuilder UseCorporateRateLimit(this IApplicationBuilder app)
    {
        // Corporate client ID middleware (must come before rate limiting)
        app.UseMiddleware<CorporateRateLimitMiddleware>();

        try
        {
            // IP rate limiting
            app.UseIpRateLimiting();

            // Client rate limiting
            app.UseClientRateLimiting();
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the application
            var logger = app.ApplicationServices.GetRequiredService<ILogger<CorporateRateLimitMiddleware>>();
            LogRateLimitingInitializationError(logger, ex);
        }

        return app;
    }

    [LoggerMessage(EventId = 2002, Level = LogLevel.Error,
        Message = "Failed to initialize rate limiting middleware. Rate limiting will be disabled.")]
    private static partial void LogRateLimitingInitializationError(ILogger logger, Exception exception);
}
