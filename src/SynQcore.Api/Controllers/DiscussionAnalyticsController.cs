using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.Handlers.Communication.DiscussionThreads;
using SynQcore.Application.Queries.Communication.DiscussionThreads;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para analytics de Discussion Threads corporativas
/// </summary>
[ApiController]
[Route("api/discussion-threads/analytics")]
[Authorize]
public partial class DiscussionAnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DiscussionAnalyticsController> _logger;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public DiscussionAnalyticsController(IMediator mediator, ILogger<DiscussionAnalyticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém analytics detalhado de discussions
    /// </summary>
    /// <param name="postId">ID do post para filtrar</param>
    /// <param name="fromDate">Data inicial do período</param>
    /// <param name="toDate">Data final do período</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <returns>Dados analíticos das discussões</returns>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(DiscussionAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DiscussionAnalyticsDto>> GetDiscussionAnalytics(
        [FromQuery] Guid? postId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] Guid? departmentId = null)
    {
        try
        {
            var query = new GetDiscussionAnalyticsQuery(postId, fromDate, toDate, departmentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingAnalytics(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém analytics de participação de usuário específico
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="fromDate">Data inicial do período</param>
    /// <param name="toDate">Data final do período</param>
    /// <returns>Dados analíticos de participação do usuário</returns>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(UserDiscussionAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDiscussionAnalyticsDto>> GetUserAnalytics(
        Guid userId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetUserDiscussionAnalyticsQuery(userId, fromDate, toDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            LogUserNotFound(_logger, ex, userId);
            return NotFound(new { message = "Usuário não encontrado" });
        }
        catch (Exception ex)
        {
            LogErrorGettingUserAnalytics(_logger, ex, userId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém métricas de moderação (Manager/HR/Admin)
    /// </summary>
    /// <param name="fromDate">Data inicial do período</param>
    /// <param name="toDate">Data final do período</param>
    /// <param name="moderatorId">ID do moderador para filtrar</param>
    /// <returns>Métricas de moderação de discussões</returns>
    [HttpGet("moderation")]
    [Authorize(Roles = "Manager,HR,Admin")]
    [ProducesResponseType(typeof(ModerationMetricsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ModerationMetricsDto>> GetModerationMetrics(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] Guid? moderatorId = null)
    {
        try
        {
            var query = new GetModerationMetricsQuery(fromDate, toDate, moderatorId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingModerationMetrics(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém estatísticas de engagement por período
    /// </summary>
    /// <param name="fromDate">Data inicial do período</param>
    /// <param name="toDate">Data final do período</param>
    /// <param name="groupBy">Agrupamento por período (Day, Week, Month)</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <param name="teamId">ID do time para filtrar</param>
    /// <returns>Estatísticas de engagement das discussões</returns>
    [HttpGet("engagement")]
    [ProducesResponseType(typeof(EngagementStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EngagementStatisticsDto>> GetEngagementStatistics(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string groupBy = "Day",
        [FromQuery] Guid? departmentId = null,
        [FromQuery] Guid? teamId = null)
    {
        try
        {
            var query = new GetEngagementStatisticsQuery(fromDate, toDate, groupBy, departmentId, teamId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingEngagementStats(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém discussões em trending (alta atividade recente)
    /// </summary>
    /// <param name="hours">Janela de horas para análise</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="department">Departamento para filtrar</param>
    /// <param name="category">Categoria para filtrar</param>
    /// <returns>Lista paginada de discussões em trending</returns>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(PagedTrendingDiscussionsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedTrendingDiscussionsResponse>> GetTrendingDiscussions(
        [FromQuery] int hours = 24,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? department = null,
        [FromQuery] string? category = null)
    {
        try
        {
            var query = new GetTrendingDiscussionsQuery(hours, page, pageSize, department, category);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingTrending(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém comentários que precisam de atenção (questões não resolvidas, alta prioridade)
    /// </summary>
    /// <param name="userId">ID do usuário para filtrar</param>
    /// <param name="unresolvedOnly">Se deve retornar apenas não resolvidos</param>
    /// <param name="highPriorityOnly">Se deve retornar apenas alta prioridade</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de comentários que precisam de atenção</returns>
    [HttpGet("attention-needed")]
    [ProducesResponseType(typeof(PagedCommentsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedCommentsResponse>> GetCommentsNeedingAttention(
        [FromQuery] Guid? userId = null,
        [FromQuery] bool unresolvedOnly = true,
        [FromQuery] bool highPriorityOnly = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetCommentsNeedingAttentionQuery(userId, unresolvedOnly, highPriorityOnly, page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogErrorGettingAttentionNeeded(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Gera relatório executivo de discussions (HR/Admin)
    /// </summary>
    /// <param name="fromDate">Data inicial do relatório</param>
    /// <param name="toDate">Data final do relatório</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <returns>Relatório executivo consolidado</returns>
    [HttpGet("executive-report")]
    [Authorize(Roles = "HR,Admin")]
    [ProducesResponseType(typeof(ExecutiveDiscussionReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExecutiveDiscussionReportDto>> GetExecutiveReport(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] Guid? departmentId = null)
    {
        try
        {
            // Combina múltiplas queries para relatório executivo
            var discussionAnalytics = await _mediator.Send(new GetDiscussionAnalyticsQuery(null, fromDate, toDate, departmentId));
            var moderationMetrics = await _mediator.Send(new GetModerationMetricsQuery(fromDate, toDate, null));
            var engagementStats = await _mediator.Send(new GetEngagementStatisticsQuery(fromDate, toDate, "Week", departmentId, null));

            var report = new ExecutiveDiscussionReportDto
            {
                ReportPeriod = new { FromDate = fromDate, ToDate = toDate },
                DepartmentId = departmentId,
                GeneratedAt = DateTime.UtcNow,

                TotalDiscussions = discussionAnalytics.TotalThreads,
                TotalComments = discussionAnalytics.TotalComments,
                UniqueParticipants = engagementStats.UniqueParticipants,
                EngagementScore = engagementStats.TimeSeries.LastOrDefault()?.EngagementScore ?? 0,

                ModerationHealth = new ModerationHealthDto
                {
                    TotalModerated = moderationMetrics.TotalCommentsModerated,
                    AverageResponseTime = moderationMetrics.AverageResponseTime,
                    ApprovalRate = moderationMetrics.ApprovalRate,
                    PendingCount = discussionAnalytics.PendingModeration
                },

                TopContributors = discussionAnalytics.TopContributors.Take(5).ToList(),
                MostActiveThreads = discussionAnalytics.MostActiveThreads.Take(5).ToList(),

                EngagementTrend = engagementStats.TimeSeries,
                ModerationTrends = moderationMetrics.Trends
            };

            return Ok(report);
        }
        catch (Exception ex)
        {
            LogErrorGeneratingExecutiveReport(_logger, ex);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exporta dados de analytics para CSV (HR/Admin)
    /// </summary>
    /// <param name="reportType">Tipo de relatório (discussions, users, moderation)</param>
    /// <param name="fromDate">Data inicial</param>
    /// <param name="toDate">Data final</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <returns>Arquivo CSV com dados analíticos</returns>
    [HttpGet("export/{reportType}")]
    [Authorize(Roles = "HR,Admin")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public Task<IActionResult> ExportAnalyticsData(
        string reportType,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] Guid? departmentId = null)
    {
        try
        {
            var fileName = $"discussion_analytics_{reportType}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv";
            var csvContent = "Implementação de export CSV seria feita aqui";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

            return Task.FromResult<IActionResult>(File(bytes, "text/csv", fileName));
        }
        catch (Exception ex)
        {
            LogErrorExportingData(_logger, ex, reportType);
            return Task.FromResult<IActionResult>(StatusCode(500, new { message = "Erro interno do servidor" }));
        }
    }

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 4001, Level = LogLevel.Error,
        Message = "Erro ao obter analytics de discussions")]
    private static partial void LogErrorGettingAnalytics(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Warning,
        Message = "Usuário não encontrado: {UserId}")]
    private static partial void LogUserNotFound(ILogger logger, Exception ex, Guid userId);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Error,
        Message = "Erro ao obter analytics do usuário {UserId}")]
    private static partial void LogErrorGettingUserAnalytics(ILogger logger, Exception ex, Guid userId);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Error,
        Message = "Erro ao obter métricas de moderação")]
    private static partial void LogErrorGettingModerationMetrics(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Error,
        Message = "Erro ao obter estatísticas de engagement")]
    private static partial void LogErrorGettingEngagementStats(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Error,
        Message = "Erro ao obter discussões em trending")]
    private static partial void LogErrorGettingTrending(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Error,
        Message = "Erro ao obter comentários que precisam de atenção")]
    private static partial void LogErrorGettingAttentionNeeded(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Error,
        Message = "Erro ao gerar relatório executivo")]
    private static partial void LogErrorGeneratingExecutiveReport(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Error,
        Message = "Erro ao exportar dados de analytics: {ReportType}")]
    private static partial void LogErrorExportingData(ILogger logger, Exception ex, string reportType);

    #endregion
}

// DTO para relatório executivo consolidado
/// <summary>
/// Classe para operações do sistema
/// </summary>
public class ExecutiveDiscussionReportDto
{
    /// <summary>Propriedade do sistema</summary>
    public object ReportPeriod { get; set; } = new();
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public Guid? DepartmentId { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public int TotalDiscussions { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public int TotalComments { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public int UniqueParticipants { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public double EngagementScore { get; set; }

    /// <summary>Propriedade do sistema</summary>
    public ModerationHealthDto ModerationHealth { get; set; } = new();

    /// <summary>Propriedade do sistema</summary>
    public List<TopContributor> TopContributors { get; set; } = [];
    /// <summary>Propriedade do sistema</summary>
    public List<ActiveThread> MostActiveThreads { get; set; } = [];

    /// <summary>Propriedade do sistema</summary>
    public List<EngagementDataPoint> EngagementTrend { get; set; } = [];
    /// <summary>Propriedade do sistema</summary>
    public List<ModerationTrendItem> ModerationTrends { get; set; } = [];
}

// DTO para saúde da moderação
/// <summary>
/// Classe para operações do sistema
/// </summary>
public class ModerationHealthDto
{
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public int TotalModerated { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public int AverageResponseTime { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public double ApprovalRate { get; set; }
    /// <summary>
    /// Propriedade da requisição
    /// </summary>
    /// <summary>Propriedade do sistema</summary>
    public int PendingCount { get; set; }
}
