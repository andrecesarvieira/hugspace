/*
 * SynQcore - Corporate Social Network API
 *
 * Audit Log Entity - Tracks security and compliance events
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Entidade de log de auditoria para compliance corporativo e segurança
/// </summary>
public class AuditLogEntity : BaseEntity
{
    /// <summary>
    /// ID do usuário que executou a ação (se autenticado)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Nome do usuário ou "Anonymous"
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Papel/função do usuário no momento da ação
    /// </summary>
    public string? UserRole { get; set; }

    /// <summary>
    /// Tipo de ação executada
    /// </summary>
    public AuditActionType ActionType { get; set; }

    /// <summary>
    /// Recurso/entidade afetada
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// ID do recurso afetado
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// Detalhes adicionais da ação em JSON
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Resultado da ação (sucesso/falha)
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de erro (se falhou)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Endereço IP do cliente
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// User Agent do cliente
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// ID de correlação da requisição
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Caminho da requisição HTTP
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// Método HTTP utilizado
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Duração da operação em milissegundos
    /// </summary>
    public long? DurationMs { get; set; }

    /// <summary>
    /// Nível de severidade do evento
    /// </summary>
    public AuditSeverity Severity { get; set; } = AuditSeverity.Information;

    /// <summary>
    /// Categoria do evento para classificação
    /// </summary>
    public AuditCategory Category { get; set; }

    /// <summary>
    /// Tags adicionais para busca e filtragem
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Data de retenção - quando este log pode ser removido
    /// </summary>
    public DateTime? RetentionDate { get; set; }

    /// <summary>
    /// Indica se o evento requer atenção especial
    /// </summary>
    public bool RequiresAttention { get; set; }

    /// <summary>
    /// Data em que foi revisado (para eventos que requerem atenção)
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// ID do usuário que revisou
    /// </summary>
    public string? ReviewedBy { get; set; }

    /// <summary>
    /// Notas da revisão
    /// </summary>
    public string? ReviewNotes { get; set; }
}

/// <summary>
/// Tipos de ações de auditoria
/// </summary>
public enum AuditActionType
{
    // Autenticação e Autorização
    Login,
    Logout,
    LoginFailed,
    PasswordChange,
    PasswordReset,
    AccountLocked,
    PermissionGranted,
    PermissionDenied,

    // CRUD Operations
    Create,
    Read,
    Update,
    Delete,
    BulkUpdate,
    BulkDelete,

    // Administração
    UserCreated,
    UserUpdated,
    UserDeleted,
    RoleAssigned,
    RoleRemoved,
    PermissionChanged,

    // Sistema
    SystemStart,
    SystemStop,
    ConfigurationChanged,
    BackupCreated,
    BackupRestored,

    // Segurança
    SecurityViolation,
    RateLimitExceeded,
    UnauthorizedAccess,
    SuspiciousActivity,
    DataExport,
    DataDeletion,

    // Compliance
    DataRequested,
    ConsentGiven,
    ConsentWithdrawn,
    PrivacySettingsChanged,

    // Outros
    FileUploaded,
    FileDownloaded,
    ReportGenerated,
    EmailSent,
    NotificationSent
}

/// <summary>
/// Níveis de severidade para logs de auditoria
/// </summary>
public enum AuditSeverity
{
    Information = 0,
    Warning = 1,
    Error = 2,
    Critical = 3,
    Security = 4
}

/// <summary>
/// Categorias de auditoria para organização
/// </summary>
public enum AuditCategory
{
    Authentication,
    Authorization,
    DataAccess,
    DataModification,
    SystemOperation,
    Security,
    Compliance,
    Administration,
    FileManagement,
    Communication,
    Reporting,
    Integration,
    Performance,
    Error
}
