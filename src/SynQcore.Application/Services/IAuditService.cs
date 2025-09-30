using SynQcore.Domain.Entities;

namespace SynQcore.Application.Services;

/// <summary>
/// Interface para serviço de auditoria corporativa
/// Responsável por registrar eventos de segurança e ações importantes do sistema
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Registra um evento de auditoria de forma assíncrona
    /// </summary>
    /// <param name="actionType">Tipo da ação executada</param>
    /// <param name="resourceType">Tipo do recurso afetado</param>
    /// <param name="resourceId">ID do recurso afetado</param>
    /// <param name="userId">ID do usuário que executou a ação</param>
    /// <param name="details">Detalhes adicionais da ação</param>
    /// <param name="severity">Severidade do evento (padrão: Information)</param>
    /// <param name="metadata">Metadados adicionais como JSON</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task LogAsync(
        AuditActionType actionType,
        string resourceType,
        string? resourceId,
        Guid? userId,
        string details,
        AuditSeverity severity = AuditSeverity.Information,
        string? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra um evento de login bem-sucedido
    /// </summary>
    Task LogLoginSuccessAsync(Guid userId, string email, string ipAddress, string? userAgent = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra uma tentativa de login falhada
    /// </summary>
    Task LogLoginFailureAsync(string email, string ipAddress, string reason, string? userAgent = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra logout do usuário
    /// </summary>
    Task LogLogoutAsync(Guid userId, string email, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra criação de entidade
    /// </summary>
    Task LogEntityCreatedAsync<T>(Guid entityId, Guid userId, T entity, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Registra atualização de entidade
    /// </summary>
    Task LogEntityUpdatedAsync<T>(Guid entityId, Guid userId, T previousEntity, T currentEntity, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Registra exclusão de entidade
    /// </summary>
    Task LogEntityDeletedAsync<T>(Guid entityId, Guid userId, T entity, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Registra acesso a recurso sensível
    /// </summary>
    Task LogSensitiveAccessAsync(string resourceType, string resourceId, Guid userId, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra tentativa de acesso negado
    /// </summary>
    Task LogAccessDeniedAsync(string resourceType, string resourceId, Guid userId, string reason, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra alteração de permissões
    /// </summary>
    Task LogPermissionChangedAsync(Guid targetUserId, Guid adminUserId, string permissionType, string oldValue, string newValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra exportação de dados (LGPD)
    /// </summary>
    Task LogDataExportAsync(Guid userId, string dataType, Guid requestedByUserId, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra exclusão de dados (LGPD)
    /// </summary>
    Task LogDataDeletionAsync(Guid userId, string dataType, Guid requestedByUserId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs de auditoria com paginação e filtros
    /// </summary>
    Task<(List<AuditLogEntity> logs, int totalCount)> SearchLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        AuditActionType? actionType = null,
        AuditSeverity? severity = null,
        Guid? userId = null,
        string? resourceType = null,
        string? ipAddress = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs de auditoria para um usuário específico
    /// </summary>
    Task<List<AuditLogEntity>> GetUserAuditLogsAsync(Guid userId, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca logs de segurança (alta e crítica severidade)
    /// </summary>
    Task<List<AuditLogEntity>> GetSecurityLogsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove logs antigos conforme política de retenção
    /// </summary>
    Task CleanupOldLogsAsync(CancellationToken cancellationToken = default);

    // Métodos específicos para operações críticas de segurança
    Task LogSecurityEventAsync(string securityEvent, string details, Guid? userId = null, AuditSeverity severity = AuditSeverity.Warning, CancellationToken cancellationToken = default);
    Task LogPrivacyOperationAsync(string operation, string dataCategory, Guid? userId, string details, CancellationToken cancellationToken = default);
    Task LogRateLimitViolationAsync(string ipAddress, string endpoint, string? userId = null, CancellationToken cancellationToken = default);
    Task LogInputSanitizationAsync(string inputType, string threatType, string ipAddress, string? userId = null, CancellationToken cancellationToken = default);
    Task LogSuspiciousActivityAsync(string activityType, string details, string ipAddress, Guid? userId = null, CancellationToken cancellationToken = default);
}
