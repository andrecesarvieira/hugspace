using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Corporate audit logging middleware for request/response tracking and compliance
/// </summary>
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;
    private static readonly HashSet<string> _sensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization", "Cookie", "X-Api-Key", "X-Auth-Token"
    };

    // LoggerMessage delegates for high-performance logging
    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, string, string, string, Exception?> LogRequestStarted =
        LoggerMessage.Define<string, string, string, string>(
            Microsoft.Extensions.Logging.LogLevel.Information,
            new EventId(2001, "CorporateApiRequestStarted"),
            "Corporate API Request Started: {Method} {Path} | User: {UserId} | IP: {ClientIP}");

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, string, string, Exception> LogRequestFailed =
        LoggerMessage.Define<string, string, string>(
            Microsoft.Extensions.Logging.LogLevel.Error,
            new EventId(2002, "CorporateApiRequestFailed"),
            "Corporate API Request Failed: {Method} {Path} | Exception: {ExceptionType}");

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, string, int, long, string, Exception?> LogRequestCompleted =
        LoggerMessage.Define<string, string, int, long, string>(
            Microsoft.Extensions.Logging.LogLevel.Information,
            new EventId(2003, "CorporateApiRequestCompleted"),
            "Corporate API Request Completed: {Method} {Path} | Status: {StatusCode} | Duration: {ElapsedMs}ms | User: {UserId}");

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, object, Exception?> LogAuditTrail =
        LoggerMessage.Define<object>(
            Microsoft.Extensions.Logging.LogLevel.Information,
            new EventId(2004, "CorporateAuditTrail"),
            "Corporate Audit Trail: {@AuditRecord}");

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip audit logging for health checks and static files
        if (ShouldSkipLogging(context))
        {
            await _next(context);
            return;
        }

        var correlationId = context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();
        
        // Capture request details
        var requestBody = await CaptureRequestBody(context);
        
        // Enrich log context with corporate audit information
        using var logContext1 = LogContext.PushProperty("CorrelationId", correlationId);
        using var logContext2 = LogContext.PushProperty("UserId", GetUserId(context));
        using var logContext3 = LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString());
        using var logContext4 = LogContext.PushProperty("ClientIP", GetClientIpAddress(context));
        using var logContext5 = LogContext.PushProperty("RequestMethod", context.Request.Method);
        using var logContext6 = LogContext.PushProperty("RequestPath", context.Request.Path);
        using var logContext7 = LogContext.PushProperty("RequestQuery", context.Request.QueryString.ToString());
        using var logContext8 = LogContext.PushProperty("RequestHeaders", GetSafeHeaders(context.Request.Headers), true);
        using var logContext9 = LogContext.PushProperty("RequestBodySize", requestBody?.Length ?? 0);

        // Log incoming request using high-performance LoggerMessage
        LogRequestStarted(_logger, 
            context.Request.Method, 
            context.Request.Path,
            GetUserId(context),
            GetClientIpAddress(context) ?? "unknown",
            null);

        // Capture response
        var originalResponseBody = context.Response.Body;
        using var responseBodyMemoryStream = new MemoryStream();
        context.Response.Body = responseBodyMemoryStream;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            LogRequestFailed(_logger, context.Request.Method, context.Request.Path, ex.GetType().Name, ex);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            // Capture response body for audit (if not too large)
            var responseBody = await CaptureResponseBody(responseBodyMemoryStream);
            
            // Copy response back to original stream
            responseBodyMemoryStream.Position = 0;
            await responseBodyMemoryStream.CopyToAsync(originalResponseBody);
            
            // Log completed request with corporate audit details
            using var responseLogContext1 = LogContext.PushProperty("ResponseStatusCode", context.Response.StatusCode);
            using var responseLogContext2 = LogContext.PushProperty("ResponseHeaders", GetSafeHeaders(context.Response.Headers), true);
            using var responseLogContext3 = LogContext.PushProperty("ResponseBodySize", responseBody?.Length ?? 0);
            using var responseLogContext4 = LogContext.PushProperty("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds);

            // Use high-performance LoggerMessage for request completed
            LogRequestCompleted(_logger,
                context.Request.Method, 
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                GetUserId(context),
                null);

            // Log detailed audit trail for compliance (if configured)
            if (ShouldLogDetailedAudit(context))
            {
                var auditRecord = new
                {
                    CorrelationId = correlationId,
                    Timestamp = DateTime.UtcNow,
                    UserId = GetUserId(context),
                    Action = $"{context.Request.Method} {context.Request.Path}",
                    StatusCode = context.Response.StatusCode,
                    Duration = stopwatch.ElapsedMilliseconds,
                    ClientIP = GetClientIpAddress(context),
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    RequestSize = requestBody?.Length ?? 0,
                    ResponseSize = responseBody?.Length ?? 0
                };
                
                LogAuditTrail(_logger, auditRecord, null);
            }
        }
    }

    private static bool ShouldSkipLogging(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        return path switch
        {
            "/health" or "/health/ready" or "/health/live" => true,
            "/swagger" or "/swagger/index.html" => true,
            _ when path?.StartsWith("/swagger/", StringComparison.OrdinalIgnoreCase) == true => true,
            _ when path?.StartsWith("/_vs/", StringComparison.OrdinalIgnoreCase) == true => true, // VS debugging
            _ when path?.Contains(".ico") == true => true,
            _ => false
        };
    }

    private static async Task<string?> CaptureRequestBody(HttpContext context)
    {
        if (context.Request.ContentLength == 0 || context.Request.ContentLength > 50_000) // Skip large bodies
            return null;

        context.Request.EnableBuffering();
        
        var buffer = new byte[context.Request.ContentLength ?? 0];
        await context.Request.Body.ReadExactlyAsync(buffer, 0, buffer.Length);
        context.Request.Body.Position = 0;
        
        return Encoding.UTF8.GetString(buffer);
    }

    private static async Task<string?> CaptureResponseBody(MemoryStream responseBodyStream)
    {
        if (responseBodyStream.Length == 0 || responseBodyStream.Length > 50_000) // Skip large responses
            return null;

        responseBodyStream.Position = 0;
        var responseBytes = new byte[responseBodyStream.Length];
        await responseBodyStream.ReadExactlyAsync(responseBytes);
        
        return Encoding.UTF8.GetString(responseBytes);
    }

    private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        return headers
            .Where(h => !_sensitiveHeaders.Contains(h.Key))
            .Take(10) // Limit number of headers logged
            .ToDictionary(h => h.Key, h => h.Value.ToString());
    }

    private static string GetUserId(HttpContext context)
    {
        // Extract user ID from JWT claims when authentication is implemented
        return context.User?.FindFirst("sub")?.Value ?? 
               context.User?.FindFirst("employee_id")?.Value ?? 
               "anonymous";
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // Handle corporate proxy scenarios
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

        return context.Connection.RemoteIpAddress?.ToString();
    }



    private static bool ShouldLogDetailedAudit(HttpContext context)
    {
        // Log detailed audit for:
        // - POST/PUT/DELETE operations (data changes)
        // - Authentication endpoints
        // - Admin operations
        var method = context.Request.Method;
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        return method is "POST" or "PUT" or "DELETE" or "PATCH" ||
               path?.Contains("auth") == true ||
               path?.Contains("admin") == true;
    }
}