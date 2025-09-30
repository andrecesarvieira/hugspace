/*
 * SynQcore - Corporate Social Network
 *
 * Moderation DTOs - Sistema de moderação corporativa
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using SynQcore.Application.Common.DTOs;

namespace SynQcore.Application.Features.Moderation.DTOs;

/// <summary>
/// DTO principal para moderação
/// </summary>
public record ModerationDto
{
    public Guid Id { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public Guid ContentAuthorId { get; init; }
    public string ContentAuthorName { get; init; } = string.Empty;
    public Guid? ReportedByEmployeeId { get; init; }
    public string? ReportedByEmployeeName { get; init; }
    public Guid? ModeratorId { get; init; }
    public string? ModeratorName { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string? ActionTaken { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? ModeratorNotes { get; init; }
    public bool IsAutomaticDetection { get; init; }
    public int? AutoDetectionScore { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModerationDate { get; init; }
    public DateTime? ExpirationDate { get; init; }
}

/// <summary>
/// DTO para listagem da fila de moderação
/// </summary>
public record ModerationQueueDto
{
    public Guid Id { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public string ContentAuthorName { get; init; } = string.Empty;
    public string? ReportedByEmployeeName { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public bool IsAutomaticDetection { get; init; }
    public int? AutoDetectionScore { get; init; }
    public DateTime CreatedAt { get; init; }
    public int DaysWaiting { get; init; }
}

/// <summary>
/// DTO para ação de moderação
/// </summary>
public record ModerationActionDto
{
    public Guid ModerationId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime? ExpirationDate { get; init; }
}

/// <summary>
/// Request para busca na fila de moderação
/// </summary>
public record ModerationQueueRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? Category { get; init; }
    public string? Severity { get; init; }
    public string? ContentType { get; init; }
    public bool? IsAutomaticDetection { get; init; }
    public Guid? ModeratorId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int? MinDaysWaiting { get; init; }
    public int? MaxDaysWaiting { get; init; }
}

/// <summary>
/// DTO para estatísticas de moderação
/// </summary>
public record ModerationStatsDto
{
    public int TotalPending { get; init; }
    public int TotalUnderReview { get; init; }
    public int TotalApproved { get; init; }
    public int TotalRejected { get; init; }
    public int TotalEscalated { get; init; }
    public int AverageProcessingDays { get; init; }
    public Dictionary<string, int> CategoryStats { get; init; } = new();
    public Dictionary<string, int> SeverityStats { get; init; } = new();
}
