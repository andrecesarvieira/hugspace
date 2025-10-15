namespace SynQcore.BlazorApp.Models;

/// <summary>
/// Modelo para notificações corporativas no frontend
/// </summary>
public class NotificationModel
{
    /// <summary>
    /// ID único da notificação
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Título da notificação
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem da notificação
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da notificação
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Prioridade da notificação
    /// </summary>
    public NotificationPriority Priority { get; set; }

    /// <summary>
    /// Se a notificação foi lida
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Timestamp da notificação
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Classe CSS do ícone
    /// </summary>
    public string IconClass { get; set; } = "fas fa-bell";

    /// <summary>
    /// URL de ação (opcional)
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Categoria da notificação
    /// </summary>
    public string Category { get; set; } = "geral";

    /// <summary>
    /// Texto exibido há quanto tempo
    /// </summary>
    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTimeOffset.UtcNow - Timestamp;

            if (timeSpan.TotalMinutes < 1)
                return "agora";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}min";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d";

            return Timestamp.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Classe CSS baseada na prioridade
    /// </summary>
    public string PriorityClass
    {
        get
        {
            return Priority switch
            {
                NotificationPriority.Low => "notification-low",
                NotificationPriority.Normal => "notification-normal",
                NotificationPriority.High => "notification-high",
                NotificationPriority.Critical => "notification-critical",
                _ => "notification-normal"
            };
        }
    }

    /// <summary>
    /// Cor do badge baseada no tipo
    /// </summary>
    public string TypeColor
    {
        get
        {
            return Type switch
            {
                NotificationType.Personal => "#6366f1",
                NotificationType.Department => "#10b981",
                NotificationType.Corporate => "#f59e0b",
                NotificationType.Role => "#8b5cf6",
                NotificationType.Topic => "#06b6d4",
                _ => "#6b7280"
            };
        }
    }
}

/// <summary>
/// Tipos de notificação corporativa
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Notificação pessoal
    /// </summary>
    Personal,

    /// <summary>
    /// Notificação do departamento
    /// </summary>
    Department,

    /// <summary>
    /// Notificação corporativa
    /// </summary>
    Corporate,

    /// <summary>
    /// Notificação por função/cargo
    /// </summary>
    Role,

    /// <summary>
    /// Notificação por tópico
    /// </summary>
    Topic
}

/// <summary>
/// Prioridades de notificação
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Prioridade baixa
    /// </summary>
    Low,

    /// <summary>
    /// Prioridade normal
    /// </summary>
    Normal,

    /// <summary>
    /// Prioridade alta
    /// </summary>
    High,

    /// <summary>
    /// Prioridade crítica
    /// </summary>
    Critical
}

/// <summary>
/// Preferências de notificação do usuário
/// </summary>
public class NotificationPreferences
{
    /// <summary>
    /// Receber notificações pessoais
    /// </summary>
    public bool PersonalNotifications { get; set; } = true;

    /// <summary>
    /// Receber notificações do departamento
    /// </summary>
    public bool DepartmentNotifications { get; set; } = true;

    /// <summary>
    /// Receber notificações corporativas
    /// </summary>
    public bool CorporateNotifications { get; set; } = true;

    /// <summary>
    /// Receber notificações de função
    /// </summary>
    public bool RoleNotifications { get; set; } = true;

    /// <summary>
    /// Receber notificações por email
    /// </summary>
    public bool EmailNotifications { get; set; } = true;

    /// <summary>
    /// Receber push notifications
    /// </summary>
    public bool PushNotifications { get; set; } = true;

    /// <summary>
    /// Horário de não perturbe - início
    /// </summary>
    public TimeSpan? DoNotDisturbStart { get; set; }

    /// <summary>
    /// Horário de não perturbe - fim
    /// </summary>
    public TimeSpan? DoNotDisturbEnd { get; set; }

    /// <summary>
    /// Sons de notificação habilitados
    /// </summary>
    public bool SoundEnabled { get; set; } = true;

    /// <summary>
    /// Vibração habilitada (mobile)
    /// </summary>
    public bool VibrationEnabled { get; set; } = true;

    /// <summary>
    /// Tópicos subscritos
    /// </summary>
    public List<string> SubscribedTopics { get; set; } = new();

    /// <summary>
    /// Categorias silenciadas
    /// </summary>
    public List<string> MutedCategories { get; set; } = new();
}
