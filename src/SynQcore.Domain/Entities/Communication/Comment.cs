namespace SynQcore.Domain.Entities.Communication;

public class Comment : BaseEntity
{
    // Conteúdo
    public string Content { get; set; } = string.Empty;

    // Hierarquia de comentários (replies)
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];

    // Relacionamentos
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public Employee Author { get; set; } = null!;

    // Discussion Thread Features
    public CommentType Type { get; set; } = CommentType.Regular;
    public bool IsResolved { get; set; }
    public Guid? ResolvedById { get; set; }
    public Employee? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }

    // Moderação Corporativa
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsFlagged { get; set; }
    public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;
    public Guid? ModeratedById { get; set; }
    public Employee? ModeratedBy { get; set; }
    public DateTime? ModeratedAt { get; set; }
    public string? ModerationReason { get; set; }

    // Visibilidade e Privacidade
    public CommentVisibility Visibility { get; set; } = CommentVisibility.Public;
    public bool IsConfidential { get; set; }
    
    // Threading e Organização
    public int ThreadLevel { get; set; } // Nível de profundidade na thread
    public string ThreadPath { get; set; } = string.Empty; // Caminho hierárquico (ex: "1.2.3")
    public bool IsHighlighted { get; set; } // Destacado pelo autor ou moderadores
    public CommentPriority Priority { get; set; } = CommentPriority.Normal;

    // Métricas e Engagement
    public int LikeCount { get; set; }
    public int ReplyCount { get; set; }
    public int EndorsementCount { get; set; }
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    // Propriedades de Navegação
    public ICollection<CommentLike> Likes { get; set; } = [];
    public ICollection<Endorsement> Endorsements { get; set; } = [];
    public ICollection<CommentMention> Mentions { get; set; } = [];
    public ICollection<CommentMention> MentionsMade { get; set; } = [];
}

/// <summary>
/// Tipos de comentários em discussions corporativas
/// </summary>
public enum CommentType
{
    Regular = 0,        // Comentário regular
    Question = 1,       // Pergunta que precisa de resposta
    Answer = 2,         // Resposta para uma pergunta
    Suggestion = 3,     // Sugestão de melhoria
    Concern = 4,        // Preocupação ou issue
    Acknowledgment = 5, // Reconhecimento ou agradecimento
    Decision = 6,       // Decisão tomada (Manager/Admin)
    Action = 7          // Item de ação atribuído
}

/// <summary>
/// Status de moderação corporativa
/// </summary>
public enum ModerationStatus
{
    Pending = 0,        // Aguardando moderação
    Approved = 1,       // Aprovado
    Flagged = 2,        // Sinalizado para revisão
    Hidden = 3,         // Oculto (mas não deletado)
    Rejected = 4,       // Rejeitado
    UnderReview = 5     // Sob análise detalhada
}

/// <summary>
/// Visibilidade de comentários corporativos
/// </summary>
public enum CommentVisibility
{
    Public = 0,         // Visível para todos
    Internal = 1,       // Apenas funcionários internos
    Confidential = 2,   // Apenas níveis Manager+
    Private = 3         // Apenas author e moderadores
}

/// <summary>
/// Prioridade de comentários em discussions
/// </summary>
public enum CommentPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3,
    Critical = 4
}