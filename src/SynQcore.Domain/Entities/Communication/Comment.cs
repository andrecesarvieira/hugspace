namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa um comentário em posts e discussões corporativas.
/// Suporta hierarquia de respostas, moderação, visibilidade e métricas de engajamento.
/// </summary>
public class Comment : BaseEntity
{
    /// <summary>
    /// Conteúdo textual do comentário.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// ID do comentário pai, se este for uma resposta.
    /// </summary>
    public Guid? ParentCommentId { get; set; }

    /// <summary>
    /// Comentário pai na hierarquia de respostas.
    /// </summary>
    public Comment? ParentComment { get; set; }

    /// <summary>
    /// Coleção de respostas a este comentário.
    /// </summary>
    public ICollection<Comment> Replies { get; set; } = [];

    /// <summary>
    /// ID do post ao qual este comentário pertence.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Post ao qual este comentário pertence.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    /// ID do funcionário autor do comentário.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Funcionário autor do comentário.
    /// </summary>
    public Employee Author { get; set; } = null!;

    /// <summary>
    /// Tipo do comentário na discussão corporativa.
    /// </summary>
    public CommentType Type { get; set; } = CommentType.Regular;

    /// <summary>
    /// Indica se o comentário foi resolvido (para perguntas/issues).
    /// </summary>
    public bool IsResolved { get; set; }

    /// <summary>
    /// ID do funcionário que resolveu o comentário.
    /// </summary>
    public Guid? ResolvedById { get; set; }

    /// <summary>
    /// Funcionário que resolveu o comentário.
    /// </summary>
    public Employee? ResolvedBy { get; set; }

    /// <summary>
    /// Data e hora da resolução do comentário.
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Nota explicativa sobre a resolução.
    /// </summary>
    public string? ResolutionNote { get; set; }

    /// <summary>
    /// Indica se o comentário foi editado após criação.
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// Data e hora da última edição.
    /// </summary>
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// Indica se o comentário foi sinalizado para moderação.
    /// </summary>
    public bool IsFlagged { get; set; }

    /// <summary>
    /// Status atual de moderação corporativa.
    /// </summary>
    public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;

    /// <summary>
    /// ID do moderador responsável pela moderação.
    /// </summary>
    public Guid? ModeratedById { get; set; }

    /// <summary>
    /// Moderador responsável pela moderação.
    /// </summary>
    public Employee? ModeratedBy { get; set; }

    /// <summary>
    /// Data e hora da moderação.
    /// </summary>
    public DateTime? ModeratedAt { get; set; }

    /// <summary>
    /// Razão da ação de moderação.
    /// </summary>
    public string? ModerationReason { get; set; }

    /// <summary>
    /// Nível de visibilidade do comentário na organização.
    /// </summary>
    public CommentVisibility Visibility { get; set; } = CommentVisibility.Public;

    /// <summary>
    /// Indica se o comentário contém informações confidenciais.
    /// </summary>
    public bool IsConfidential { get; set; }

    /// <summary>
    /// Nível de profundidade na hierarquia de comentários.
    /// </summary>
    public int ThreadLevel { get; set; }

    /// <summary>
    /// Caminho hierárquico na thread (ex: "1.2.3").
    /// </summary>
    public string ThreadPath { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o comentário foi destacado por autor ou moderadores.
    /// </summary>
    public bool IsHighlighted { get; set; }

    /// <summary>
    /// Prioridade do comentário na discussão.
    /// </summary>
    public CommentPriority Priority { get; set; } = CommentPriority.Normal;

    /// <summary>
    /// Número total de curtidas recebidas.
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// Número total de respostas ao comentário.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Número total de endorsements recebidos.
    /// </summary>
    public int EndorsementCount { get; set; }

    /// <summary>
    /// Data e hora da última atividade relacionada ao comentário.
    /// </summary>
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Coleção de curtidas no comentário.
    /// </summary>
    public ICollection<CommentLike> Likes { get; set; } = [];

    /// <summary>
    /// Coleção de endorsements no comentário.
    /// </summary>
    public ICollection<Endorsement> Endorsements { get; set; } = [];

    /// <summary>
    /// Coleção de menções a funcionários no comentário.
    /// </summary>
    public ICollection<CommentMention> Mentions { get; set; } = [];
}

/// <summary>
/// Tipos de comentários em discussions corporativas
/// </summary>
public enum CommentType
{
    /// <summary>
    /// Comentário regular sem classificação especial.
    /// </summary>
    Regular = 0,

    /// <summary>
    /// Pergunta que precisa de resposta da equipe.
    /// </summary>
    Question = 1,

    /// <summary>
    /// Resposta para uma pergunta específica.
    /// </summary>
    Answer = 2,

    /// <summary>
    /// Sugestão de melhoria ou nova funcionalidade.
    /// </summary>
    Suggestion = 3,

    /// <summary>
    /// Preocupação ou problema identificado.
    /// </summary>
    Concern = 4,

    /// <summary>
    /// Reconhecimento ou agradecimento a colegas.
    /// </summary>
    Acknowledgment = 5,

    /// <summary>
    /// Decisão tomada por gestores ou administradores.
    /// </summary>
    Decision = 6,

    /// <summary>
    /// Item de ação atribuído a funcionário específico.
    /// </summary>
    Action = 7
}

/// <summary>
/// Status de moderação corporativa
/// </summary>
public enum ModerationStatus
{
    /// <summary>
    /// Aguardando moderação por administrador.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Aprovado e visível publicamente.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Sinalizado para revisão manual.
    /// </summary>
    Flagged = 2,

    /// <summary>
    /// Oculto mas não deletado permanentemente.
    /// </summary>
    Hidden = 3,

    /// <summary>
    /// Rejeitado e não será exibido.
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Sob análise detalhada por moderadores.
    /// </summary>
    UnderReview = 5
}

/// <summary>
/// Visibilidade de comentários corporativos
/// </summary>
public enum CommentVisibility
{
    /// <summary>
    /// Visível para todos os funcionários da organização.
    /// </summary>
    Public = 0,

    /// <summary>
    /// Visível apenas para funcionários internos.
    /// </summary>
    Internal = 1,

    /// <summary>
    /// Visível apenas para gestores e níveis superiores.
    /// </summary>
    Confidential = 2,

    /// <summary>
    /// Visível apenas para autor e moderadores.
    /// </summary>
    Private = 3
}

/// <summary>
/// Prioridade de comentários em discussions
/// </summary>
public enum CommentPriority
{
    /// <summary>
    /// Prioridade baixa, não requer atenção imediata.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Prioridade normal, processamento padrão.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Prioridade alta, requer atenção em breve.
    /// </summary>
    High = 2,

    /// <summary>
    /// Prioridade urgente, requer atenção rápida.
    /// </summary>
    Urgent = 3,

    /// <summary>
    /// Prioridade crítica, requer atenção imediata.
    /// </summary>
    Critical = 4
}
