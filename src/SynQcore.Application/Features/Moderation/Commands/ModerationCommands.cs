using MediatR;
using SynQcore.Application.Features.Moderation.DTOs;

namespace SynQcore.Application.Features.Moderation.Commands;

/// <summary>
/// Comando para processar uma solicitação de moderação
/// </summary>
public record ProcessModerationCommand : IRequest<ModerationDto>
{
    public Guid ItemId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? Reason { get; init; }
    public string? Comments { get; init; }
    public Guid ModeratorId { get; init; }
    public string Severity { get; init; } = "Média";
    public string Category { get; init; } = string.Empty;
}

/// <summary>
/// Comando para atualizar status de uma moderação existente
/// </summary>
public record UpdateModerationCommand : IRequest<ModerationDto?>
{
    public Guid ModerationId { get; init; }
    public string? NewStatus { get; init; }
    public string? ModeratorComments { get; init; }
    public Guid ModeratorId { get; init; }
    public DateTime? ResolvedAt { get; init; }
}

/// <summary>
/// Comando para escalar uma moderação para nível superior
/// </summary>
public record EscalateModerationCommand : IRequest<ModerationDto?>
{
    public Guid ModerationId { get; init; }
    public string EscalationReason { get; init; } = string.Empty;
    public Guid EscalatedBy { get; init; }
    public string? AdditionalContext { get; init; }
    public string Priority { get; init; } = "Alta";
}

/// <summary>
/// Comando para criar uma nova solicitação de moderação
/// </summary>
public record CreateModerationRequestCommand : IRequest<ModerationDto>
{
    public Guid ItemId { get; init; }
    public string ItemType { get; init; } = string.Empty; // "Post", "Comment", "User"
    public string ReportReason { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid ReportedBy { get; init; }
    public string Severity { get; init; } = "Média";
    public string Category { get; init; } = string.Empty;
}

/// <summary>
/// Comando para ações em lote de moderação
/// </summary>
public record BulkModerationCommand : IRequest<List<ModerationDto>>
{
    public List<Guid> ModerationIds { get; init; } = new();
    public string Action { get; init; } = string.Empty;
    public string? Reason { get; init; }
    public Guid ModeratorId { get; init; }
}

/// <summary>
/// Comando para arquivar moderações antigas
/// </summary>
public record ArchiveOldModerationsCommand : IRequest<int>
{
    public int DaysOld { get; init; } = 90;
    public Guid RequestedBy { get; init; }
}
