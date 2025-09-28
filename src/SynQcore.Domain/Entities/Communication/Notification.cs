namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma notificação corporativa enviada a um funcionário.
/// Suporta diferentes tipos, prioridades e referências contextuais.
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// Título da notificação.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem principal da notificação.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// URL de ação opcional para navegar quando clicada.
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Tipo da notificação corporativa.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Prioridade da notificação para ordenação e destaque.
    /// </summary>
    public NotificationPriority Priority { get; set; } =
        NotificationPriority.Normal;

    /// <summary>
    /// ID do funcionário destinatário da notificação.
    /// </summary>
    public Guid RecipientId { get; set; }

    /// <summary>
    /// Funcionário destinatário da notificação.
    /// </summary>
    public Employee Recipient { get; set; } = null!;

    /// <summary>
    /// ID do funcionário remetente (opcional para notificações do sistema).
    /// </summary>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// Funcionário remetente da notificação.
    /// </summary>
    public Employee? Sender { get; set; }

    /// <summary>
    /// ID do post relacionado à notificação (se aplicável).
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// Post relacionado à notificação.
    /// </summary>
    public Post? Post { get; set; }

    /// <summary>
    /// ID do comentário relacionado à notificação (se aplicável).
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// Comentário relacionado à notificação.
    /// </summary>
    public Comment? Comment { get; set; }

    /// <summary>
    /// ID de entidade genérica relacionada (para casos especiais).
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Tipo da entidade genérica relacionada.
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Indica se a notificação foi lida pelo destinatário.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Data e hora quando a notificação foi lida.
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Indica se foi enviado email para esta notificação.
    /// </summary>
    public bool IsEmailSent { get; set; }
}

/// <summary>
/// Tipos de notificações corporativas disponíveis no sistema.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Alguém curtiu seu post.
    /// </summary>
    PostLike = 0,

    /// <summary>
    /// Alguém comentou no seu post.
    /// </summary>
    PostComment = 1,

    /// <summary>
    /// Alguém respondeu ao seu comentário.
    /// </summary>
    CommentReply = 2,

    /// <summary>
    /// Alguém curtiu seu comentário.
    /// </summary>
    CommentLike = 3,

    /// <summary>
    /// Você foi mencionado em um post ou comentário.
    /// </summary>
    Mention = 4,

    /// <summary>
    /// Alguém começou a seguir você.
    /// </summary>
    Follow = 5,

    /// <summary>
    /// Notícias ou comunicados oficiais da empresa.
    /// </summary>
    CompanyNews = 6,

    /// <summary>
    /// Alertas do sistema ou manutenção.
    /// </summary>
    SystemAlert = 7,

    /// <summary>
    /// Aniversário de um colega de trabalho.
    /// </summary>
    Birthday = 8,

    /// <summary>
    /// Aniversário de trabalho de um colega.
    /// </summary>
    WorkAnniversary = 9,

    /// <summary>
    /// Novo funcionário ingressou na empresa.
    /// </summary>
    NewEmployee = 10,

    /// <summary>
    /// Ação de moderação foi realizada em seu conteúdo.
    /// </summary>
    ModerationAction = 11
}

/// <summary>
/// Prioridades de notificação para ordenação e destaque visual.
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Prioridade baixa - informações não urgentes.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Prioridade normal - padrão para a maioria das notificações.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Prioridade alta - informações importantes que merecem atenção.
    /// </summary>
    High = 2,

    /// <summary>
    /// Prioridade crítica - informações urgentes que requerem ação imediata.
    /// </summary>
    Critical = 3
}
