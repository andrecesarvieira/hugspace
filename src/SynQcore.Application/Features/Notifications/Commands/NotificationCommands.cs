using MediatR;
using SynQcore.Application.DTOs.Notifications;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Commands;

/// <summary>
/// Command para criar notificação corporativa
/// </summary>
public record CreateNotificationCommand : IRequest<CreateNotificationResponse>
{
    /// <summary>
    /// Título da notificação
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Conteúdo da notificação
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Tipo de notificação
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Prioridade da notificação
    /// </summary>
    public string Priority { get; init; } = string.Empty;

    /// <summary>
    /// ID do departamento alvo (null para company-wide)
    /// </summary>
    public Guid? TargetDepartmentId { get; init; }

    /// <summary>
    /// Data de expiração
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Data de agendamento para envio
    /// </summary>
    public DateTimeOffset? ScheduledFor { get; init; }

    /// <summary>
    /// Requer aprovação gerencial
    /// </summary>
    public bool RequiresApproval { get; init; }

    /// <summary>
    /// Requer confirmação de leitura
    /// </summary>
    public bool RequiresAcknowledgment { get; init; }

    /// <summary>
    /// Canais de entrega habilitados
    /// </summary>
    public List<string> EnabledChannels { get; init; } = new();

    /// <summary>
    /// Código do template (opcional)
    /// </summary>
    public string? TemplateCode { get; init; }

    /// <summary>
    /// Dados para substituição de placeholders
    /// </summary>
    public Dictionary<string, string>? PlaceholderData { get; init; }
}

/// <summary>
/// Response para criação de notificação
/// </summary>
public class CreateNotificationResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ID da notificação criada
    /// </summary>
    public Guid? NotificationId { get; set; }

    /// <summary>
    /// Status da notificação
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Total de destinatários estimado
    /// </summary>
    public int? EstimatedRecipients { get; set; }

    /// <summary>
    /// Data prevista para envio
    /// </summary>
    public DateTimeOffset? SendDateTime { get; set; }
}

/// <summary>
/// Command para aprovar notificação
/// </summary>
public record ApproveNotificationCommand : IRequest<ApproveNotificationResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Aprovação (true) ou rejeição (false)
    /// </summary>
    public bool IsApproved { get; init; }

    /// <summary>
    /// Comentários do aprovador
    /// </summary>
    public string? Comments { get; init; }
}

/// <summary>
/// Response para aprovação de notificação
/// </summary>
public class ApproveNotificationResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Novo status da notificação
    /// </summary>
    public string? NewStatus { get; set; }

    /// <summary>
    /// Data de aprovação/rejeição
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; set; }
}

/// <summary>
/// Command para enviar notificação imediatamente
/// </summary>
public record SendNotificationCommand : IRequest<SendNotificationResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Forçar envio mesmo se agendada para o futuro
    /// </summary>
    public bool Force { get; init; }
}

/// <summary>
/// Response para envio de notificação
/// </summary>
public class SendNotificationResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Total de destinatários
    /// </summary>
    public int TotalRecipients { get; set; }

    /// <summary>
    /// Entregas iniciadas
    /// </summary>
    public int DeliveriesStarted { get; set; }

    /// <summary>
    /// Data/hora de início do envio
    /// </summary>
    public DateTimeOffset? SendStartedAt { get; set; }
}

/// <summary>
/// Command para cancelar notificação
/// </summary>
public record CancelNotificationCommand : IRequest<CancelNotificationResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Motivo do cancelamento
    /// </summary>
    public string? Reason { get; init; }
}

/// <summary>
/// Response para cancelamento de notificação
/// </summary>
public class CancelNotificationResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Data de cancelamento
    /// </summary>
    public DateTimeOffset? CancelledAt { get; set; }
}

/// <summary>
/// Command para marcar notificação como lida
/// </summary>
public record MarkNotificationAsReadCommand : IRequest<MarkNotificationAsReadResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// ID do funcionário que leu
    /// </summary>
    public Guid EmployeeId { get; init; }
}

/// <summary>
/// Response para marcar como lida
/// </summary>
public class MarkNotificationAsReadResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Data de leitura
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }
}

/// <summary>
/// Command para confirmar notificação (acknowledge)
/// </summary>
public record AcknowledgeNotificationCommand : IRequest<AcknowledgeNotificationResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// ID do funcionário que confirmou
    /// </summary>
    public Guid EmployeeId { get; init; }

    /// <summary>
    /// Comentários opcionais
    /// </summary>
    public string? Comments { get; init; }
}

/// <summary>
/// Response para confirmação de notificação
/// </summary>
public class AcknowledgeNotificationResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Data de confirmação
    /// </summary>
    public DateTimeOffset? AcknowledgedAt { get; set; }
}

/// <summary>
/// Command para atualização de notificação (apenas rascunhos)
/// </summary>
public record UpdateNotificationCommand : IRequest<UpdateNotificationResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Novo título (opcional)
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Novo conteúdo (opcional)
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Novo tipo (opcional)
    /// </summary>
    public NotificationType? Type { get; init; }

    /// <summary>
    /// Nova prioridade (opcional)
    /// </summary>
    public NotificationPriority? Priority { get; init; }

    /// <summary>
    /// Novo departamento alvo (opcional)
    /// </summary>
    public Guid? TargetDepartmentId { get; init; }

    /// <summary>
    /// Novos canais habilitados (opcional)
    /// </summary>
    public NotificationChannels? EnabledChannels { get; init; }

    /// <summary>
    /// Nova data de agendamento (opcional)
    /// </summary>
    public DateTimeOffset? ScheduledFor { get; init; }

    /// <summary>
    /// Nova data de expiração (opcional)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Requer aprovação (opcional)
    /// </summary>
    public bool? RequiresApproval { get; init; }

    /// <summary>
    /// Requer confirmação (opcional)
    /// </summary>
    public bool? RequiresAcknowledgment { get; init; }

    /// <summary>
    /// Novos metadados (opcional)
    /// </summary>
    public string? Metadata { get; init; }
}

/// <summary>
/// Response para atualização de notificação
/// </summary>
public class UpdateNotificationResponse
{
    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

