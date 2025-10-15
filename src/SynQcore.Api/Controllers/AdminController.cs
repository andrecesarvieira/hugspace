using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SynQcore.Api.Hubs;
using SynQcore.Application.Commands.Admin;
using SynQcore.Application.DTOs.Admin;
using SynQcore.Application.DTOs.Auth;
using SynQcore.Application.Queries.Admin;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para funcionalidades administrativas do sistema corporativo
/// Fornece endpoints para gerenciamento de usuários e configurações (apenas Admin)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
/// <summary>
/// Classe para operações do sistema
/// </summary>
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<CorporateNotificationHub> _notificationHub;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public AdminController(IMediator mediator, IHubContext<CorporateNotificationHub> notificationHub)
    {
        _mediator = mediator;
        _notificationHub = notificationHub;
    }

    /// <summary>
    /// Listar todos os usuários do sistema com paginação e busca
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 10)</param>
    /// <param name="searchTerm">Termo para busca por nome ou email</param>
    /// <returns>Lista paginada de usuários do sistema</returns>
    [HttpGet("users")]
    public async Task<ActionResult<UsersListResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        // Validar e ajustar parâmetros de paginação
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Limitar tamanho máximo da página

        var query = new GetAllUsersQuery(page, pageSize, searchTerm);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Criar novo usuário com papel específico (apenas administradores)
    /// </summary>
    /// <param name="request">Dados do novo usuário incluindo papel</param>
    /// <returns>Usuário criado com confirmação</returns>
    [HttpPost("users")]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.Role);

        var response = await _mediator.Send(command);

        if (!response.Success)
            return BadRequest(new { message = response.Message });

        return CreatedAtAction(
            nameof(GetUsers),
            new { searchTerm = response.Email },
            response);
    }

    /// <summary>
    /// Listar papéis disponíveis no sistema
    /// </summary>
    /// <returns>Lista de papéis corporativos disponíveis</returns>
    [HttpGet("roles")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public ActionResult<IEnumerable<string>> GetAvailableRoles()
    {
        var roles = new[] { "Employee", "Manager", "HR", "Admin" };
        return Ok(roles);
    }

    /// <summary>
    /// Enviar notificação de teste via SignalR
    /// </summary>
    /// <param name="message">Mensagem da notificação</param>
    /// <param name="type">Tipo da notificação (Info, Warning, Error)</param>
    /// <returns>Status do envio da notificação</returns>
    [HttpPost("test-notification")]
    public async Task<ActionResult> SendTestNotification(
        [FromQuery] string message = "Notificação de teste",
        [FromQuery] string type = "Info")
    {
        try
        {
            var notification = new
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Teste do Sistema",
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Action = new
                {
                    Label = "Ver",
                    Url = "/feed"
                }
            };

            // Enviar para todos os usuários conectados
            await _notificationHub.Clients.All.SendAsync("ReceberNotificacao", notification);

            return Ok(new
            {
                message = "Notificação enviada com sucesso",
                notification = notification
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Erro ao enviar notificação: {ex.Message}" });
        }
    }
}
