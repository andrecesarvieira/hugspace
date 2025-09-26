namespace SynQcore.Domain.Entities;

/// <summary>
/// Tipos de notificação corporativa
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Anúncio geral da empresa
    /// </summary>
    CompanyAnnouncement = 1,

    /// <summary>
    /// Atualização de política
    /// </summary>
    PolicyUpdate = 2,

    /// <summary>
    /// Comunicado de emergência
    /// </summary>
    Emergency = 3,

    /// <summary>
    /// Comunicado de sistema/manutenção
    /// </summary>
    SystemNotification = 4,

    /// <summary>
    /// Comunicado de RH
    /// </summary>
    HumanResources = 5,

    /// <summary>
    /// Comunicado departamental
    /// </summary>
    DepartmentUpdate = 6,

    /// <summary>
    /// Notificação de projeto
    /// </summary>
    ProjectUpdate = 7,

    /// <summary>
    /// Comunicado de segurança
    /// </summary>
    Security = 8,

    /// <summary>
    /// Comunicado executivo
    /// </summary>
    ExecutiveCommunication = 9,

    /// <summary>
    /// Comunicado de treinamento
    /// </summary>
    Training = 10
}

/// <summary>
/// Prioridades de notificação
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Prioridade baixa - informativo
    /// </summary>
    Low = 1,

    /// <summary>
    /// Prioridade normal - padrão
    /// </summary>
    Normal = 2,

    /// <summary>
    /// Prioridade alta - importante
    /// </summary>
    High = 3,

    /// <summary>
    /// Prioridade crítica - urgente
    /// </summary>
    Critical = 4,

    /// <summary>
    /// Emergência - máxima prioridade
    /// </summary>
    Emergency = 5
}

/// <summary>
/// Status da notificação
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Rascunho - ainda não enviada
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Agendada para envio futuro
    /// </summary>
    Scheduled = 2,

    /// <summary>
    /// Aguardando aprovação gerencial
    /// </summary>
    PendingApproval = 3,

    /// <summary>
    /// Aprovada e pronta para envio
    /// </summary>
    Approved = 4,

    /// <summary>
    /// Em processo de envio
    /// </summary>
    Sending = 5,

    /// <summary>
    /// Enviada com sucesso
    /// </summary>
    Sent = 6,

    /// <summary>
    /// Rejeitada pelo aprovador
    /// </summary>
    Rejected = 7,

    /// <summary>
    /// Cancelada antes do envio
    /// </summary>
    Cancelled = 8,

    /// <summary>
    /// Expirada sem ser enviada
    /// </summary>
    Expired = 9,

    /// <summary>
    /// Falha no envio
    /// </summary>
    Failed = 10
}

/// <summary>
/// Canais de notificação (flags para múltipla seleção)
/// </summary>
[Flags]
public enum NotificationChannels
{
    /// <summary>
    /// Nenhum canal
    /// </summary>
    None = 0,

    /// <summary>
    /// Notificação in-app via SignalR
    /// </summary>
    InApp = 1,

    /// <summary>
    /// Notificação por email
    /// </summary>
    Email = 2,

    /// <summary>
    /// Notificação push mobile/browser
    /// </summary>
    Push = 4,

    /// <summary>
    /// Notificação por SMS
    /// </summary>
    SMS = 8,

    /// <summary>
    /// Webhook para sistemas externos
    /// </summary>
    Webhook = 16,

    /// <summary>
    /// Integração com Teams
    /// </summary>
    Teams = 32,

    /// <summary>
    /// Integração com Slack
    /// </summary>
    Slack = 64,

    /// <summary>
    /// Todos os canais disponíveis
    /// </summary>
    All = InApp | Email | Push | SMS | Webhook | Teams | Slack
}

/// <summary>
/// Canal específico usado na entrega
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    /// Notificação in-app via SignalR
    /// </summary>
    InApp = 1,

    /// <summary>
    /// Email corporativo
    /// </summary>
    Email = 2,

    /// <summary>
    /// Push notification mobile
    /// </summary>
    MobilePush = 3,

    /// <summary>
    /// Push notification browser
    /// </summary>
    BrowserPush = 4,

    /// <summary>
    /// SMS corporativo
    /// </summary>
    SMS = 5,

    /// <summary>
    /// Webhook para sistema externo
    /// </summary>
    Webhook = 6,

    /// <summary>
    /// Microsoft Teams
    /// </summary>
    Teams = 7,

    /// <summary>
    /// Slack
    /// </summary>
    Slack = 8
}

/// <summary>
/// Status de entrega da notificação
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Aguardando entrega
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Em processo de entrega
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Entregue com sucesso
    /// </summary>
    Delivered = 3,

    /// <summary>
    /// Lida pelo destinatário
    /// </summary>
    Read = 4,

    /// <summary>
    /// Confirmada pelo destinatário
    /// </summary>
    Acknowledged = 5,

    /// <summary>
    /// Falha na entrega
    /// </summary>
    Failed = 6,

    /// <summary>
    /// Descartada (usuário não elegível)
    /// </summary>
    Discarded = 7,

    /// <summary>
    /// Expirada antes da entrega
    /// </summary>
    Expired = 8,

    /// <summary>
    /// Tentativa de entrega em progresso
    /// </summary>
    Retrying = 9
}