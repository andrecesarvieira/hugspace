using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SynQcore.Api.Hubs;

[Authorize]
public partial class ExecutiveCommunicationHub : Hub
{
    private readonly ILogger<ExecutiveCommunicationHub> _logger;

    public ExecutiveCommunicationHub(ILogger<ExecutiveCommunicationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // Adicionar todos os funcionários ao canal de anúncios gerais
            await Groups.AddToGroupAsync(Context.ConnectionId, "CompanyAnnouncements");

            // Adicionar executivos aos canais de broadcast
            if (userRole is "Manager" or "HR" or "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "ExecutiveBroadcast");
                LogExecutiveConnected(_logger, userEmail ?? "Unknown", userRole, Context.ConnectionId);
            }
            else
            {
                LogEmployeeConnected(_logger, userEmail ?? "Unknown", Context.ConnectionId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        LogUserDisconnectedFromExecutive(_logger, userEmail ?? "Unknown", Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task SendCompanyAnnouncement(string title, string message, string priority = "Normal")
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(message))
        {
            return;
        }

        LogCompanyAnnouncementSent(_logger, userId, userRole ?? "Unknown", title, priority);

        // Broadcast para todos os funcionários da empresa
        await Clients.Group("CompanyAnnouncements").SendAsync("ReceiveCompanyAnnouncement", new
        {
            AnnouncementId = Guid.NewGuid(),
            Title = title,
            Message = message,
            Priority = priority,
            SentBy = new
            {
                UserId = userId,
                Email = userEmail,
                Role = userRole
            },
            Timestamp = DateTimeOffset.UtcNow,
            Type = "CompanyAnnouncement"
        });
    }

    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task SendExecutiveCommunication(string title, string message, string confidentialityLevel = "Internal")
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(message))
        {
            return;
        }

        LogExecutiveCommunicationSent(_logger, userId, userRole ?? "Unknown", title, confidentialityLevel);

        // Enviar apenas para executivos
        await Clients.Group("ExecutiveBroadcast").SendAsync("ReceiveExecutiveCommunication", new
        {
            CommunicationId = Guid.NewGuid(),
            Title = title,
            Message = message,
            ConfidentialityLevel = confidentialityLevel,
            SentBy = new
            {
                UserId = userId,
                Email = userEmail,
                Role = userRole
            },
            Timestamp = DateTimeOffset.UtcNow,
            Type = "ExecutiveCommunication"
        });
    }

    [Authorize(Roles = "HR,Admin")]
    public async Task SendPolicyUpdate(string policyTitle, string changeDescription, string effectiveDate, bool requiresAcknowledgment = false)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(policyTitle) || string.IsNullOrEmpty(changeDescription))
        {
            return;
        }

        LogPolicyUpdateSent(_logger, userId, userRole ?? "Unknown", policyTitle, requiresAcknowledgment);

        // Broadcast para todos os funcionários
        await Clients.Group("CompanyAnnouncements").SendAsync("ReceivePolicyUpdate", new
        {
            PolicyUpdateId = Guid.NewGuid(),
            PolicyTitle = policyTitle,
            ChangeDescription = changeDescription,
            EffectiveDate = effectiveDate,
            RequiresAcknowledgment = requiresAcknowledgment,
            SentBy = new
            {
                UserId = userId,
                Email = userEmail,
                Role = userRole
            },
            Timestamp = DateTimeOffset.UtcNow,
            Type = "PolicyUpdate"
        });
    }

    public async Task JoinDepartmentCommunications(string departmentId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(departmentId))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"Department_{departmentId}");
        LogJoinedDepartmentCommunications(_logger, userId, departmentId);
    }

    public async Task LeaveDepartmentCommunications(string departmentId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(departmentId))
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Department_{departmentId}");
        LogLeftDepartmentCommunications(_logger, userId, departmentId);
    }

    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task SendDepartmentCommunication(string departmentId, string title, string message)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(departmentId) || 
            string.IsNullOrEmpty(title) || string.IsNullOrEmpty(message))
        {
            return;
        }

        LogDepartmentCommunicationSent(_logger, userId, userRole ?? "Unknown", departmentId, title);

        // Enviar para funcionários do departamento específico
        await Clients.Group($"Department_{departmentId}").SendAsync("ReceiveDepartmentCommunication", new
        {
            CommunicationId = Guid.NewGuid(),
            DepartmentId = departmentId,
            Title = title,
            Message = message,
            SentBy = new
            {
                UserId = userId,
                Email = userEmail,
                Role = userRole
            },
            Timestamp = DateTimeOffset.UtcNow,
            Type = "DepartmentCommunication"
        });
    }

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 4101, Level = LogLevel.Information,
        Message = "Executivo conectado ao hub: {Email} ({Role}) | ConnectionId: {ConnectionId}")]
    private static partial void LogExecutiveConnected(ILogger logger, string email, string role, string connectionId);

    [LoggerMessage(EventId = 4102, Level = LogLevel.Information,
        Message = "Funcionário conectado ao hub executivo: {Email} | ConnectionId: {ConnectionId}")]
    private static partial void LogEmployeeConnected(ILogger logger, string email, string connectionId);

    [LoggerMessage(EventId = 4103, Level = LogLevel.Information,
        Message = "Usuário desconectado do hub executivo: {Email} | ConnectionId: {ConnectionId}")]
    private static partial void LogUserDisconnectedFromExecutive(ILogger logger, string email, string connectionId);

    [LoggerMessage(EventId = 4104, Level = LogLevel.Information,
        Message = "Anúncio corporativo enviado: {UserId} ({Role}) - Título: {Title} - Prioridade: {Priority}")]
    private static partial void LogCompanyAnnouncementSent(ILogger logger, string userId, string role, string title, string priority);

    [LoggerMessage(EventId = 4105, Level = LogLevel.Information,
        Message = "Comunicação executiva enviada: {UserId} ({Role}) - Título: {Title} - Nível: {ConfidentialityLevel}")]
    private static partial void LogExecutiveCommunicationSent(ILogger logger, string userId, string role, string title, string confidentialityLevel);

    [LoggerMessage(EventId = 4106, Level = LogLevel.Information,
        Message = "Atualização de política enviada: {UserId} ({Role}) - Política: {PolicyTitle} - Requer confirmação: {RequiresAcknowledgment}")]
    private static partial void LogPolicyUpdateSent(ILogger logger, string userId, string role, string policyTitle, bool requiresAcknowledgment);

    [LoggerMessage(EventId = 4107, Level = LogLevel.Information,
        Message = "Usuário {UserId} entrou nas comunicações do departamento {DepartmentId}")]
    private static partial void LogJoinedDepartmentCommunications(ILogger logger, string userId, string departmentId);

    [LoggerMessage(EventId = 4108, Level = LogLevel.Information,
        Message = "Usuário {UserId} saiu das comunicações do departamento {DepartmentId}")]
    private static partial void LogLeftDepartmentCommunications(ILogger logger, string userId, string departmentId);

    [LoggerMessage(EventId = 4109, Level = LogLevel.Information,
        Message = "Comunicação departamental enviada: {UserId} ({Role}) -> Departamento {DepartmentId} - Título: {Title}")]
    private static partial void LogDepartmentCommunicationSent(ILogger logger, string userId, string role, string departmentId, string title);

    #endregion
}