namespace SynQcore.Application.DTOs.Notifications;

/// <summary>
/// DTO para notificação corporativa
/// </summary>
public class CorporateNotificationDto
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid Id { get; set; }

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
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Prioridade da notificação
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Status da notificação
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Funcionário que criou
    /// </summary>
    public EmployeeBasicDto CreatedBy { get; set; } = null!;

    /// <summary>
    /// Departamento alvo (se aplicável)
    /// </summary>
    public DepartmentBasicDto? TargetDepartment { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Data de expiração
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Data de agendamento
    /// </summary>
    public DateTimeOffset? ScheduledFor { get; set; }

    /// <summary>
    /// Requer aprovação
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Funcionário aprovador (se aplicável)
    /// </summary>
    public EmployeeBasicDto? ApprovedBy { get; set; }

    /// <summary>
    /// Data de aprovação
    /// </summary>
    public DateTimeOffset? ApprovedAt { get; set; }

    /// <summary>
    /// Requer confirmação de leitura
    /// </summary>
    public bool RequiresAcknowledgment { get; set; }

    /// <summary>
    /// Canais habilitados
    /// </summary>
    public List<string> EnabledChannels { get; set; } = new();

    /// <summary>
    /// Estatísticas de entrega
    /// </summary>
    public NotificationStatsDto? Stats { get; set; }
}

/// <summary>
/// DTO básico para funcionário em notificações
/// </summary>
public class EmployeeBasicDto
{
    /// <summary>
    /// ID do funcionário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Cargo/posição
    /// </summary>
    public string? Position { get; set; }
}

/// <summary>
/// DTO básico para departamento em notificações
/// </summary>
public class DepartmentBasicDto
{
    /// <summary>
    /// ID do departamento
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do departamento
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código do departamento
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// DTO para estatísticas de entrega de notificação
/// </summary>
public class NotificationStatsDto
{
    /// <summary>
    /// Total de destinatários
    /// </summary>
    public int TotalRecipients { get; set; }

    /// <summary>
    /// Total entregue
    /// </summary>
    public int TotalDelivered { get; set; }

    /// <summary>
    /// Total lido
    /// </summary>
    public int TotalRead { get; set; }

    /// <summary>
    /// Total confirmado
    /// </summary>
    public int TotalAcknowledged { get; set; }

    /// <summary>
    /// Total com falha
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Taxa de entrega (%)
    /// </summary>
    public decimal DeliveryRate { get; set; }

    /// <summary>
    /// Taxa de leitura (%)
    /// </summary>
    public decimal ReadRate { get; set; }

    /// <summary>
    /// Taxa de confirmação (%)
    /// </summary>
    public decimal AcknowledgmentRate { get; set; }

    /// <summary>
    /// Estatísticas por canal
    /// </summary>
    public List<ChannelStatsDto> ChannelStats { get; set; } = new();
}

/// <summary>
/// DTO para estatísticas por canal
/// </summary>
public class ChannelStatsDto
{
    /// <summary>
    /// Canal
    /// </summary>
    public string Channel { get; set; } = string.Empty;

    /// <summary>
    /// Total entregue neste canal
    /// </summary>
    public int Delivered { get; set; }

    /// <summary>
    /// Total com falha neste canal
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Taxa de sucesso (%)
    /// </summary>
    public decimal SuccessRate { get; set; }
}

/// <summary>
/// DTO para entrega de notificação
/// </summary>
public class NotificationDeliveryDto
{
    /// <summary>
    /// ID da entrega
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Funcionário destinatário
    /// </summary>
    public EmployeeBasicDto Employee { get; set; } = null!;

    /// <summary>
    /// Status da entrega
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Canal utilizado
    /// </summary>
    public string Channel { get; set; } = string.Empty;

    /// <summary>
    /// Data de entrega
    /// </summary>
    public DateTimeOffset? DeliveredAt { get; set; }

    /// <summary>
    /// Data de leitura
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }

    /// <summary>
    /// Data de confirmação
    /// </summary>
    public DateTimeOffset? AcknowledgedAt { get; set; }

    /// <summary>
    /// Tentativas de entrega
    /// </summary>
    public int DeliveryAttempts { get; set; }

    /// <summary>
    /// Detalhes de erro
    /// </summary>
    public string? ErrorDetails { get; set; }
}

/// <summary>
/// DTO para template de notificação
/// </summary>
public class NotificationTemplateDto
{
    /// <summary>
    /// ID do template
    /// </summary>
    public Guid Id { get; set; }

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
    /// Template do título
    /// </summary>
    public string TitleTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Template do conteúdo
    /// </summary>
    public string ContentTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Tipo padrão
    /// </summary>
    public string DefaultType { get; set; } = string.Empty;

    /// <summary>
    /// Prioridade padrão
    /// </summary>
    public string DefaultPriority { get; set; } = string.Empty;

    /// <summary>
    /// Canais padrão
    /// </summary>
    public List<string> DefaultChannels { get; set; } = new();

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
    public bool IsActive { get; set; }

    /// <summary>
    /// Placeholders disponíveis
    /// </summary>
    public List<string> AvailablePlaceholders { get; set; } = new();

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Contadores de notificações por status
/// </summary>
public class NotificationCountsDto
{
    /// <summary>
    /// Total de notificações
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Não lidas
    /// </summary>
    public int Unread { get; set; }

    /// <summary>
    /// Lidas
    /// </summary>
    public int Read { get; set; }

    /// <summary>
    /// Confirmadas
    /// </summary>
    public int Acknowledged { get; set; }

    /// <summary>
    /// Expiradas
    /// </summary>
    public int Expired { get; set; }
}