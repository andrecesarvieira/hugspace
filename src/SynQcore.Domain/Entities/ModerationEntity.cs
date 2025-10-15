/*
 * SynQcore - Corporate Social Network
 *
 * Moderation Entity - Sistema de moderação corporativa
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.ComponentModel.DataAnnotations;
using SynQcore.Domain.Common;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Entidade de moderação para controle de conteúdo corporativo
/// </summary>
public class ModerationEntity : BaseEntity
{
    /// <summary>
    /// Tipo de conteúdo sendo moderado
    /// </summary>
    [Required]
    public ModerationContentType ContentType { get; set; }

    /// <summary>
    /// ID do conteúdo sendo moderado (Post, Comment, etc.)
    /// </summary>
    [Required]
    public Guid ContentId { get; set; }

    /// <summary>
    /// ID do usuário que criou o conteúdo
    /// </summary>
    [Required]
    public Guid ContentAuthorId { get; set; }

    /// <summary>
    /// ID do funcionário que reportou
    /// </summary>
    public Guid? ReportedByEmployeeId { get; set; }

    /// <summary>
    /// ID do moderador responsável
    /// </summary>
    public Guid? ModeratorId { get; set; }

    /// <summary>
    /// Status atual da moderação
    /// </summary>
    [Required]
    public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

    /// <summary>
    /// Categoria da violação reportada
    /// </summary>
    [Required]
    public ModerationCategory Category { get; set; }

    /// <summary>
    /// Severidade da violação
    /// </summary>
    [Required]
    public ModerationSeverity Severity { get; set; } = ModerationSeverity.Low;

    /// <summary>
    /// Ação tomada pelo sistema/moderador
    /// </summary>
    public ModerationAction? ActionTaken { get; set; }

    /// <summary>
    /// Razão do report/moderação
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Notas do moderador
    /// </summary>
    [MaxLength(2000)]
    public string? ModeratorNotes { get; set; }

    /// <summary>
    /// Detalhes adicionais em JSON
    /// </summary>
    public string? AdditionalDetails { get; set; }

    /// <summary>
    /// Se foi detectado automaticamente
    /// </summary>
    public bool IsAutomaticDetection { get; set; }

    /// <summary>
    /// Score de confiança da detecção automática (0-100)
    /// </summary>
    public int? AutoDetectionScore { get; set; }

    /// <summary>
    /// Data/hora da decisão de moderação
    /// </summary>
    public DateTime? ModerationDate { get; set; }

    /// <summary>
    /// Data/hora de expiração (para ações temporárias)
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// IP do usuário no momento da criação do conteúdo
    /// </summary>
    [MaxLength(45)]
    public string? UserIpAddress { get; set; }

    /// <summary>
    /// User Agent do usuário
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    // Navegação
    /// <summary>
    /// Funcionário que criou o conteúdo
    /// </summary>
    public Employee? ContentAuthor { get; set; }

    /// <summary>
    /// Funcionário que reportou
    /// </summary>
    public Employee? ReportedByEmployee { get; set; }

    /// <summary>
    /// Moderador responsável
    /// </summary>
    public Employee? Moderator { get; set; }

    /// <summary>
    /// Logs de ações de moderação
    /// </summary>
    public ICollection<ModerationLogEntity> ModerationLogs { get; set; } = new List<ModerationLogEntity>();

    /// <summary>
    /// Appeals relacionados
    /// </summary>
    public ICollection<ModerationAppealEntity> Appeals { get; set; } = new List<ModerationAppealEntity>();
}

/// <summary>
/// Tipos de conteúdo que podem ser moderados
/// </summary>
public enum ModerationContentType
{
    Post,
    Comment,
    Message,
    Profile,
    Image,
    Document,
    Video,
    Audio,
    Other
}

/// <summary>
/// Status da moderação
/// </summary>
public enum ModerationStatus
{
    Pending,           // Aguardando análise
    UnderReview,       // Sendo analisado
    Approved,          // Aprovado (sem violação)
    Rejected,          // Rejeitado (violação confirmada)
    Escalated,         // Escalado para supervisor
    OnHold,            // Em espera (aguardando informações)
    AutoApproved,      // Aprovado automaticamente
    AutoRejected,      // Rejeitado automaticamente
    Expired            // Expirado (sem análise)
}

/// <summary>
/// Categorias de moderação corporativa
/// </summary>
public enum ModerationCategory
{
    Spam,              // Spam/flood
    Harassment,        // Assédio/bullying
    InappropriateContent, // Conteúdo inapropriado
    Misinformation,    // Desinformação
    OffensiveLanguage, // Linguagem ofensiva
    Copyright,         // Violação de direitos autorais
    Privacy,           // Violação de privacidade
    SecurityThreat,    // Ameaça de segurança
    PolicyViolation,   // Violação de política corporativa
    Other              // Outros
}

/// <summary>
/// Severidade da violação
/// </summary>
public enum ModerationSeverity
{
    Low,               // Baixa - aviso
    Medium,            // Média - suspensão temporária
    High,              // Alta - suspensão longa
    Critical           // Crítica - banimento permanente
}

/// <summary>
/// Ações de moderação possíveis
/// </summary>
public enum ModerationAction
{
    NoAction,          // Nenhuma ação
    Warning,           // Aviso ao usuário
    ContentRemoval,    // Remoção do conteúdo
    ContentEdit,       // Edição do conteúdo
    UserSuspension,    // Suspensão temporária
    UserBan,           // Banimento permanente
    Shadowban,         // Shadowban (invisível para outros)
    AccountRestriction, // Restrição de funcionalidades
    EscalateToAdmin,   // Escalar para administração
    RequireModeration  // Futuras postagens precisam moderação
}
