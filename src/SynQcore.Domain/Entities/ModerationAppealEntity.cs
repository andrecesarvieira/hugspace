/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Appeal Entity - Sistema de recursos/appeals de moderação
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using SynQcore.Domain.Common;
using System.ComponentModel.DataAnnotations;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Entidade para appeals/recursos contra decisões de moderação
/// </summary>
public class ModerationAppealEntity : BaseEntity
{
    /// <summary>
    /// ID da moderação que está sendo contestada
    /// </summary>
    [Required]
    public Guid ModerationId { get; set; }

    /// <summary>
    /// ID do funcionário que fez o appeal
    /// </summary>
    public Guid AppealByEmployeeId { get; set; }

    /// <summary>
    /// ID do funcionário que revisou o appeal
    /// </summary>
    public Guid? ReviewedByEmployeeId { get; set; }

    /// <summary>
    /// Status do appeal
    /// </summary>
    [Required]
    public AppealStatus Status { get; set; } = AppealStatus.Pending;

    /// <summary>
    /// Tipo do appeal
    /// </summary>
    [Required]
    public AppealType Type { get; set; }

    /// <summary>
    /// Razão do appeal fornecida pelo usuário
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Evidências ou informações adicionais
    /// </summary>
    [MaxLength(5000)]
    public string? Evidence { get; set; }

    /// <summary>
    /// Resposta do moderador/administrador
    /// </summary>
    [MaxLength(2000)]
    public string? ReviewerResponse { get; set; }

    /// <summary>
    /// Data/hora da revisão
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Decisão do appeal
    /// </summary>
    public AppealDecision? Decision { get; set; }

    /// <summary>
    /// Ação tomada como resultado do appeal
    /// </summary>
    public AppealResultAction? ResultAction { get; set; }

    /// <summary>
    /// Prioridade do appeal
    /// </summary>
    [Required]
    public AppealPriority Priority { get; set; } = AppealPriority.Normal;

    /// <summary>
    /// Dados adicionais em JSON
    /// </summary>
    public string? AdditionalData { get; set; }

    /// <summary>
    /// IP do usuário no momento do appeal
    /// </summary>
    [MaxLength(45)]
    public string? UserIpAddress { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data limite para resposta
    /// </summary>
    public DateTime? ResponseDeadline { get; set; }

    /// <summary>
    /// Se o appeal é anônimo (não revela identidade para outros moderadores)
    /// </summary>
    public bool IsAnonymous { get; set; }

    // Navegação
    /// <summary>
    /// Moderação sendo contestada
    /// </summary>
    public ModerationEntity? Moderation { get; set; }

    /// <summary>
    /// Funcionário que fez o appeal
    /// </summary>
    public Employee? AppealByEmployee { get; set; }

    /// <summary>
    /// Funcionário que revisou o appeal
    /// </summary>
    public Employee? ReviewedByEmployee { get; set; }
}

/// <summary>
/// Status do appeal
/// </summary>
public enum AppealStatus
{
    Pending,           // Aguardando revisão
    UnderReview,       // Sendo revisado
    Approved,          // Appeal aprovado
    Denied,            // Appeal negado
    PartiallyApproved, // Parcialmente aprovado
    Withdrawn,         // Retirado pelo usuário
    Expired,           // Expirado sem resposta
    Escalated          // Escalado para instância superior
}

/// <summary>
/// Tipo do appeal
/// </summary>
public enum AppealType
{
    WrongDecision,     // Decisão incorreta
    NewEvidence,       // Novas evidências
    ProcessError,      // Erro no processo
    TechnicalIssue,    // Problema técnico
    PolicyDispute,     // Disputa de política
    SeverityDispute,   // Disputa de severidade
    Other              // Outros
}

/// <summary>
/// Decisão do appeal
/// </summary>
public enum AppealDecision
{
    Upheld,            // Decisão original mantida
    Overturned,        // Decisão original revertida
    Modified,          // Decisão modificada
    RequiresEscalation // Requer escalação
}

/// <summary>
/// Ação resultante do appeal
/// </summary>
public enum AppealResultAction
{
    NoAction,          // Nenhuma ação
    RestoreContent,    // Restaurar conteúdo
    ReducePenalty,     // Reduzir penalidade
    RemovePenalty,     // Remover penalidade
    ModifyPenalty,     // Modificar penalidade
    ApologyRequired,   // Desculpa necessária
    PolicyUpdate,      // Atualização de política
    TrainingRequired   // Treinamento necessário
}

/// <summary>
/// Prioridade do appeal
/// </summary>
public enum AppealPriority
{
    None = 0,          // Valor sentinela - não usado
    Low = 1,           // Baixa prioridade
    Normal = 2,        // Prioridade normal
    High = 3,          // Alta prioridade
    Urgent = 4         // Urgente
}
