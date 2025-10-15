using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SynQcore.Api.Hubs;

/// <summary>
/// Hub SignalR para notificações corporativas em tempo real
/// Gerencia push notifications, alertas e comunicações instantâneas
/// </summary>
[Authorize]
public partial class CorporateNotificationHub : Hub
{
    private readonly ILogger<CorporateNotificationHub> _logger;

    public CorporateNotificationHub(ILogger<CorporateNotificationHub> logger)
    {
        _logger = logger;
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Usuário conectado ao NotificationHub - Email: {email}, Role: {role}, ConnectionId: {connectionId}")]
    private static partial void LogUserConnectedToNotifications(ILogger logger, string? email, string? role, string connectionId);

    [LoggerMessage(LogLevel.Information, "Usuário desconectado do NotificationHub - Email: {email}, ConnectionId: {connectionId}")]
    private static partial void LogUserDisconnectedFromNotifications(ILogger logger, string? email, string connectionId);

    [LoggerMessage(LogLevel.Information, "Usuário {email} entrou no grupo {groupName} para notificações")]
    private static partial void LogUserJoinedNotificationGroup(ILogger logger, string? email, string groupName);

    [LoggerMessage(LogLevel.Information, "Notificação enviada para usuário {userId} - Tipo: {type}")]
    private static partial void LogNotificationSent(ILogger logger, string userId, string type);

    [LoggerMessage(LogLevel.Information, "Notificação broadcast enviada para departamento {department} - Tipo: {type}")]
    private static partial void LogDepartmentNotificationSent(ILogger logger, string department, string type);

    [LoggerMessage(LogLevel.Warning, "Falha ao enviar notificação para usuário {userId} - Erro: {error}")]
    private static partial void LogNotificationFailed(ILogger logger, string userId, string error, Exception? exception);

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        var department = Context.User?.FindFirst("department")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // Adicionar usuário ao grupo pessoal para notificações diretas
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

            // Adicionar ao grupo de função corporativa
            if (!string.IsNullOrEmpty(userRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{userRole}");
                LogUserJoinedNotificationGroup(_logger, userEmail, $"Role_{userRole}");
            }

            // Adicionar ao grupo do departamento
            if (!string.IsNullOrEmpty(department))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Department_{department}");
                LogUserJoinedNotificationGroup(_logger, userEmail, $"Department_{department}");
            }

            // Adicionar ao grupo geral de notificações corporativas
            await Groups.AddToGroupAsync(Context.ConnectionId, "CorporateNotifications");

            LogUserConnectedToNotifications(_logger, userEmail, userRole, Context.ConnectionId);

            // Notificar cliente sobre conexão bem-sucedida
            await Clients.Caller.SendAsync("NotificationHubConnected", new
            {
                UserId = userId,
                Timestamp = DateTimeOffset.UtcNow,
                Groups = new[] { $"User_{userId}", $"Role_{userRole}", $"Department_{department}", "CorporateNotifications" }
            });
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        LogUserDisconnectedFromNotifications(_logger, userEmail, Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Marca notificação como lida
    /// </summary>
    /// <param name="notificationId">ID da notificação</param>
    public async Task MarkNotificationAsRead(string notificationId)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        try
        {
            // Aqui você integraria com o serviço de notificações para marcar como lida
            await Clients.Caller.SendAsync("NotificationMarkedAsRead", new
            {
                NotificationId = notificationId,
                UserId = userId,
                Timestamp = DateTimeOffset.UtcNow
            });

            LogNotificationSent(_logger, userId, "MarkAsRead");
        }
        catch (Exception ex)
        {
            LogNotificationFailed(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Subscreve usuário a notificações de um tópico específico
    /// </summary>
    /// <param name="topic">Tópico para subscrição (ex: "project_123", "announcement_hr")</param>
    public async Task SubscribeToTopic(string topic)
    {
        var userId = GetUserId();
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(topic)) return;

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Topic_{topic}");
            LogUserJoinedNotificationGroup(_logger, userEmail, $"Topic_{topic}");

            await Clients.Caller.SendAsync("SubscribedToTopic", new
            {
                Topic = topic,
                UserId = userId,
                Timestamp = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            LogNotificationFailed(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Remove subscrição de um tópico
    /// </summary>
    /// <param name="topic">Tópico para remoção</param>
    public async Task UnsubscribeFromTopic(string topic)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(topic)) return;

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Topic_{topic}");

            await Clients.Caller.SendAsync("UnsubscribedFromTopic", new
            {
                Topic = topic,
                UserId = userId,
                Timestamp = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            LogNotificationFailed(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Atualiza preferências de notificação do usuário
    /// </summary>
    /// <param name="preferences">Preferências de notificação</param>
    public async Task UpdateNotificationPreferences(object preferences)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        try
        {
            // Aqui você integraria com o serviço de preferências
            await Clients.Caller.SendAsync("NotificationPreferencesUpdated", new
            {
                UserId = userId,
                Preferences = preferences,
                Timestamp = DateTimeOffset.UtcNow
            });

            LogNotificationSent(_logger, userId, "PreferencesUpdate");
        }
        catch (Exception ex)
        {
            LogNotificationFailed(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Solicita contagem de notificações não lidas
    /// </summary>
    public async Task RequestUnreadCount()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        try
        {
            // Simulação - aqui você consultaria o banco de dados
            var unreadCount = 3; // Placeholder

            await Clients.Caller.SendAsync("UnreadNotificationCount", new
            {
                UserId = userId,
                Count = unreadCount,
                Timestamp = DateTimeOffset.UtcNow
            });

            LogNotificationSent(_logger, userId, "UnreadCount");
        }
        catch (Exception ex)
        {
            LogNotificationFailed(_logger, userId, ex.Message, ex);
        }
    }

    /// <summary>
    /// Obtém ID do usuário atual
    /// </summary>
    private string GetUserId()
    {
        return Context.User?.FindFirst("sub")?.Value ??
               Context.User?.FindFirst("id")?.Value ??
               Context.User?.FindFirst("userId")?.Value ??
               Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               string.Empty;
    }
}

/// <summary>
/// Métodos do servidor para envio de notificações (usados por outros serviços)
/// </summary>
public partial class CorporateNotificationHub
{
    /// <summary>
    /// Envia notificação push para usuário específico
    /// </summary>
    /// <param name="hubContext">Contexto do Hub</param>
    /// <param name="userId">ID do usuário</param>
    /// <param name="notification">Dados da notificação</param>
    public static async Task SendPersonalNotification(IHubContext<CorporateNotificationHub> hubContext, string userId, object notification)
    {
        await hubContext.Clients.Group($"User_{userId}").SendAsync("PersonalNotification", notification);
    }

    /// <summary>
    /// Envia notificação para departamento
    /// </summary>
    /// <param name="hubContext">Contexto do Hub</param>
    /// <param name="department">Nome do departamento</param>
    /// <param name="notification">Dados da notificação</param>
    public static async Task SendDepartmentNotification(IHubContext<CorporateNotificationHub> hubContext, string department, object notification)
    {
        await hubContext.Clients.Group($"Department_{department}").SendAsync("DepartmentNotification", notification);
    }

    /// <summary>
    /// Envia notificação corporativa para todos os usuários
    /// </summary>
    /// <param name="hubContext">Contexto do Hub</param>
    /// <param name="notification">Dados da notificação</param>
    public static async Task SendCorporateNotification(IHubContext<CorporateNotificationHub> hubContext, object notification)
    {
        await hubContext.Clients.Group("CorporateNotifications").SendAsync("CorporateNotification", notification);
    }

    /// <summary>
    /// Envia notificação por função/cargo
    /// </summary>
    /// <param name="hubContext">Contexto do Hub</param>
    /// <param name="role">Função corporativa</param>
    /// <param name="notification">Dados da notificação</param>
    public static async Task SendRoleNotification(IHubContext<CorporateNotificationHub> hubContext, string role, object notification)
    {
        await hubContext.Clients.Group($"Role_{role}").SendAsync("RoleNotification", notification);
    }

    /// <summary>
    /// Envia notificação para tópico específico
    /// </summary>
    /// <param name="hubContext">Contexto do Hub</param>
    /// <param name="topic">Tópico da notificação</param>
    /// <param name="notification">Dados da notificação</param>
    public static async Task SendTopicNotification(IHubContext<CorporateNotificationHub> hubContext, string topic, object notification)
    {
        await hubContext.Clients.Group($"Topic_{topic}").SendAsync("TopicNotification", notification);
    }
}
