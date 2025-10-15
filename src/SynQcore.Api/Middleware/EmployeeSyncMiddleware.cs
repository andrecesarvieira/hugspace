using System.Security.Claims;
using SynQcore.Application.Services;

namespace SynQcore.Api.Middleware;

/// <summary>
/// Middleware para garantir sincronização automática entre AspNetUsers e Employees
/// </summary>
public partial class EmployeeSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EmployeeSyncMiddleware> _logger;

    public EmployeeSyncMiddleware(
        RequestDelegate next,
        ILogger<EmployeeSyncMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IEmployeeSyncService employeeSyncService)
    {
        // Verificar se o usuário está autenticado
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Obter ID do usuário do JWT
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                 context.User.FindFirst("sub")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
                {
                    // Garantir que o Employee existe (criação automática se necessário)
                    await employeeSyncService.EnsureEmployeeExistsAsync(userId);

                    LogEmployeeSyncVerified(_logger, userId);
                }
                else
                {
                    LogUserIdNotFound(_logger, userIdClaim ?? "null");
                }
            }
            catch (Exception ex)
            {
                // Log do erro, mas não interromper a requisição
                LogEmployeeSyncError(_logger, ex);
            }
        }

        // Continuar com o pipeline
        await _next(context);
    }

    #region LoggerMessage delegates

    [LoggerMessage(EventId = 4001, Level = LogLevel.Debug,
        Message = "Employee sync verificado para usuário {UserId}")]
    private static partial void LogEmployeeSyncVerified(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Warning,
        Message = "ID do usuário não encontrado no token JWT: {UserIdClaim}")]
    private static partial void LogUserIdNotFound(ILogger logger, string userIdClaim);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Error,
        Message = "Erro durante sincronização automática de Employee")]
    private static partial void LogEmployeeSyncError(ILogger logger, Exception ex);

    #endregion
}
