/*
 * SynQcore - Corporate Social Network API
 *
 * HTTPS Enforcement Middleware - Força HTTPS em produção com redirect seguro
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Middleware que força o uso de HTTPS em ambientes corporativos
/// </summary>
public partial class HttpsEnforcementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpsEnforcementMiddleware> _logger;
    private readonly HttpsEnforcementOptions _options;

    public HttpsEnforcementMiddleware(
        RequestDelegate next,
        ILogger<HttpsEnforcementMiddleware> logger,
        IOptions<HttpsEnforcementOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip enforcement in development environment se configurado
        if (_options.SkipInDevelopment &&
            context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            await _next(context);
            return;
        }

        // Skip enforcement para health checks e swagger se configurado
        if (_options.SkipHealthChecks && IsHealthCheckOrSwagger(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Verificar se a requisição já é HTTPS
        if (context.Request.IsHttps)
        {
            await _next(context);
            return;
        }

        // Verificar se está atrás de proxy com X-Forwarded-Proto
        var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedProto) &&
            forwardedProto.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Log da tentativa de acesso inseguro
        LogInsecureAccessAttempt(_logger, context.Request.Path, GetClientIp(context), null);

        // Aplicar ação configurada
        switch (_options.EnforcementAction)
        {
            case HttpsEnforcementAction.Redirect:
                await RedirectToHttps(context);
                break;

            case HttpsEnforcementAction.Block:
                await BlockHttpRequest(context);
                break;

            case HttpsEnforcementAction.Warn:
                await WarnAndContinue(context);
                break;
        }
    }

    private async Task RedirectToHttps(HttpContext context)
    {
        var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";

        LogHttpsRedirect(_logger, context.Request.GetDisplayUrl(), httpsUrl, GetClientIp(context), null);

        context.Response.StatusCode = _options.RedirectStatusCode;
        context.Response.Headers.Location = httpsUrl;

        await context.Response.WriteAsync($"Redirecting to HTTPS: {httpsUrl}");
    }

    private async Task BlockHttpRequest(HttpContext context)
    {
        LogHttpRequestBlocked(_logger, context.Request.Path, GetClientIp(context), null);

        context.Response.StatusCode = 426; // Upgrade Required
        context.Response.Headers.Append("Upgrade", "TLS/1.2, HTTP/1.1");
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "HTTPS_REQUIRED",
            message = "This corporate network requires HTTPS for security compliance",
            upgrade = "Please use HTTPS to access this resource"
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private async Task WarnAndContinue(HttpContext context)
    {
        LogHttpsWarning(_logger, context.Request.Path, GetClientIp(context), null);

        // Adicionar header de warning
        context.Response.Headers.Append("Warning", "299 - \"Insecure connection detected. Please use HTTPS.\"");

        await _next(context);
    }

    private static bool IsHealthCheckOrSwagger(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? "";
        return pathValue.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
               pathValue.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
               pathValue.StartsWith("/_health", StringComparison.OrdinalIgnoreCase);
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

    // LoggerMessage delegates para performance
    [LoggerMessage(EventId = 3101, Level = LogLevel.Warning,
        Message = "Insecure access attempt - Path: {Path} | IP: {ClientIP}")]
    private static partial void LogInsecureAccessAttempt(ILogger logger, string path, string clientIP, Exception? exception);

    [LoggerMessage(EventId = 3102, Level = LogLevel.Information,
        Message = "HTTPS redirect - From: {HttpUrl} | To: {HttpsUrl} | IP: {ClientIP}")]
    private static partial void LogHttpsRedirect(ILogger logger, string httpUrl, string httpsUrl, string clientIP, Exception? exception);

    [LoggerMessage(EventId = 3103, Level = LogLevel.Warning,
        Message = "HTTP request blocked - Path: {Path} | IP: {ClientIP}")]
    private static partial void LogHttpRequestBlocked(ILogger logger, string path, string clientIP, Exception? exception);

    [LoggerMessage(EventId = 3104, Level = LogLevel.Information,
        Message = "HTTPS warning issued - Path: {Path} | IP: {ClientIP}")]
    private static partial void LogHttpsWarning(ILogger logger, string path, string clientIP, Exception? exception);
}

/// <summary>
/// Opções de configuração para enforcement de HTTPS
/// </summary>
public class HttpsEnforcementOptions
{
    public const string SectionName = "HttpsEnforcement";

    /// <summary>
    /// Ação a ser tomada quando HTTP é detectado
    /// </summary>
    public HttpsEnforcementAction EnforcementAction { get; set; } = HttpsEnforcementAction.Redirect;

    /// <summary>
    /// Status code para redirect (301 permanente ou 302 temporário)
    /// </summary>
    public int RedirectStatusCode { get; set; } = 301;

    /// <summary>
    /// Pular enforcement em desenvolvimento
    /// </summary>
    public bool SkipInDevelopment { get; set; } = true;

    /// <summary>
    /// Pular enforcement para health checks e swagger
    /// </summary>
    public bool SkipHealthChecks { get; set; } = true;

    /// <summary>
    /// Habilitar enforcement
    /// </summary>
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Ações possíveis para enforcement de HTTPS
/// </summary>
public enum HttpsEnforcementAction
{
    /// <summary>
    /// Redirecionar para HTTPS
    /// </summary>
    Redirect,

    /// <summary>
    /// Bloquear requisição HTTP
    /// </summary>
    Block,

    /// <summary>
    /// Apenas avisar mas continuar
    /// </summary>
    Warn
}

/// <summary>
/// Extension methods para configuração do middleware
/// </summary>
public static class HttpsEnforcementExtensions
{
    public static IServiceCollection AddHttpsEnforcement(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HttpsEnforcementOptions>(configuration.GetSection(HttpsEnforcementOptions.SectionName));
        return services;
    }

    public static IApplicationBuilder UseHttpsEnforcement(this IApplicationBuilder app)
    {
        return app.UseMiddleware<HttpsEnforcementMiddleware>();
    }
}
