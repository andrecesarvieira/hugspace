namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma menção (@username) em comentários corporativos
/// </summary>
public class CommentMention : BaseEntity
{
    // Relacionamentos
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
    
    public Guid MentionedEmployeeId { get; set; }
    public Employee MentionedEmployee { get; set; } = null!;
    
    public Guid MentionedById { get; set; }
    public Employee MentionedBy { get; set; } = null!;

    // Detalhes da menção
    public string MentionText { get; set; } = string.Empty; // Texto original (@nome.sobrenome)
    public int StartPosition { get; set; } // Posição no texto do comentário
    public int Length { get; set; } // Tamanho da menção

    // Status e notificação
    public bool HasBeenNotified { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // Contexto corporativo
    public MentionContext Context { get; set; } = MentionContext.General;
    public MentionUrgency Urgency { get; set; } = MentionUrgency.Normal;
}

/// <summary>
/// Contexto da menção corporativa
/// </summary>
public enum MentionContext
{
    General = 0,        // Menção geral
    Question = 1,       // Pergunta direcionada
    Action = 2,         // Solicitação de ação
    FYI = 3,           // Para conhecimento (For Your Information)
    Decision = 4,       // Necessita decisão
    Approval = 5,       // Solicitação de aprovação
    Review = 6,         // Solicitação de revisão
    Escalation = 7      // Escalação de issue
}

/// <summary>
/// Urgência da menção
/// </summary>
public enum MentionUrgency
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}