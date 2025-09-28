using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Documentação e informações dos Hubs SignalR para comunicação em tempo real
/// </summary>
[ApiController]
[Route("api/[controller]")]
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
public class SignalRDocumentationController : ControllerBase
{
    /// <summary>
    /// Obter informações sobre todos os Hubs SignalR disponíveis
    /// </summary>
    /// <returns>Lista de Hubs e seus métodos</returns>
    [HttpGet("hubs")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetAvailableHubs()
    {
        var hubsInfo = new
        {
            CorporateCollaborationHub = new
            {
                Name = "CorporateCollaborationHub",
                Endpoint = "/hubs/corporate-collaboration",
                Description = "Hub principal para colaboração corporativa em tempo real",
                Authentication = "JWT Bearer Token via query string (?access_token=TOKEN)",
                Methods = "JoinTeamChannel, LeaveTeamChannel, JoinProjectChannel, LeaveProjectChannel, SendTeamMessage, SendProjectMessage, UpdatePresenceStatus",
                Events = "UserOnline, UserOffline, ReceiveTeamMessage, ReceiveProjectMessage, UserPresenceChanged"
            },
            ExecutiveCommunicationHub = new
            {
                Name = "ExecutiveCommunicationHub", 
                Endpoint = "/hubs/executive-communication",
                Description = "Hub especializado para comunicações executivas e anúncios corporativos",
                Authentication = "JWT Bearer Token via query string (?access_token=TOKEN)",
                Methods = "SendCompanyAnnouncement (Manager+), SendExecutiveCommunication (Manager+), SendPolicyUpdate (HR+), JoinDepartmentCommunications, SendDepartmentCommunication (Manager+)",
                Events = "ReceiveCompanyAnnouncement, ReceiveExecutiveCommunication, ReceivePolicyUpdate, ReceiveDepartmentCommunication"
            },
            ConnectionInfo = new
            {
                AuthenticationMethod = "JWT Bearer Token",
                AuthenticationLocation = "Query String Parameter 'access_token'",
                Example = "wss://localhost:5000/hubs/corporate-collaboration?access_token=YOUR_JWT_TOKEN",
                SupportedProtocols = "WebSocket, ServerSentEvents, LongPolling"
            }
        };

        return Ok(hubsInfo);
    }

    /// <summary>
    /// Obter exemplo de conexão JavaScript para os Hubs SignalR
    /// </summary>
    /// <returns>Código JavaScript de exemplo</returns>
    [HttpGet("examples/javascript")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetJavaScriptExamples()
    {
        var examples = new
        {
            InstallSignalRClient = "npm install @microsoft/signalr",
            CorporateCollaborationExample = @"
// Conectar ao Hub de Colaboração Corporativa
const collaborationConnection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/corporate-collaboration', {
        accessTokenFactory: () => localStorage.getItem('jwt_token')
    })
    .build();

// Configurar eventos
collaborationConnection.on('ReceiveTeamMessage', (data) => {
    addMessageToUI(data.TeamId, data.Message, data.SentBy);
});

collaborationConnection.on('UserOnline', (data) => {
    updateUserPresence(data.UserId, 'online');
});

collaborationConnection.on('UserOffline', (data) => {
    updateUserPresence(data.UserId, 'offline');
});

// Conectar
await collaborationConnection.start();

// Usar funcionalidades
await collaborationConnection.invoke('JoinTeamChannel', 'team-dev');
await collaborationConnection.invoke('SendTeamMessage', 'team-dev', 'Reunião às 14h!');
await collaborationConnection.invoke('UpdatePresenceStatus', 'Available');
",
            ExecutiveCommunicationExample = @"
// Conectar ao Hub Executivo (requer role Manager/HR/Admin para envios)
const executiveConnection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/executive-communication', {
        accessTokenFactory: () => localStorage.getItem('jwt_token')
    })
    .build();

// Configurar eventos
executiveConnection.on('ReceiveCompanyAnnouncement', (data) => {
    showCompanyAnnouncement(data.Title, data.Message, data.Priority);
});

executiveConnection.on('ReceivePolicyUpdate', (data) => {
    showPolicyUpdate(data.PolicyTitle, data.ChangeDescription, data.RequiresAcknowledgment);
});

// Conectar
await executiveConnection.start();

// Usar funcionalidades (apenas Manager/HR/Admin)
await executiveConnection.invoke('SendCompanyAnnouncement', 
    'Política de Home Office', 
    'Nova política entra em vigor na próxima semana.', 
    'High'
);
",
            ErrorHandling = @"
// Tratamento de erros
connection.onclose((error) => {
    console.error('Conexão perdida:', error);
    // Implementar reconexão automática
    setTimeout(() => startConnection(), 5000);
});

connection.onreconnecting((error) => {
    console.warn('Tentando reconectar:', error);
});

connection.onreconnected((connectionId) => {
    console.info('Reconectado:', connectionId);
});
"
        };

        return Ok(examples);
    }

    /// <summary>
    /// Obter status atual dos Hubs SignalR (conexões ativas, grupos, etc.)
    /// </summary>
    /// <returns>Estatísticas dos Hubs</returns>
    [HttpGet("status")]
    [Authorize(Roles = "Manager,HR,Admin")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetHubStatus()
    {
        // Em implementação real, isso viria do SignalR Hub Context
        var status = new
        {
            CorporateCollaborationHub = new
            {
                ActiveConnections = 42,
                ActiveGroups = new
                {
                    TeamChannels = 8,
                    ProjectChannels = 5,
                    RoleGroups = 4
                },
                MessagesSentToday = 1247,
                LastActivity = DateTimeOffset.UtcNow.AddMinutes(-2)
            },
            ExecutiveCommunicationHub = new
            {
                ActiveConnections = 67,
                ActiveGroups = new
                {
                    CompanyAnnouncements = 67,
                    ExecutiveBroadcast = 12,
                    DepartmentChannels = 8
                },
                AnnouncementsSentToday = 3,
                LastActivity = DateTimeOffset.UtcNow.AddMinutes(-15)
            },
            SystemHealth = new
            {
                Status = "Healthy",
                UpTime = TimeSpan.FromHours(4.2).ToString(),
                LastRestart = DateTimeOffset.UtcNow.AddHours(-4),
                TotalConnectionsToday = 234
            }
        };

        return Ok(status);
    }

    /// <summary>
    /// Obter documentação para implementação de cliente SignalR
    /// </summary>
    /// <returns>Guia de implementação</returns>
    [HttpGet("implementation-guide")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetImplementationGuide()
    {
        var guide = new
        {
            Overview = "SynQcore SignalR Hubs fornecem comunicação em tempo real para ambientes corporativos",
            RequiredLibraries = new
            {
                JavaScript = "@microsoft/signalr",
                DotNetClient = "Microsoft.AspNetCore.SignalR.Client",
                JavaClient = "com.microsoft.signalr"
            },
            AuthenticationFlow = "1. Login → JWT Token → Include in SignalR connection → Auto validation → Group assignment",
            BestPractices = "Reconexão automática, Error handling, Permission validation, Client throttling, Heartbeat, Offline cache",
            CommonPatterns = new
            {
                TeamCollaboration = "JoinTeamChannel → SendTeamMessage → ReceiveTeamMessage",
                PresenceManagement = "OnConnectedAsync → UpdatePresenceStatus → UserPresenceChanged",
                CompanyAnnouncements = "SendCompanyAnnouncement → ReceiveCompanyAnnouncement",
                DepartmentCommunication = "JoinDepartmentCommunications → SendDepartmentCommunication → ReceiveDepartmentCommunication"
            }
        };

        return Ok(guide);
    }
}