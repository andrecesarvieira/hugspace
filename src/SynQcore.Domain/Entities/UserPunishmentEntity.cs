/*
 * SynQcore - Corporate Social Network
 *
 * User Punishment Entity - Sistema de punições de usuários
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using SynQcore.Domain.Common;
using System.ComponentModel.DataAnnotations;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Entidade para gerenciar punições de usuários no sistema
/// </summary>
public class UserPunishmentEntity : BaseEntity
{
    /// <summary>
    /// ID do funcionário punido
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// ID da moderação que originou a punição
    /// </summary>
    public Guid? ModerationId { get; set; }

    /// <summary>
    /// ID do funcionário que aplicou a punição
    /// </summary>
    public Guid AppliedByEmployeeId { get; set; }

    /// <summary>
    /// Tipo da punição
    /// </summary>
    [Required]
    public PunishmentType Type { get; set; }

    /// <summary>
    /// Status da punição
    /// </summary>
    [Required]
    public PunishmentStatus Status { get; set; } = PunishmentStatus.Active;

    /// <summary>
    /// Severidade da punição
    /// </summary>
    [Required]
    public PunishmentSeverity Severity { get; set; }

    /// <summary>
    /// Razão da punição
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Detalhes adicionais da punição
    /// </summary>
    [MaxLength(2000)]
    public string? Details { get; set; }

    /// <summary>
    /// Data/hora de início da punição
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data/hora de fim da punição (null para permanente)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Se a punição é permanente
    /// </summary>
    public bool IsPermanent { get; set; }

    /// <summary>
    /// Restrições específicas aplicadas
    /// </summary>
    public string? Restrictions { get; set; }

    /// <summary>
    /// Número de avisos anteriores
    /// </summary>
    public int PreviousWarnings { get; set; }

    /// <summary>
    /// É uma reincidência?
    /// </summary>
    public bool IsRecurrence { get; set; }

    /// <summary>
    /// Pontos de infração acumulados
    /// </summary>
    public int InfractionPoints { get; set; }

    /// <summary>
    /// Data/hora da notificação ao usuário
    /// </summary>
    public DateTime? NotifiedAt { get; set; }

    /// <summary>
    /// Data/hora do reconhecimento pelo usuário
    /// </summary>
    public DateTime? AcknowledgedAt { get; set; }

    /// <summary>
    /// Método de notificação usado
    /// </summary>
    [MaxLength(50)]
    public string? NotificationMethod { get; set; }

    /// <summary>
    /// IP do moderador que aplicou a punição
    /// </summary>
    [MaxLength(45)]
    public string? ModeratorIpAddress { get; set; }

    /// <summary>
    /// Dados adicionais em JSON
    /// </summary>
    public string? AdditionalData { get; set; }

    // Navegação
    /// <summary>
    /// Funcionário punido
    /// </summary>
    public Employee? Employee { get; set; }

    /// <summary>
    /// Moderação que originou a punição
    /// </summary>
    public ModerationEntity? Moderation { get; set; }

    /// <summary>
    /// Funcionário que aplicou a punição
    /// </summary>
    public Employee? AppliedByEmployee { get; set; }
}

/// <summary>
/// Tipos de punição disponíveis
/// </summary>
public enum PunishmentType
{
    Warning,           // Aviso
    Mute,              // Silenciamento (não pode postar)
    Suspension,        // Suspensão temporária
    Ban,               // Banimento permanente
    Shadowban,         // Shadowban (posts não aparecem para outros)
    FeatureRestriction, // Restrição de funcionalidades específicas
    ContentModeration,  // Futuros posts precisam aprovação
    AccountReview,     // Conta em revisão
    DepartmentRestriction, // Restrição departamental
    TimeRestriction    // Restrição de horário de acesso
}

/// <summary>
/// Status da punição
/// </summary>
public enum PunishmentStatus
{
    Active,            // Ativa
    Expired,           // Expirada
    Revoked,           // Revogada
    Appealed,          // Em appeal
    Modified,          // Modificada
    Suspended,         // Suspensa temporariamente
    UnderReview        // Em revisão
}

/// <summary>
/// Severidade da punição
/// </summary>
public enum PunishmentSeverity
{
    Verbal,            // Verbal/aviso
    Light,             // Leve
    Moderate,          // Moderada
    Severe,            // Severa
    Critical           // Crítica
}
