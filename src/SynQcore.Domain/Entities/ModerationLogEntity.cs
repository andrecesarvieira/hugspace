/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Log Entity - Log de ações de moderação
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using SynQcore.Domain.Common;
using System.ComponentModel.DataAnnotations;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Log de ações de moderação para trilha de auditoria
/// </summary>
public class ModerationLogEntity : BaseEntity
{
    /// <summary>
    /// ID da moderação relacionada
    /// </summary>
    [Required]
    public Guid ModerationId { get; set; }

    /// <summary>
    /// ID do funcionário que executou a ação (moderador/sistema)
    /// </summary>
    public Guid? ActionByEmployeeId { get; set; }

    /// <summary>
    /// Ação executada
    /// </summary>
    [Required]
    public ModerationLogAction Action { get; set; }

    /// <summary>
    /// Status anterior da moderação
    /// </summary>
    public ModerationStatus? PreviousStatus { get; set; }

    /// <summary>
    /// Novo status da moderação
    /// </summary>
    public ModerationStatus? NewStatus { get; set; }

    /// <summary>
    /// Notas da ação executada
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Dados adicionais em JSON
    /// </summary>
    public string? AdditionalData { get; set; }

    /// <summary>
    /// IP do usuário que executou a ação
    /// </summary>
    [MaxLength(45)]
    public string? UserIpAddress { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Se a ação foi executada automaticamente
    /// </summary>
    public bool IsAutomaticAction { get; set; }

    // Navegação
    /// <summary>
    /// Moderação relacionada
    /// </summary>
    public ModerationEntity? Moderation { get; set; }

    /// <summary>
    /// Funcionário que executou a ação
    /// </summary>
    public Employee? ActionByEmployee { get; set; }
}

/// <summary>
/// Ações que podem ser logadas no sistema de moderação
/// </summary>
public enum ModerationLogAction
{
    Created,           // Moderação criada
    StatusChanged,     // Status alterado
    Assigned,          // Atribuído a moderador
    Reviewed,          // Revisado
    Approved,          // Aprovado
    Rejected,          // Rejeitado
    Escalated,         // Escalado
    ActionExecuted,    // Ação executada (ban, warning, etc.)
    AppealSubmitted,   // Appeal submetido
    AppealReviewed,    // Appeal revisado
    Expired,           // Expirado
    SystemAutoAction,  // Ação automática do sistema
    NotesAdded,        // Notas adicionadas
    PriorityChanged,   // Prioridade alterada
    CategoryChanged,   // Categoria alterada
    SeverityChanged    // Severidade alterada
}
