/*
 * SynQcore - Corporate Social Network API
 * 
 * Copyright (c) 2025 André César Vieira
 * Licensed under the MIT License
 */

using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using SynQcore.Shared;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Test controller for validating corporate rate limiting
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    // LoggerMessage delegates for high-performance logging
    private static readonly Action<ILogger, string, Exception?> LogPingReceived =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(4001, "TestPingReceived"),
            "Test ping received from ClientId: {ClientId}");

    private static readonly Action<ILogger, string, int, Exception?> LogHeavyOperationStarted =
        LoggerMessage.Define<string, int>(
            LogLevel.Information,
            new EventId(4002, "HeavyOperationStarted"),
            "Heavy operation started by ClientId: {ClientId} with delay: {Delay}ms");

    private static readonly Action<ILogger, string, string, Exception?> LogClientTypeTest =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(4003, "ClientTypeTest"),
            "Client type test - Requested: {RequestedType}, Detected: {ActualClientId}");

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple ping endpoint to test rate limiting
    /// </summary>
    /// <returns>Pong response with timestamp and rate limit info</returns>
    [HttpGet("ping")]
    public ActionResult<object> Ping()
    {
        var clientId = Request.Headers["X-ClientId"].FirstOrDefault() ?? "anonymous";
        var userAgent = Request.Headers.UserAgent.ToString();
        
        LogPingReceived(_logger, clientId, null);

        return Ok(new
        {
            Message = "Pong! SynQcore Corporate API is working",
            Timestamp = DateTime.UtcNow,
            ClientId = clientId,
            UserAgent = userAgent,
            TraceId = HttpContext.TraceIdentifier,
            RateLimitInfo = new
            {
                ClientIdentification = "Based on X-ClientId header, User-Agent, or defaults to employee-app",
                AvailableLimits = new
                {
                    EmployeeApp = "100 requests/min, 1000 requests/hour",
                    ManagerApp = "300 requests/min, 5000 requests/hour", 
                    HRApp = "500 requests/min, 10000 requests/hour",
                    AdminApp = "1000 requests/min, 50000 requests/hour"
                }
            }
        });
    }

    /// <summary>
    /// Endpoint to simulate heavy operations (for testing rate limiting)
    /// </summary>
    /// <param name="delay">Delay in milliseconds</param>
    /// <returns>Response after delay</returns>
    [HttpGet("heavy")]
    public async Task<ActionResult<object>> HeavyOperation([FromQuery] int delay = 100)
    {
        var clientId = Request.Headers["X-ClientId"].FirstOrDefault() ?? "anonymous";
        
        LogHeavyOperationStarted(_logger, clientId, delay, null);

        // Simulate processing time
        if (delay > 0 && delay <= 5000) // Max 5 seconds for safety
        {
            await Task.Delay(delay);
        }

        return Ok(new
        {
            Message = "Heavy operation completed",
            ClientId = clientId,
            DelayMs = delay,
            CompletedAt = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// Endpoint for testing different client types
    /// </summary>
    /// <param name="clientType">Type of client (employee, manager, hr, admin)</param>
    /// <returns>Response with client-specific information</returns>
    [HttpGet("client/{clientType}")]
    public ActionResult<object> TestClientType(string clientType)
    {
        var actualClientId = Request.Headers["X-ClientId"].FirstOrDefault() ?? "detected-as-employee-app";
        
        LogClientTypeTest(_logger, clientType, actualClientId, null);

        var limits = GetClientLimits(actualClientId);

        return Ok(new
        {
            RequestedClientType = clientType,
            DetectedClientId = actualClientId,
            RateLimits = limits,
            Recommendations = new
            {
                SetXClientIdHeader = "Send X-ClientId header with: employee-app, manager-app, hr-app, or admin-app",
                SetUserAgent = "Use User-Agent with: synqcore-employee, synqcore-manager, synqcore-hr, or synqcore-admin",
                UseApiKey = "Send X-Api-Key with appropriate prefix: employee_, manager_, hr_, or admin_"
            }
        });
    }

    /// <summary>
    /// Get project information and author details
    /// </summary>
    /// <returns>Complete project information including author, version, and technology stack</returns>
    [HttpGet("about")]
    public ActionResult<object> GetProjectInfo()
    {
        return Ok(new
        {
            Project = new
            {
                Name = SynQcoreInfo.ProjectName,
                FullTitle = SynQcoreInfo.FullTitle,
                Description = SynQcoreInfo.Description,
                Version = SynQcoreInfo.Version,
                CurrentPhase = SynQcoreInfo.Metrics.CurrentPhase,
                NextPhase = SynQcoreInfo.Metrics.NextPhase
            },
            Author = new
            {
                Name = SynQcoreInfo.Author,
                Email = SynQcoreInfo.AuthorEmail,
                GitHub = SynQcoreInfo.AuthorGitHub,
                Expertise = new[]
                {
                    "Enterprise Architecture",
                    ".NET Ecosystem", 
                    "Clean Architecture",
                    "PostgreSQL Optimization",
                    "Performance Engineering",
                    "Corporate Software Development"
                }
            },
            Legal = new
            {
                Copyright = SynQcoreInfo.Copyright,
                License = SynQcoreInfo.License,
                Repository = SynQcoreInfo.RepositoryUrl
            },
            TechnologyStack = new
            {
                Framework = SynQcoreInfo.Technologies.Framework,
                Database = SynQcoreInfo.Technologies.Database,
                Cache = SynQcoreInfo.Technologies.Cache,
                Architecture = SynQcoreInfo.Technologies.Architecture,
                Patterns = SynQcoreInfo.Technologies.Patterns,
                Frontend = SynQcoreInfo.Technologies.Frontend,
                Containerization = SynQcoreInfo.Technologies.Containerization
            },
            ProjectStats = new
            {
                Entities = SynQcoreInfo.Metrics.EntitiesCount,
                DatabaseTables = SynQcoreInfo.Metrics.DatabaseTables,
                Projects = SynQcoreInfo.Metrics.ProjectsCount,
                BuildDate = SynQcoreInfo.BuildDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " UTC"
            },
            Message = "⭐ Star this repository if SynQcore helped you build better corporate applications!",
            CallToAction = new
            {
                StarRepository = SynQcoreInfo.RepositoryUrl,
                FollowAuthor = SynQcoreInfo.AuthorGitHub,
                ContactAuthor = $"mailto:{SynQcoreInfo.AuthorEmail}",
                ContributeToProject = $"{SynQcoreInfo.RepositoryUrl}/issues"
            }
        });
    }

    private static object GetClientLimits(string clientId)
    {
        return clientId switch
        {
            "employee-app" => new { RequestsPerMinute = 100, RequestsPerHour = 1000, Priority = "Standard" },
            "manager-app" => new { RequestsPerMinute = 300, RequestsPerHour = 5000, Priority = "Enhanced" },
            "hr-app" => new { RequestsPerMinute = 500, RequestsPerHour = 10000, Priority = "High" },
            "admin-app" => new { RequestsPerMinute = 1000, RequestsPerHour = 50000, Priority = "Maximum" },
            "monitoring-client" => new { RequestsPerMinute = "Unlimited", RequestsPerHour = "Unlimited", Priority = "Whitelisted" },
            _ => new { RequestsPerMinute = 100, RequestsPerHour = 1000, Priority = "Default (Employee)" }
        };
    }
}