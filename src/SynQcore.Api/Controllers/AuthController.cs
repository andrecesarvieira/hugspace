using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SynQcore.Api.Hubs;
using SynQcore.Application.Commands.Auth;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para autenticação corporativa
/// Gerencia registro e login de funcionários
/// </summary>
[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Classe para operações do sistema
/// </summary>
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<CorporateNotificationHub> _notificationHub;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public AuthController(IMediator mediator, IHubContext<CorporateNotificationHub> notificationHub)
    {
        _mediator = mediator;
        _notificationHub = notificationHub;
    }

    /// <summary>
    /// Registrar novo funcionário no sistema corporativo
    /// </summary>
    /// <param name="request">Dados de registro do funcionário</param>
    /// <returns>Resposta com token JWT se sucesso</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.PhoneNumber);

        var response = await _mediator.Send(command);

        if (!response.Success)
            return BadRequest(new { message = response.Message });

        return Ok(response);
    }

    /// <summary>
    /// Autenticar funcionário e gerar token JWT
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Resposta com token JWT se autenticação válida</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var response = await _mediator.Send(command);

        if (!response.Success)
            return Unauthorized(new { message = response.Message });

        return Ok(response);
    }

    /// <summary>
    /// Enviar notificação de teste para todos os clientes conectados
    /// </summary>
    /// <param name="message">Mensagem da notificação</param>
    /// <returns>Status do envio</returns>
    [HttpPost("test-notification")]
    public async Task<ActionResult> SendTestNotification([FromQuery] string message = "Teste de notificação")
    {
        try
        {
            var notification = new
            {
                Id = Guid.NewGuid().ToString(),
                Title = "🧪 Teste do Sistema",
                Message = message,
                Type = "Info",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Action = new
                {
                    Label = "Ver Feed",
                    Url = "/feed"
                }
            };

            // Enviar para todos os clientes conectados
            await _notificationHub.Clients.All.SendAsync("ReceberNotificacao", notification);

            return Ok(new
            {
                success = true,
                message = "Notificação enviada com sucesso via SignalR",
                notification = notification,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = $"Erro ao enviar notificação: {ex.Message}",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
