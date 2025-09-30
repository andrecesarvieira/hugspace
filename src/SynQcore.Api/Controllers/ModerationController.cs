using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Application.Features.Moderation.Queries;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para funcionalidades de moderação corporativa
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ModerationController> _logger;

    public ModerationController(IMediator mediator, ILogger<ModerationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém estatísticas do dashboard de moderação
    /// </summary>
    /// <param name="referenceDate">Data de referência para cálculos (opcional, padrão: agora)</param>
    /// <returns>Estatísticas detalhadas para o dashboard</returns>
    [HttpGet("dashboard/stats")]
    [ProducesResponseType(typeof(ModerationDashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ModerationDashboardStatsDto>> GetDashboardStats(
        [FromQuery] DateTime? referenceDate = null)
    {
        var query = new GetModerationDashboardStatsQuery
        {
            ReferenceDate = referenceDate ?? DateTime.UtcNow
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Busca logs de auditoria com filtros para moderação
    /// </summary>
    /// <param name="startDate">Data de início</param>
    /// <param name="endDate">Data de fim</param>
    /// <param name="actionType">Tipo de ação</param>
    /// <param name="severity">Nível de severidade</param>
    /// <param name="category">Categoria do evento</param>
    /// <param name="userId">ID do usuário</param>
    /// <param name="ipAddress">Endereço IP</param>
    /// <param name="onlyRequiringAttention">Apenas logs que requerem atenção</param>
    /// <param name="page">Página atual</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de logs de auditoria</returns>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(PagedResult<ModerationAuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ModerationAuditLogDto>>> GetAuditLogs(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Domain.Entities.AuditActionType? actionType = null,
        [FromQuery] Domain.Entities.AuditSeverity? severity = null,
        [FromQuery] Domain.Entities.AuditCategory? category = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? ipAddress = null,
        [FromQuery] bool onlyRequiringAttention = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetModerationAuditLogsQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            ActionType = actionType,
            Severity = severity,
            Category = category,
            UserId = userId,
            IpAddress = ipAddress,
            OnlyRequiringAttention = onlyRequiringAttention,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtém logs que requerem atenção
    /// </summary>
    /// <param name="page">Página atual</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="minimumSeverity">Severidade mínima</param>
    /// <returns>Lista paginada de logs que requerem atenção</returns>
    [HttpGet("attention-required")]
    [ProducesResponseType(typeof(PagedResult<ModerationAuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ModerationAuditLogDto>>> GetLogsRequiringAttention(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Domain.Entities.AuditSeverity minimumSeverity = Domain.Entities.AuditSeverity.Warning)
    {
        var query = new GetLogsRequiringAttentionQuery
        {
            Page = page,
            PageSize = pageSize,
            MinimumSeverity = minimumSeverity
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtém eventos de segurança recentes
    /// </summary>
    /// <param name="hoursBack">Horas atrás para buscar eventos</param>
    /// <param name="limit">Limite de resultados</param>
    /// <returns>Lista de eventos de segurança</returns>
    [HttpGet("security-events")]
    [ProducesResponseType(typeof(List<ModerationAuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ModerationAuditLogDto>>> GetRecentSecurityEvents(
        [FromQuery] int hoursBack = 24,
        [FromQuery] int limit = 50)
    {
        var query = new GetRecentSecurityEventsQuery
        {
            HoursBack = hoursBack,
            Limit = limit
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtém atividade por IP
    /// </summary>
    /// <param name="startDate">Data de início</param>
    /// <param name="endDate">Data de fim</param>
    /// <param name="limit">Limite de resultados</param>
    /// <param name="onlySuspicious">Apenas IPs suspeitos</param>
    /// <returns>Lista de atividades por IP</returns>
    [HttpGet("ip-activity")]
    [ProducesResponseType(typeof(List<IpActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IpActivityDto>>> GetIpActivity(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int limit = 10,
        [FromQuery] bool onlySuspicious = false)
    {
        var query = new GetIpActivityQuery
        {
            StartDate = startDate ?? DateTime.UtcNow.AddDays(-1),
            EndDate = endDate ?? DateTime.UtcNow,
            Limit = limit,
            OnlySuspicious = onlySuspicious
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Exporta logs de auditoria
    /// </summary>
    /// <param name="startDate">Data de início</param>
    /// <param name="endDate">Data de fim</param>
    /// <param name="format">Formato de exportação (CSV, Excel, JSON)</param>
    /// <returns>Arquivo com logs exportados</returns>
    [HttpGet("export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportAuditLogs(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string format = "CSV")
    {
        if (startDate >= endDate)
        {
            return BadRequest("Data de início deve ser anterior à data de fim");
        }

        if ((endDate - startDate).TotalDays > 90)
        {
            return BadRequest("Período de exportação não pode exceder 90 dias");
        }

        var query = new ExportAuditLogsQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            Format = format.ToUpperInvariant()
        };

        var fileContent = await _mediator.Send(query);

        var contentType = format.ToUpperInvariant() switch
        {
            "EXCEL" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "JSON" => "application/json",
            _ => "text/csv"
        };

        var fileName = $"audit-logs-{startDate:yyyy-MM-dd}-{endDate:yyyy-MM-dd}.{format.ToLowerInvariant()}";

        return File(fileContent, contentType, fileName);
    }

    /// <summary>
    /// Obtém estatísticas de usuário específico
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="periodDays">Período em dias</param>
    /// <returns>Estatísticas detalhadas do usuário</returns>
    [HttpGet("user/{userId}/stats")]
    [ProducesResponseType(typeof(UserAuditStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserAuditStatsDto>> GetUserAuditStats(
        string userId,
        [FromQuery] int periodDays = 30)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("ID do usuário é obrigatório");
        }

        var query = new GetUserAuditStatsQuery
        {
            UserId = userId,
            PeriodDays = periodDays
        };

        var result = await _mediator.Send(query);

        if (result.TotalActions == 0)
        {
            return NotFound("Usuário não encontrado ou sem atividade no período especificado");
        }

        return Ok(result);
    }
}
