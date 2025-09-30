using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Services;
using SynQcore.Domain.Entities;
using SynQcore.Infrastructure.Data;
using System.Text.Json;

namespace SynQcore.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de auditoria corporativa
/// Registra eventos de segurança e ações importantes do sistema com alta performance
/// </summary>
public partial class AuditService : IAuditService
{
    private readonly SynQcoreDbContext _context;
    private readonly ILogger<AuditService> _logger;

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Evento de auditoria registrado - Ação: {actionType}, Usuário: {userId}, Recurso: {resourceType}:{resourceId}")]
    private static partial void LogAuditEventRegistered(ILogger logger, AuditActionType actionType, Guid? userId, string resourceType, string? resourceId, Exception? exception);

    [LoggerMessage(LogLevel.Warning, "Falha ao registrar evento de auditoria - Ação: {actionType}, Erro: {error}")]
    private static partial void LogAuditEventFailed(ILogger logger, AuditActionType actionType, string error, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Limpeza de logs antigos executada - Removidos: {removedCount} registros")]
    private static partial void LogCleanupExecuted(ILogger logger, int removedCount, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Busca de logs de auditoria - Filtros: {filters}, Resultados: {count}")]
    private static partial void LogSearchExecuted(ILogger logger, string filters, int count, Exception? exception);

    [LoggerMessage(LogLevel.Error, "Erro durante limpeza de logs de auditoria - {error}")]
    private static partial void LogCleanupError(ILogger logger, string error, Exception? exception);

    public AuditService(SynQcoreDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(
        AuditActionType actionType,
        string resourceType,
        string? resourceId,
        Guid? userId,
        string details,
        AuditSeverity severity = AuditSeverity.Information,
        string? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLogEntity
            {
                Id = Guid.NewGuid(),
                ActionType = actionType,
                ResourceType = resourceType,
                ResourceId = resourceId,
                UserId = userId?.ToString(),
                UserName = "Sistema", // TODO: Buscar nome do usuário
                Details = details,
                Severity = severity,
                Success = true,
                Category = GetCategoryFromActionType(actionType),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);

            LogAuditEventRegistered(_logger, actionType, userId, resourceType, resourceId, null);
        }
        catch (Exception ex)
        {
            LogAuditEventFailed(_logger, actionType, ex.Message, ex);
            // Não re-lançar exceção para não interromper operações principais
        }
    }

    private static AuditCategory GetCategoryFromActionType(AuditActionType actionType)
    {
        return actionType switch
        {
            AuditActionType.Login or AuditActionType.Logout or AuditActionType.LoginFailed => AuditCategory.Authentication,
            AuditActionType.PermissionGranted or AuditActionType.PermissionDenied or AuditActionType.PermissionChanged => AuditCategory.Authorization,
            AuditActionType.Create or AuditActionType.Update or AuditActionType.Delete => AuditCategory.DataModification,
            AuditActionType.Read => AuditCategory.DataAccess,
            AuditActionType.SecurityViolation or AuditActionType.UnauthorizedAccess => AuditCategory.Security,
            AuditActionType.DataExport or AuditActionType.DataDeletion => AuditCategory.Compliance,
            _ => AuditCategory.SystemOperation
        };
    }

    public async Task LogLoginSuccessAsync(Guid userId, string email, string ipAddress, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.Login,
            "Authentication",
            userId.ToString(),
            userId,
            $"Login bem-sucedido para usuário {email} do IP {ipAddress}",
            AuditSeverity.Information,
            JsonSerializer.Serialize(new { email, ipAddress, userAgent, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogLoginFailureAsync(string email, string ipAddress, string reason, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.LoginFailed,
            "Authentication",
            null,
            null,
            $"Tentativa de login falhada para {email} do IP {ipAddress}: {reason}",
            AuditSeverity.Warning,
            JsonSerializer.Serialize(new { email, ipAddress, userAgent, reason, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogLogoutAsync(Guid userId, string email, string ipAddress, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.Logout,
            "Authentication",
            userId.ToString(),
            userId,
            $"Logout do usuário {email} do IP {ipAddress}",
            AuditSeverity.Information,
            JsonSerializer.Serialize(new { email, ipAddress, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogEntityCreatedAsync<T>(Guid entityId, Guid userId, T entity, CancellationToken cancellationToken = default) where T : class
    {
        var entityType = typeof(T).Name;
        await LogAsync(
            AuditActionType.Create,
            entityType,
            entityId.ToString(),
            userId,
            $"Entidade {entityType} criada com ID {entityId}",
            AuditSeverity.Information,
            JsonSerializer.Serialize(new { entityData = entity, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogEntityUpdatedAsync<T>(Guid entityId, Guid userId, T previousEntity, T currentEntity, CancellationToken cancellationToken = default) where T : class
    {
        var entityType = typeof(T).Name;
        await LogAsync(
            AuditActionType.Update,
            entityType,
            entityId.ToString(),
            userId,
            $"Entidade {entityType} atualizada com ID {entityId}",
            AuditSeverity.Information,
            JsonSerializer.Serialize(new { previousData = previousEntity, currentData = currentEntity, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogEntityDeletedAsync<T>(Guid entityId, Guid userId, T entity, CancellationToken cancellationToken = default) where T : class
    {
        var entityType = typeof(T).Name;
        await LogAsync(
            AuditActionType.Delete,
            entityType,
            entityId.ToString(),
            userId,
            $"Entidade {entityType} excluída com ID {entityId}",
            AuditSeverity.Warning,
            JsonSerializer.Serialize(new { entityData = entity, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogSensitiveAccessAsync(string resourceType, string resourceId, Guid userId, string ipAddress, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.Read,
            resourceType,
            resourceId,
            userId,
            $"Acesso a recurso sensível {resourceType}:{resourceId} do IP {ipAddress}",
            AuditSeverity.Security,
            JsonSerializer.Serialize(new { ipAddress, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogAccessDeniedAsync(string resourceType, string resourceId, Guid userId, string reason, string ipAddress, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.PermissionDenied,
            resourceType,
            resourceId,
            userId,
            $"Acesso negado a {resourceType}:{resourceId} do IP {ipAddress}: {reason}",
            AuditSeverity.Warning,
            JsonSerializer.Serialize(new { reason, ipAddress, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogPermissionChangedAsync(Guid targetUserId, Guid adminUserId, string permissionType, string oldValue, string newValue, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.PermissionChanged,
            "Permission",
            permissionType,
            adminUserId,
            $"Permissão {permissionType} alterada para usuário {targetUserId}: {oldValue} → {newValue}",
            AuditSeverity.Security,
            JsonSerializer.Serialize(new { targetUserId, permissionType, oldValue, newValue, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogDataExportAsync(Guid userId, string dataType, Guid requestedByUserId, string ipAddress, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.DataExport,
            "PersonalData",
            userId.ToString(),
            requestedByUserId,
            $"Exportação de dados {dataType} para usuário {userId} solicitada por {requestedByUserId}",
            AuditSeverity.Security,
            JsonSerializer.Serialize(new { dataType, requestedByUserId, ipAddress, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task LogDataDeletionAsync(Guid userId, string dataType, Guid requestedByUserId, string reason, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.DataDeletion,
            "PersonalData",
            userId.ToString(),
            requestedByUserId,
            $"Exclusão de dados {dataType} para usuário {userId}: {reason}",
            AuditSeverity.Critical,
            JsonSerializer.Serialize(new { dataType, requestedByUserId, reason, timestamp = DateTime.UtcNow }),
            cancellationToken);
    }

    public async Task<(List<AuditLogEntity> logs, int totalCount)> SearchLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        AuditActionType? actionType = null,
        AuditSeverity? severity = null,
        Guid? userId = null,
        string? resourceType = null,
        string? ipAddress = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        // Aplicar filtros
        if (startDate.HasValue)
            query = query.Where(log => log.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(log => log.CreatedAt <= endDate.Value);

        if (actionType.HasValue)
            query = query.Where(log => log.ActionType == actionType.Value);

        if (severity.HasValue)
            query = query.Where(log => log.Severity == severity.Value);

        if (userId.HasValue)
            query = query.Where(log => log.UserId == userId.Value.ToString());

        if (!string.IsNullOrEmpty(resourceType))
            query = query.Where(log => log.ResourceType == resourceType);

        if (!string.IsNullOrEmpty(ipAddress))
            query = query.Where(log => log.ClientIpAddress == ipAddress);

        // Contar total
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação e ordenação
        var logs = await query
            .OrderByDescending(log => log.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var filters = $"startDate:{startDate}, endDate:{endDate}, actionType:{actionType}, severity:{severity}, userId:{userId}, resourceType:{resourceType}, ipAddress:{ipAddress}";
        LogSearchExecuted(_logger, filters, logs.Count, null);

        return (logs, totalCount);
    }

    public async Task<List<AuditLogEntity>> GetUserAuditLogsAsync(Guid userId, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(log => log.UserId == userId.ToString())
            .OrderByDescending(log => log.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLogEntity>> GetSecurityLogsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(log => log.CreatedAt >= startDate
                       && log.CreatedAt <= endDate
                       && (log.Severity == AuditSeverity.Security || log.Severity == AuditSeverity.Critical))
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task CleanupOldLogsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Logs informacionais: manter por 90 dias
            var informationCutoff = DateTime.UtcNow.AddDays(-90);
            var informationLogs = await _context.AuditLogs
                .Where(log => log.Severity == AuditSeverity.Information && log.CreatedAt < informationCutoff)
                .ToListAsync(cancellationToken);

            // Logs de warning: manter por 1 ano
            var warningCutoff = DateTime.UtcNow.AddDays(-365);
            var warningLogs = await _context.AuditLogs
                .Where(log => log.Severity == AuditSeverity.Warning && log.CreatedAt < warningCutoff)
                .ToListAsync(cancellationToken);

            // Logs de alta severidade: manter por 3 anos
            var securityCutoff = DateTime.UtcNow.AddDays(-1095);
            var securityLogs = await _context.AuditLogs
                .Where(log => log.Severity == AuditSeverity.Security && log.CreatedAt < securityCutoff)
                .ToListAsync(cancellationToken);

            // Logs críticos: nunca remover automaticamente (requer intervenção manual)

            var logsToRemove = informationLogs.Concat(warningLogs).Concat(securityLogs).ToList();

            if (logsToRemove.Count > 0)
            {
                _context.AuditLogs.RemoveRange(logsToRemove);
                await _context.SaveChangesAsync(cancellationToken);

                LogCleanupExecuted(_logger, logsToRemove.Count, null);
            }
        }
        catch (Exception ex)
        {
            LogCleanupError(_logger, ex.Message, ex);
            throw;
        }
    }

    // Métodos específicos para operações críticas de segurança
    public async Task LogSecurityEventAsync(string securityEvent, string details, Guid? userId = null, AuditSeverity severity = AuditSeverity.Warning, CancellationToken cancellationToken = default)
    {
        await LogAsync(
            AuditActionType.SecurityViolation,
            "Security",
            securityEvent,
            userId,
            details,
            severity,
            null,
            cancellationToken);
    }

    public async Task LogPrivacyOperationAsync(string operation, string dataCategory, Guid? userId, string details, CancellationToken cancellationToken = default)
    {
        var metadata = JsonSerializer.Serialize(new { operation, dataCategory });
        await LogAsync(
            AuditActionType.DataRequested,
            "Privacy",
            dataCategory,
            userId,
            details,
            AuditSeverity.Information,
            metadata,
            cancellationToken);
    }

    public async Task LogRateLimitViolationAsync(string ipAddress, string endpoint, string? userId = null, CancellationToken cancellationToken = default)
    {
        var metadata = JsonSerializer.Serialize(new { ipAddress, endpoint, timestamp = DateTime.UtcNow });
        await LogAsync(
            AuditActionType.RateLimitExceeded,
            "RateLimit",
            endpoint,
            userId != null ? Guid.Parse(userId) : null,
            $"Rate limit exceeded for IP {ipAddress} on endpoint {endpoint}",
            AuditSeverity.Warning,
            metadata,
            cancellationToken);
    }

    public async Task LogInputSanitizationAsync(string inputType, string threatType, string ipAddress, string? userId = null, CancellationToken cancellationToken = default)
    {
        var metadata = JsonSerializer.Serialize(new { inputType, threatType, ipAddress, timestamp = DateTime.UtcNow });
        await LogAsync(
            AuditActionType.SecurityViolation,
            "InputSanitization",
            threatType,
            userId != null ? Guid.Parse(userId) : null,
            $"Security threat detected: {threatType} in {inputType} from IP {ipAddress}",
            AuditSeverity.Warning,
            metadata,
            cancellationToken);
    }

    public async Task LogSuspiciousActivityAsync(string activityType, string details, string ipAddress, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var metadata = JsonSerializer.Serialize(new { activityType, ipAddress, timestamp = DateTime.UtcNow });
        await LogAsync(
            AuditActionType.SuspiciousActivity,
            "SuspiciousActivity",
            activityType,
            userId,
            details,
            AuditSeverity.Warning,
            metadata,
            cancellationToken);
    }
}
