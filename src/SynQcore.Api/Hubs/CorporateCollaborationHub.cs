using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SynQcore.Api.Hubs;

/// <summary>
/// Hub principal para colaboração corporativa em tempo real
/// Gerencia comunicação entre funcionários, canais de equipe e notificações
/// </summary>
[Authorize]
public partial class CorporateCollaborationHub : Hub
{
    private readonly ILogger<CorporateCollaborationHub> _logger;

    public CorporateCollaborationHub(ILogger<CorporateCollaborationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Evento de conexão de funcionário - registra presença corporativa
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // Adicionar usuário ao grupo de sua função corporativa
            if (!string.IsNullOrEmpty(userRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{userRole}");
                LogUserConnected(_logger, userEmail ?? "Unknown", userRole, Context.ConnectionId);
            }

            // Notificar outros usuários sobre presença online
            await Clients.Others.SendAsync("UserOnline", new
            {
                UserId = userId,
                Email = userEmail,
                Role = userRole,
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Evento de desconexão - atualiza status de presença
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            LogUserDisconnected(_logger, userEmail ?? "Unknown", Context.ConnectionId);

            // Notificar outros usuários sobre saída
            await Clients.Others.SendAsync("UserOffline", new
            {
                UserId = userId,
                Email = userEmail,
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Entrar em canal de equipe específico
    /// </summary>
    public async Task JoinTeamChannel(string teamId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(teamId))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"Team_{teamId}");
        LogJoinedTeamChannel(_logger, userId, teamId);

        // Notificar membros da equipe sobre entrada
        await Clients.Group($"Team_{teamId}").SendAsync("UserJoinedTeam", new
        {
            UserId = userId,
            TeamId = teamId,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Sair de canal de equipe específico
    /// </summary>
    public async Task LeaveTeamChannel(string teamId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(teamId))
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Team_{teamId}");
        LogLeftTeamChannel(_logger, userId, teamId);

        // Notificar membros da equipe sobre saída
        await Clients.Group($"Team_{teamId}").SendAsync("UserLeftTeam", new
        {
            UserId = userId,
            TeamId = teamId,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Entrar em canal de projeto específico
    /// </summary>
    public async Task JoinProjectChannel(string projectId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"Project_{projectId}");
        LogJoinedProjectChannel(_logger, userId, projectId);

        // Notificar participantes do projeto sobre entrada
        await Clients.Group($"Project_{projectId}").SendAsync("UserJoinedProject", new
        {
            UserId = userId,
            ProjectId = projectId,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Sair de canal de projeto específico
    /// </summary>
    public async Task LeaveProjectChannel(string projectId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Project_{projectId}");
        LogLeftProjectChannel(_logger, userId, projectId);

        // Notificar participantes do projeto sobre saída
        await Clients.Group($"Project_{projectId}").SendAsync("UserLeftProject", new
        {
            UserId = userId,
            ProjectId = projectId,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Enviar mensagem para canal de equipe
    /// </summary>
    public async Task SendTeamMessage(string teamId, string message)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(teamId) || string.IsNullOrEmpty(message))
        {
            return;
        }

        LogTeamMessageSent(_logger, userId, teamId, message.Length);

        // Enviar mensagem para todos os membros da equipe
        await Clients.Group($"Team_{teamId}").SendAsync("ReceiveTeamMessage", new
        {
            UserId = userId,
            UserEmail = userEmail,
            TeamId = teamId,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Enviar mensagem para canal de projeto
    /// </summary>
    public async Task SendProjectMessage(string projectId, string message)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(message))
        {
            return;
        }

        LogProjectMessageSent(_logger, userId, projectId, message.Length);

        // Enviar mensagem para todos os participantes do projeto
        await Clients.Group($"Project_{projectId}").SendAsync("ReceiveProjectMessage", new
        {
            UserId = userId,
            UserEmail = userEmail,
            ProjectId = projectId,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Atualizar status de presença do usuário
    /// </summary>
    public async Task UpdatePresenceStatus(string status)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(status))
        {
            return;
        }

        LogPresenceStatusUpdated(_logger, userId, status);

        // Notificar todos os usuários conectados sobre mudança de status
        await Clients.Others.SendAsync("UserPresenceChanged", new
        {
            UserId = userId,
            Status = status,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 4001, Level = LogLevel.Information,
        Message = "Usuário conectado: {Email} ({Role}) | ConnectionId: {ConnectionId}")]
    private static partial void LogUserConnected(ILogger logger, string email, string role, string connectionId);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information,
        Message = "Usuário desconectado: {Email} | ConnectionId: {ConnectionId}")]
    private static partial void LogUserDisconnected(ILogger logger, string email, string connectionId);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Information,
        Message = "Usuário {UserId} entrou no canal da equipe {TeamId}")]
    private static partial void LogJoinedTeamChannel(ILogger logger, string userId, string teamId);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Information,
        Message = "Usuário {UserId} saiu do canal da equipe {TeamId}")]
    private static partial void LogLeftTeamChannel(ILogger logger, string userId, string teamId);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Information,
        Message = "Usuário {UserId} entrou no canal do projeto {ProjectId}")]
    private static partial void LogJoinedProjectChannel(ILogger logger, string userId, string projectId);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Information,
        Message = "Usuário {UserId} saiu do canal do projeto {ProjectId}")]
    private static partial void LogLeftProjectChannel(ILogger logger, string userId, string projectId);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Information,
        Message = "Mensagem de equipe enviada: UserId {UserId} -> TeamId {TeamId} ({MessageLength} chars)")]
    private static partial void LogTeamMessageSent(ILogger logger, string userId, string teamId, int messageLength);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Information,
        Message = "Mensagem de projeto enviada: UserId {UserId} -> ProjectId {ProjectId} ({MessageLength} chars)")]
    private static partial void LogProjectMessageSent(ILogger logger, string userId, string projectId, int messageLength);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Information,
        Message = "Status de presença atualizado: UserId {UserId} -> {Status}")]
    private static partial void LogPresenceStatusUpdated(ILogger logger, string userId, string status);

    #endregion
}