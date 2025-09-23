namespace SynQcore.Domain.Entities.Communication;

public class Notification : BaseEntity
{
    // Conteúdo
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }

    // Tipo de notificação
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } =
        NotificationPriority.Normal;

    // Relacionamentos
    public Guid RecipientId { get; set; }
    public Employee Recipient { get; set; } = null!;
    public Guid? SenderId { get; set; }
    public Employee? Sender { get; set; }

    // Referências opcionais
    public Guid? PostId { get; set; }
    public Post? Post { get; set; }
    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }

    // Status
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsEmailSent { get; set; }
}

public enum NotificationType
{
    PostLike = 0,           // Alguém curtiu seu post
    PostComment = 1,        // Alguém comentou no seu post
    CommentReply = 2,       // Alguém respondeu seu comentário
    CommentLike = 3,        // Alguém curtiu seu comentário
    Mention = 4,            // Você foi mencionado
    Follow = 5,             // Alguém começou a te seguir
    CompanyNews = 6,        // Notícias da empresa
    SystemAlert = 7,        // Alertas do sistema
    Birthday = 8,           // Aniversário de colega
    WorkAnniversary = 9,    // Aniversário de trabalho
    NewEmployee = 10        // Novo funcionário
}

public enum NotificationPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}