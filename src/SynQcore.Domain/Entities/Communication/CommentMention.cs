namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma menção (@username) em comentários corporativos
/// </summary>
public class CommentMention : BaseEntity
{
    /// <summary>
    /// ID do comentário onde a menção foi feita.
    /// </summary>
    public Guid CommentId { get; set; }

    /// <summary>
    /// Comentário onde a menção foi feita.
    /// </summary>
    public Comment Comment { get; set; } = null!;

    /// <summary>
    /// ID do funcionário que foi mencionado.
    /// </summary>
    public Guid MentionedEmployeeId { get; set; }

    /// <summary>
    /// Funcionário que foi mencionado.
    /// </summary>
    public Employee MentionedEmployee { get; set; } = null!;

    /// <summary>
    /// ID do funcionário que fez a menção.
    /// </summary>
    public Guid MentionedById { get; set; }

    /// <summary>
    /// Funcionário que fez a menção.
    /// </summary>
    public Employee MentionedBy { get; set; } = null!;

    /// <summary>
    /// Texto original da menção (ex: @nome.sobrenome).
    /// </summary>
    public string MentionText { get; set; } = string.Empty;

    /// <summary>
    /// Posição inicial da menção no texto do comentário.
    /// </summary>
    public int StartPosition { get; set; }

    /// <summary>
    /// Tamanho em caracteres da menção.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Indica se o funcionário mencionado foi notificado.
    /// </summary>
    public bool HasBeenNotified { get; set; }

    /// <summary>
    /// Data e hora quando a notificação foi enviada.
    /// </summary>
    public DateTime? NotifiedAt { get; set; }

    /// <summary>
    /// Indica se a menção foi lida pelo funcionário mencionado.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Data e hora quando a menção foi lida.
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Contexto corporativo da menção.
    /// </summary>
    public MentionContext Context { get; set; } = MentionContext.General;

    /// <summary>
    /// Nível de urgência da menção.
    /// </summary>
    public MentionUrgency Urgency { get; set; } = MentionUrgency.Normal;
}

/// <summary>
/// Contexto da menção corporativa
/// </summary>
public enum MentionContext
{
    /// <summary>
    /// Menção geral sem contexto específico.
    /// </summary>
    General = 0,

    /// <summary>
    /// Pergunta direcionada ao funcionário mencionado.
    /// </summary>
    Question = 1,

    /// <summary>
    /// Solicitação de ação específica.
    /// </summary>
    Action = 2,

    /// <summary>
    /// Para conhecimento (For Your Information).
    /// </summary>
    FYI = 3,

    /// <summary>
    /// Necessita decisão do funcionário mencionado.
    /// </summary>
    Decision = 4,

    /// <summary>
    /// Solicitação de aprovação.
    /// </summary>
    Approval = 5,

    /// <summary>
    /// Solicitação de revisão ou feedback.
    /// </summary>
    Review = 6,

    /// <summary>
    /// Escalação de problema ou issue.
    /// </summary>
    Escalation = 7
}

/// <summary>
/// Urgência da menção
/// </summary>
public enum MentionUrgency
{
    /// <summary>
    /// Baixa urgência, não requer resposta imediata.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Urgência normal, resposta em prazo regular.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Alta urgência, requer atenção prioritária.
    /// </summary>
    High = 2,

    /// <summary>
    /// Urgência crítica, requer resposta imediata.
    /// </summary>
    Urgent = 3
}
