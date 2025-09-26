using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Notificação corporativa para funcionários
/// Suporte para diferentes tipos de notificação e canais de entrega
/// </summary>
public class CorporateNotification : BaseEntity
{
    /// <summary>
    /// Título da notificação
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Conteúdo da notificação
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de notificação
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Prioridade da notificação
    /// </summary>
    public NotificationPriority Priority { get; set; }

    /// <summary>
    /// Status da notificação
    /// </summary>
    public NotificationStatus Status { get; set; }

    /// <summary>
    /// ID do funcionário que criou a notificação
    /// </summary>
    public Guid CreatedByEmployeeId { get; set; }

    /// <summary>
    /// Funcionário que criou a notificação
    /// </summary>
    public Employee CreatedByEmployee { get; set; } = null!;

    /// <summary>
    /// ID do departamento alvo (null para company-wide)
    /// </summary>
    public Guid? TargetDepartmentId { get; set; }

    /// <summary>
    /// Departamento alvo da notificação
    /// </summary>
    public Department? TargetDepartment { get; set; }

    /// <summary>
    /// Data de expiração da notificação
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Data de agendamento para envio
    /// </summary>
    public DateTimeOffset? ScheduledFor { get; set; }

    /// <summary>
    /// Requer aprovação gerencial
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// ID do aprovador (se requer aprovação)
    /// </summary>
    public Guid? ApprovedByEmployeeId { get; set; }

    /// <summary>
    /// Funcionário que aprovou a notificação
    /// </summary>
    public Employee? ApprovedByEmployee { get; set; }

    /// <summary>
    /// Data da aprovação
    /// </summary>
    public DateTimeOffset? ApprovedAt { get; set; }

    /// <summary>
    /// Requer confirmação de leitura
    /// </summary>
    public bool RequiresAcknowledgment { get; set; }

    /// <summary>
    /// Canais de entrega configurados
    /// </summary>
    public NotificationChannels EnabledChannels { get; set; }

    /// <summary>
    /// Metadados adicionais em JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Entregas da notificação para funcionários
    /// </summary>
    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
}

/// <summary>
/// Entrega de notificação para um funcionário específico
/// </summary>
public class NotificationDelivery : BaseEntity
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Notificação corporativa
    /// </summary>
    public CorporateNotification Notification { get; set; } = null!;

    /// <summary>
    /// ID do funcionário destinatário
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário destinatário
    /// </summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// Status da entrega
    /// </summary>
    public DeliveryStatus Status { get; set; }

    /// <summary>
    /// Canal utilizado para entrega
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Data de entrega
    /// </summary>
    public DateTimeOffset? DeliveredAt { get; set; }

    /// <summary>
    /// Data de leitura
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }

    /// <summary>
    /// Data de confirmação (se requerida)
    /// </summary>
    public DateTimeOffset? AcknowledgedAt { get; set; }

    /// <summary>
    /// Tentativas de entrega
    /// </summary>
    public int DeliveryAttempts { get; set; }

    /// <summary>
    /// Próxima tentativa de entrega
    /// </summary>
    public DateTimeOffset? NextAttemptAt { get; set; }

    /// <summary>
    /// Detalhes de erro na entrega
    /// </summary>
    public string? ErrorDetails { get; set; }

    /// <summary>
    /// Dados específicos do canal (email ID, push token, etc.)
    /// </summary>
    public string? ChannelData { get; set; }
}

/// <summary>
/// Template de notificação para padronização corporativa
/// </summary>
public class NotificationTemplate : BaseEntity
{
    /// <summary>
    /// Nome do template
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código único do template
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Categoria do template
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Template do título (suporte a placeholders)
    /// </summary>
    public string TitleTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Template do conteúdo (suporte a placeholders)
    /// </summary>
    public string ContentTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Template HTML para email (opcional)
    /// </summary>
    public string? EmailTemplate { get; set; }

    /// <summary>
    /// Tipo padrão de notificação
    /// </summary>
    public NotificationType DefaultType { get; set; }

    /// <summary>
    /// Prioridade padrão
    /// </summary>
    public NotificationPriority DefaultPriority { get; set; }

    /// <summary>
    /// Canais habilitados por padrão
    /// </summary>
    public NotificationChannels DefaultChannels { get; set; }

    /// <summary>
    /// Requer aprovação por padrão
    /// </summary>
    public bool DefaultRequiresApproval { get; set; }

    /// <summary>
    /// Requer confirmação por padrão
    /// </summary>
    public bool DefaultRequiresAcknowledgment { get; set; }

    /// <summary>
    /// Template está ativo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Placeholders disponíveis no template
    /// </summary>
    public string? AvailablePlaceholders { get; set; }
}