using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Features.Notifications.Queries;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento administrativo de notificações
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
public class NotificationManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obter estatísticas de notificações corporativas
    /// </summary>
    /// <param name="dateFrom">Data inicial</param>
    /// <param name="dateTo">Data final</param>
    /// <param name="departmentId">Filtro por departamento</param>
    /// <returns>Estatísticas detalhadas</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<GetNotificationStatsResponse>> GetNotificationStats(
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        Guid? departmentId = null)
    {
        var query = new GetNotificationStatsQuery
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            DepartmentId = departmentId
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Obter templates de notificação disponíveis
    /// </summary>
    /// <param name="type">Filtro por tipo</param>
    /// <param name="category">Filtro por categoria</param>
    /// <param name="activeOnly">Apenas templates ativos</param>
    /// <returns>Lista de templates</returns>
    [HttpGet("templates")]
    public async Task<ActionResult<GetNotificationTemplatesResponse>> GetNotificationTemplates(
        string? type = null,
        string? category = null,
        bool activeOnly = true)
    {
        var query = new GetNotificationTemplatesQuery
        {
            Type = type,
            Category = category,
            ActiveOnly = activeOnly
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Obter aprovações pendentes
    /// </summary>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Itens por página</param>
    /// <returns>Lista de notificações pendentes de aprovação</returns>
    [HttpGet("pending-approvals")]
    public async Task<ActionResult<GetPendingApprovalsResponse>> GetPendingApprovals(
        int page = 1,
        int pageSize = 20)
    {
        // Obter ID do usuário do token JWT
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
        if (!Guid.TryParse(userIdClaim, out var approverId))
        {
            return BadRequest(new { message = "ID do funcionário não encontrado no token" });
        }

        var query = new GetPendingApprovalsQuery
        {
            ApproverId = approverId,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }
}