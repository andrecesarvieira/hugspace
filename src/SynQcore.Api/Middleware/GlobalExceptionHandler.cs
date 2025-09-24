using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using Serilog;
using Serilog.Context;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Global exception handler for corporate API with structured logging and audit trails
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    // Cached JsonSerializerOptions for performance
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    // LoggerMessage delegates for high-performance logging
    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, int, string, Exception?> LogCorporateException =
        LoggerMessage.Define<string, int, string>(
            Microsoft.Extensions.Logging.LogLevel.Error,
            new EventId(1001, "CorporateApiException"),
            "Corporate API Exception: {Title} | StatusCode: {StatusCode} | Detail: {Detail}");
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // Enrich logging context with corporate audit information
        using var logContext1 = LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier);
        using var logContext2 = LogContext.PushProperty("UserId", GetUserId(httpContext));
        using var logContext3 = LogContext.PushProperty("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        using var logContext4 = LogContext.PushProperty("RequestPath", httpContext.Request.Path);
        using var logContext5 = LogContext.PushProperty("RequestMethod", httpContext.Request.Method);
        using var logContext6 = LogContext.PushProperty("ClientIP", GetClientIpAddress(httpContext));

        var (statusCode, title, detail) = GetErrorDetails(exception);
        
        // Log structured error for corporate audit trails using high-performance LoggerMessage
        LogCorporateException(_logger, title, (int)statusCode, detail, exception);

        var problemDetails = new
        {
            Type = GetProblemType(statusCode),
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path,
            TraceId = httpContext.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);

        await httpContext.Response.WriteAsync(json, cancellationToken);
        
        return true;
    }

    private static (HttpStatusCode StatusCode, string Title, string Detail) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "Bad Request", "Required parameter was null"),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request", exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "Access denied"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid Operation", exception.Message),
            NotSupportedException => (HttpStatusCode.BadRequest, "Not Supported", exception.Message),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request Timeout", "The request timed out"),
            
            // Corporate specific exceptions
            _ when exception.Message.Contains("Employee not found") => 
                (HttpStatusCode.NotFound, "Employee Not Found", "The requested employee could not be found"),
            _ when exception.Message.Contains("Department not found") => 
                (HttpStatusCode.NotFound, "Department Not Found", "The requested department could not be found"),
            _ when exception.Message.Contains("Insufficient permissions") => 
                (HttpStatusCode.Forbidden, "Insufficient Permissions", "Access denied for this corporate resource"),
            
            // Generic server error for unhandled exceptions (don't expose internal details)
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error", 
                "An internal server error occurred. Please contact IT support if the problem persists.")
        };
    }

    private static string GetProblemType(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1", 
            HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            HttpStatusCode.RequestTimeout => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
            HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }

    private static string? GetUserId(HttpContext context)
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
}