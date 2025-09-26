using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Application.Features.Notifications.Queries;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de notificações corporativas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Criar uma nova notificação corporativa
    /// </summary>
    /// <param name="command">Dados da notificação</param>
    /// <returns>Response com ID da notificação criada</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<ActionResult<CreateNotificationResponse>> CreateNotification(CreateNotificationCommand command)
    {
        var response = await _mediator.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }

        return CreatedAtAction(nameof(GetNotificationDetails), new { id = response.NotificationId }, response);
    }

    /// <summary>
    /// Obter notificações do funcionário logado
    /// </summary>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Itens por página</param>
    /// <param name="isRead">Filtro por leitura</param>
    /// <param name="type">Filtro por tipo</param>
    /// <param name="priority">Filtro por prioridade</param>
    /// <param name="dateFrom">Data inicial</param>
    /// <param name="dateTo">Data final</param>
    /// <returns>Lista paginada de notificações</returns>
    [HttpGet("my-notifications")]
    public async Task<ActionResult<GetEmployeeNotificationsResponse>> GetMyNotifications(
        int page = 1, 
        int pageSize = 20,
        bool? isRead = null,
        string? type = null,
        string? priority = null,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null)
    {
        // Obter ID do funcionário do token JWT
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
        if (!Guid.TryParse(userIdClaim, out var employeeId))
        {
            return BadRequest(new { message = "ID do funcionário não encontrado no token" });
        }

        var query = new GetEmployeeNotificationsQuery
        {
            EmployeeId = employeeId,
            Page = page,
            PageSize = pageSize,
            IsRead = isRead,
            Type = type,
            Priority = priority,
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Obter todas as notificações corporativas (Admin/Manager)
    /// </summary>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Itens por página</param>
    /// <param name="status">Filtro por status</param>
    /// <param name="type">Filtro por tipo</param>
    /// <param name="priority">Filtro por prioridade</param>
    /// <param name="createdBy">Filtro por criador</param>
    /// <param name="targetDepartmentId">Filtro por departamento</param>
    /// <param name="dateFrom">Data inicial</param>
    /// <param name="dateTo">Data final</param>
    /// <returns>Lista paginada de notificações corporativas</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<ActionResult<GetCorporateNotificationsResponse>> GetCorporateNotifications(
        int page = 1,
        int pageSize = 20,
        string? status = null,
        string? type = null,
        string? priority = null,
        Guid? createdBy = null,
        Guid? targetDepartmentId = null,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null)
    {
        var query = new GetCorporateNotificationsQuery
        {
            Page = page,
            PageSize = pageSize,
            Status = status,
            Type = type,
            Priority = priority,
            CreatedBy = createdBy,
            TargetDepartmentId = targetDepartmentId,
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Obter detalhes de uma notificação específica
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <param name="includeDeliveries">Incluir entregas detalhadas</param>
    /// <returns>Detalhes da notificação</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetNotificationDetailsResponse>> GetNotificationDetails(
        Guid id, 
        bool includeDeliveries = false)
    {
        var query = new GetNotificationDetailsQuery 
        { 
            NotificationId = id,
            IncludeDeliveries = includeDeliveries
        };

        var response = await _mediator.Send(query);
        
        if (response.Notification.Id == Guid.Empty)
        {
            return NotFound(new { message = "Notificação não encontrada" });
        }

        return Ok(response);
    }

    /// <summary>
    /// Aprovar ou rejeitar uma notificação
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <param name="command">Dados de aprovação</param>
    /// <returns>Resultado da aprovação</returns>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<ActionResult<ApproveNotificationResponse>> ApproveNotification(
        Guid id, 
        ApproveNotificationCommand command)
    {
        // Garantir que o ID da URL corresponde ao comando
        var updatedCommand = command with { NotificationId = id };
        
        var response = await _mediator.Send(updatedCommand);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }

    /// <summary>
    /// Enviar uma notificação aprovada
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <returns>Resultado do envio</returns>
    [HttpPost("{id:guid}/send")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<ActionResult<SendNotificationResponse>> SendNotification(Guid id)
    {
        var command = new SendNotificationCommand { NotificationId = id };
        var response = await _mediator.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }

    /// <summary>
    /// Marcar notificação como lida
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("{id:guid}/mark-read")]
    public async Task<ActionResult<MarkNotificationAsReadResponse>> MarkAsRead(Guid id)
    {
        // Obter ID do funcionário do token JWT
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
        if (!Guid.TryParse(userIdClaim, out var employeeId))
        {
            return BadRequest(new { message = "ID do funcionário não encontrado no token" });
        }

        var command = new MarkNotificationAsReadCommand 
        { 
            NotificationId = id,
            EmployeeId = employeeId
        };
        
        var response = await _mediator.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }

    /// <summary>
    /// Atualizar uma notificação (apenas rascunhos)
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <param name="command">Dados para atualização</param>
    /// <returns>Resultado da atualização</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<ActionResult<UpdateNotificationResponse>> UpdateNotification(
        Guid id, 
        UpdateNotificationCommand command)
    {
        // Garantir que o ID da URL corresponde ao comando
        var updatedCommand = command with { NotificationId = id };
        
        var response = await _mediator.Send(updatedCommand);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }

    /// <summary>
    /// Cancelar uma notificação
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <param name="command">Motivo do cancelamento</param>
    /// <returns>Resultado do cancelamento</returns>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<ActionResult<CancelNotificationResponse>> CancelNotification(
        Guid id, 
        CancelNotificationCommand command)
    {
        // Garantir que o ID da URL corresponde ao comando
        var updatedCommand = command with { NotificationId = id };
        
        var response = await _mediator.Send(updatedCommand);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }
}