using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SynQcore.Api.Hubs;
using SynQcore.Application.Common.Interfaces;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Gerencia comunicações corporativas em tempo real e presença de funcionários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class CorporateCommunicationController : ControllerBase
{
    private readonly IHubContext<CorporateCollaborationHub> _collaborationHub;
    private readonly IHubContext<ExecutiveCommunicationHub> _executiveHub;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CorporateCommunicationController> _logger;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public CorporateCommunicationController(
        IHubContext<CorporateCollaborationHub> collaborationHub,
        IHubContext<ExecutiveCommunicationHub> executiveHub,
        ICurrentUserService currentUserService,
        ILogger<CorporateCommunicationController> logger)
    {
        _collaborationHub = collaborationHub;
        _executiveHub = executiveHub;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Obter status de presença de todos os usuários conectados
    /// </summary>
    /// <returns>Lista de usuários online</returns>
    [HttpGet("presence/online-users")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetOnlineUsers()
    {
        var currentUserId = _currentUserService.UserId;

        LogGettingOnlineUsers(_logger, currentUserId.ToString());

        // Em uma implementação real, isso viria de um cache Redis ou serviço de presença
        var onlineUsers = new
        {
            TotalOnline = 42, // Mockado para demonstração
            OnlineByRole = new
            {
                Admin = 2,
                Manager = 8,
                HR = 3,
                Employee = 29
            },
            LastUpdated = DateTimeOffset.UtcNow
        };

        return Ok(onlineUsers);
    }

    /// <summary>
    /// Enviar notificação corporativa para todos os funcionários
    /// </summary>
    /// <param name="request">Dados da notificação</param>
    /// <returns>Status do envio</returns>
    [HttpPost("notifications/company-wide")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<IActionResult> SendCompanyWideNotification([FromBody] CompanyNotificationRequest request)
    {
        var currentUserId = _currentUserService.UserId;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

        LogSendingCompanyNotification(_logger, currentUserId.ToString(), userRole, request.Title);

        // Enviar via SignalR Hub Executivo
        await _executiveHub.Clients.Group("CompanyAnnouncements")
            .SendAsync("ReceiveCompanyNotification", new
            {
                NotificationId = Guid.NewGuid(),
                Title = request.Title,
                Message = request.Message,
                Priority = request.Priority,
                Type = request.Type,
                SentBy = new
                {
                    UserId = currentUserId,
                    Role = userRole
                },
                Timestamp = DateTimeOffset.UtcNow
            });

        return Ok(new
        {
            Success = true,
            Message = "Notificação enviada para todos os funcionários",
            NotificationId = Guid.NewGuid(),
            Recipients = "Company-wide",
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Enviar mensagem para canal de equipe específica
    /// </summary>
    /// <param name="teamId">ID da equipe</param>
    /// <param name="request">Dados da mensagem</param>
    /// <returns>Status do envio</returns>
    [HttpPost("teams/{teamId}/messages")]
    public async Task<IActionResult> SendTeamMessage(string teamId, [FromBody] TeamMessageRequest request)
    {
        var currentUserId = _currentUserService.UserId;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

        LogSendingTeamMessage(_logger, currentUserId.ToString(), teamId, request.Message.Length);

        // Enviar via SignalR Hub Colaborativo
        await _collaborationHub.Clients.Group($"Team_{teamId}")
            .SendAsync("ReceiveTeamMessage", new
            {
                MessageId = Guid.NewGuid(),
                TeamId = teamId,
                Message = request.Message,
                SentBy = new
                {
                    UserId = currentUserId,
                    Email = userEmail
                },
                Timestamp = DateTimeOffset.UtcNow
            });

        return Ok(new
        {
            Success = true,
            Message = "Mensagem enviada para a equipe",
            MessageId = Guid.NewGuid(),
            TeamId = teamId,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Enviar comunicado para departamento específico
    /// </summary>
    /// <param name="departmentId">ID do departamento</param>
    /// <param name="request">Dados do comunicado</param>
    /// <returns>Status do envio</returns>
    [HttpPost("departments/{departmentId}/communications")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<IActionResult> SendDepartmentCommunication(string departmentId, [FromBody] DepartmentCommunicationRequest request)
    {
        var currentUserId = _currentUserService.UserId;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

        LogSendingDepartmentCommunication(_logger, currentUserId.ToString(), departmentId, request.Title);

        // Enviar via SignalR Hub Executivo
        await _executiveHub.Clients.Group($"Department_{departmentId}")
            .SendAsync("ReceiveDepartmentCommunication", new
            {
                CommunicationId = Guid.NewGuid(),
                DepartmentId = departmentId,
                Title = request.Title,
                Message = request.Message,
                Priority = request.Priority,
                SentBy = new
                {
                    UserId = currentUserId,
                    Role = userRole
                },
                Timestamp = DateTimeOffset.UtcNow
            });

        return Ok(new
        {
            Success = true,
            Message = "Comunicado enviado para o departamento",
            CommunicationId = Guid.NewGuid(),
            DepartmentId = departmentId,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Obter histórico de mensagens de uma equipe
    /// </summary>
    /// <param name="teamId">ID da equipe</param>
    /// <param name="page">Página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 50)</param>
    /// <returns>Histórico de mensagens</returns>
    [HttpGet("teams/{teamId}/messages")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetTeamMessageHistory(string teamId, int page = 1, int pageSize = 50)
    {
        var currentUserId = _currentUserService.UserId;

        LogGettingTeamMessages(_logger, currentUserId.ToString(), teamId, page);

        // Em implementação real, buscar do banco de dados
        var messageHistory = new
        {
            TeamId = teamId,
            Page = page,
            PageSize = pageSize,
            TotalMessages = 150, // Mockado
            Messages = new[]
            {
                new
                {
                    MessageId = Guid.NewGuid(),
                    Message = "Exemplo de mensagem da equipe",
                    SentBy = new { UserId = Guid.NewGuid(), Email = "usuario@empresa.com" },
                    Timestamp = DateTimeOffset.UtcNow.AddHours(-2)
                }
            }
        };

        return Ok(messageHistory);
    }

    /// <summary>
    /// Obter estatísticas de comunicação corporativa
    /// </summary>
    /// <returns>Estatísticas detalhadas</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Manager,HR,Admin")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public IActionResult GetCommunicationStatistics()
    {
        var currentUserId = _currentUserService.UserId;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

        LogGettingCommunicationStats(_logger, currentUserId.ToString(), userRole);

        // Em implementação real, calcular do banco de dados e cache
        var statistics = new
        {
            Today = new
            {
                TotalMessages = 1247,
                CompanyAnnouncements = 3,
                TeamMessages = 892,
                DepartmentCommunications = 15,
                ExecutiveCommunications = 2
            },
            ThisWeek = new
            {
                TotalMessages = 8934,
                ActiveTeams = 23,
                ActiveDepartments = 8,
                OnlineUsers = 67
            },
            TopActiveTeams = new[]
            {
                new { TeamId = "team-1", Name = "Desenvolvimento", MessageCount = 234 },
                new { TeamId = "team-2", Name = "Marketing", MessageCount = 189 },
                new { TeamId = "team-3", Name = "Vendas", MessageCount = 156 }
            },
            LastUpdated = DateTimeOffset.UtcNow
        };

        return Ok(statistics);
    }

    #region Request DTOs

    /// <summary>
    /// Dados para notificação corporativa
    /// </summary>
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
    public class CompanyNotificationRequest
    {
        /// <summary>
        /// Título da notificação
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo da mensagem
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Prioridade da notificação (Low, Normal, High, Critical)
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Priority { get; set; } = "Normal";

        /// <summary>
        /// Tipo de notificação (Announcement, Policy, Emergency, Update)
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Type { get; set; } = "Announcement";
    }

    /// <summary>
    /// Dados para mensagem de equipe
    /// </summary>
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
    public class TeamMessageRequest
    {
        /// <summary>
        /// Conteúdo da mensagem
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Dados para comunicado departamental
    /// </summary>
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
    public class DepartmentCommunicationRequest
    {
        /// <summary>
        /// Título do comunicado
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo da mensagem
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Prioridade do comunicado (Low, Normal, High, Urgent)
        /// </summary>
        /// <summary>Propriedade do sistema</summary>
        public string Priority { get; set; } = "Normal";
    }

    #endregion

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 4201, Level = LogLevel.Information,
        Message = "Obtendo usuários online - UserId: {UserId}")]
    private static partial void LogGettingOnlineUsers(ILogger logger, string userId);

    [LoggerMessage(EventId = 4202, Level = LogLevel.Information,
        Message = "Enviando notificação corporativa - UserId: {UserId} ({Role}) - Título: {Title}")]
    private static partial void LogSendingCompanyNotification(ILogger logger, string userId, string role, string title);

    [LoggerMessage(EventId = 4203, Level = LogLevel.Information,
        Message = "Enviando mensagem para equipe - UserId: {UserId} -> TeamId: {TeamId} ({MessageLength} chars)")]
    private static partial void LogSendingTeamMessage(ILogger logger, string userId, string teamId, int messageLength);

    [LoggerMessage(EventId = 4204, Level = LogLevel.Information,
        Message = "Enviando comunicado departamental - UserId: {UserId} -> DepartmentId: {DepartmentId} - Título: {Title}")]
    private static partial void LogSendingDepartmentCommunication(ILogger logger, string userId, string departmentId, string title);

    [LoggerMessage(EventId = 4205, Level = LogLevel.Information,
        Message = "Obtendo histórico de mensagens da equipe - UserId: {UserId} -> TeamId: {TeamId} (Página: {Page})")]
    private static partial void LogGettingTeamMessages(ILogger logger, string userId, string teamId, int page);

    [LoggerMessage(EventId = 4206, Level = LogLevel.Information,
        Message = "Obtendo estatísticas de comunicação - UserId: {UserId} ({Role})")]
    private static partial void LogGettingCommunicationStats(ILogger logger, string userId, string role);

    #endregion
}
