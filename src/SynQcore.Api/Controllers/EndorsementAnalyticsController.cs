using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SynQcore.Application.Features.Collaboration.Queries;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Common.DTOs;
using System.Security.Claims;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para analytics e relatórios de endorsements corporativos
/// </summary>
[ApiController]
[Route("api/endorsements/analytics")]
[Authorize]
[Produces("application/json")]
public partial class EndorsementAnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EndorsementAnalyticsController> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, Exception?> LogProcessingAnalyticsRequest =
        LoggerMessage.Define(LogLevel.Information, new EventId(5001, nameof(LogProcessingAnalyticsRequest)), 
            "Processando requisição de analytics de endorsements");

    private static readonly Action<ILogger, int, Exception?> LogRankingGenerated =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5002, nameof(LogRankingGenerated)), 
            "Ranking gerado com {Count} funcionários");

    private static readonly Action<ILogger, int, Exception?> LogTrendingGenerated =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5003, nameof(LogTrendingGenerated)), 
            "Trending gerado com {Count} tipos");

    private static readonly Action<ILogger, Guid, Exception?> LogEmployeeAnalyticsRequest =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5004, nameof(LogEmployeeAnalyticsRequest)), 
            "Gerando analytics para funcionário: {EmployeeId}");

    // LoggerMessage delegates para erros
    [LoggerMessage(EventId = 5101, Level = LogLevel.Error,
        Message = "Erro ao gerar ranking de endorsements")]
    private static partial void LogRankingError(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 5102, Level = LogLevel.Error,
        Message = "Erro ao gerar trending de endorsements")]
    private static partial void LogTrendingError(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 5103, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsements dados por {EmployeeId}")]
    private static partial void LogEmployeeEndorsementsGivenError(ILogger logger, Guid employeeId, Exception ex);

    [LoggerMessage(EventId = 5104, Level = LogLevel.Error,
        Message = "Erro ao buscar endorsements recebidos por {EmployeeId}")]
    private static partial void LogEmployeeEndorsementsReceivedError(ILogger logger, Guid employeeId, Exception ex);

    [LoggerMessage(EventId = 5105, Level = LogLevel.Error,
        Message = "Erro ao verificar endorsement do usuário {UserId}")]
    private static partial void LogCheckEndorsementError(ILogger logger, Guid userId, Exception ex);

    [LoggerMessage(EventId = 5106, Level = LogLevel.Error,
        Message = "Erro ao gerar dashboard executivo de endorsements")]
    private static partial void LogDashboardError(ILogger logger, Exception ex);

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public EndorsementAnalyticsController(IMediator mediator, ILogger<EndorsementAnalyticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obter ID do usuário autenticado dos claims JWT
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Ranking de funcionários por endorsements corporativos
    /// </summary>
    [HttpGet("ranking")]
    [Authorize(Roles = "Manager,HR,Admin")]
    [ProducesResponseType(typeof(List<EmployeeEndorsementRankingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<EmployeeEndorsementRankingDto>>> GetEmployeeRanking(
        [FromQuery] int topCount = 10,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string rankingType = "Engagement")
    {
        LogProcessingAnalyticsRequest(_logger, null);

        try
        {
            var query = new GetEmployeeEndorsementRankingQuery
            {
                TopCount = Math.Min(topCount, 50), // Limitar para performance
                DepartmentId = departmentId,
                StartDate = startDate,
                EndDate = endDate,
                RankingType = rankingType
            };
            var result = await _mediator.Send(query);
            
            LogRankingGenerated(_logger, result.Count, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogRankingError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao gerar ranking.");
        }
    }

    /// <summary>
    /// Tipos de endorsement em tendência por período
    /// </summary>
    [HttpGet("trending")]
    [Authorize(Roles = "Manager,HR,Admin")]
    [ProducesResponseType(typeof(List<EndorsementTypeTrendDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<EndorsementTypeTrendDto>>> GetTrendingTypes(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] int topCount = 5)
    {
        LogProcessingAnalyticsRequest(_logger, null);

        try
        {
            // Definir período padrão se não especificado (últimos 30 dias)
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var query = new GetTrendingEndorsementTypesQuery
            {
                StartDate = start,
                EndDate = end,
                DepartmentId = departmentId,
                TopCount = Math.Min(topCount, 10) // Limitar para performance
            };
            var result = await _mediator.Send(query);
            
            LogTrendingGenerated(_logger, result.Count, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTrendingError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao gerar trending.");
        }
    }

    /// <summary>
    /// Endorsements dados por funcionário específico
    /// </summary>
    [HttpGet("employee/{employeeId:guid}/given")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(PagedResult<EndorsementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<EndorsementDto>>> GetEmployeeEndorsementsGiven(
        Guid employeeId,
        [FromQuery] SynQcore.Domain.Entities.Communication.EndorsementType? filterByType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Verificar se é o próprio usuário ou se tem permissão para ver outros
        var currentUserId = GetCurrentUserId();
        if (employeeId != currentUserId && !User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
        {
            return Forbid("Não é possível visualizar endorsements de outros funcionários.");
        }

        LogEmployeeAnalyticsRequest(_logger, employeeId, null);

        try
        {
            var query = new GetEmployeeEndorsementsGivenQuery
            {
                EmployeeId = employeeId,
                FilterByType = filterByType,
                StartDate = startDate,
                EndDate = endDate,
                Page = page,
                PageSize = Math.Min(pageSize, 50) // Limitar para performance
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogEmployeeEndorsementsGivenError(_logger, employeeId, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Endorsements recebidos por funcionário específico
    /// </summary>
    [HttpGet("employee/{employeeId:guid}/received")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(PagedResult<EndorsementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<EndorsementDto>>> GetEmployeeEndorsementsReceived(
        Guid employeeId,
        [FromQuery] SynQcore.Domain.Entities.Communication.EndorsementType? filterByType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Verificar se é o próprio usuário ou se tem permissão para ver outros
        var currentUserId = GetCurrentUserId();
        if (employeeId != currentUserId && !User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
        {
            return Forbid("Não é possível visualizar endorsements de outros funcionários.");
        }

        LogEmployeeAnalyticsRequest(_logger, employeeId, null);

        try
        {
            var query = new GetEmployeeEndorsementsReceivedQuery
            {
                EmployeeId = employeeId,
                FilterByType = filterByType,
                StartDate = startDate,
                EndDate = endDate,
                Page = page,
                PageSize = Math.Min(pageSize, 50) // Limitar para performance
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogEmployeeEndorsementsReceivedError(_logger, employeeId, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Verificar se usuário já endossou conteúdo específico
    /// </summary>
    [HttpGet("check")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    [ProducesResponseType(typeof(EndorsementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EndorsementDto?>> CheckUserEndorsement(
        [FromQuery] Guid? postId = null,
        [FromQuery] Guid? commentId = null,
        [FromQuery] SynQcore.Domain.Entities.Communication.EndorsementType? specificType = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized("Usuário não autenticado.");
        }

        try
        {
            var query = new CheckUserEndorsementQuery
            {
                UserId = userId,
                PostId = postId,
                CommentId = commentId,
                SpecificType = specificType
            };
            var result = await _mediator.Send(query);
            
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NoContent(); // Usuário não endossou
            }
        }
        catch (Exception ex)
        {
            LogCheckEndorsementError(_logger, userId, ex);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Dashboard executivo - métricas consolidadas de endorsements
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "HR,Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> GetExecutiveDashboard(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? departmentId = null)
    {
        LogProcessingAnalyticsRequest(_logger, null);

        try
        {
            // Definir período padrão se não especificado (últimos 30 dias)
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            // Buscar múltiplos analytics em paralelo
            var rankingTask = _mediator.Send(new GetEmployeeEndorsementRankingQuery
            {
                TopCount = 10,
                DepartmentId = departmentId,
                StartDate = start,
                EndDate = end,
                RankingType = "Engagement"
            });

            var trendingTask = _mediator.Send(new GetTrendingEndorsementTypesQuery
            {
                StartDate = start,
                EndDate = end,
                DepartmentId = departmentId,
                TopCount = 8
            });

            await Task.WhenAll(rankingTask, trendingTask);

            var dashboard = new
            {
                Period = new { StartDate = start, EndDate = end },
                TopEmployees = rankingTask.Result,
                TrendingTypes = trendingTask.Result,
                Summary = new
                {
                    TotalEmployeesRanked = rankingTask.Result.Count,
                    TopEndorsementType = trendingTask.Result.FirstOrDefault()?.TypeDisplayName ?? "N/A",
                    TopEngagementScore = rankingTask.Result.FirstOrDefault()?.EngagementScore ?? 0
                }
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            LogDashboardError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao gerar dashboard.");
        }
    }
}