using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Moderation.Queries;

/// <summary>
/// Query para buscar logs de auditoria com filtros para moderação
/// </summary>
public class GetModerationAuditLogsQuery : IRequest<PagedResult<ModerationAuditLogDto>>
{
    /// <summary>
    /// Data de início
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Data de fim
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Tipo de ação
    /// </summary>
    public AuditActionType? ActionType { get; set; }

    /// <summary>
    /// Severidade
    /// </summary>
    public AuditSeverity? Severity { get; set; }

    /// <summary>
    /// Categoria
    /// </summary>
    public AuditCategory? Category { get; set; }

    /// <summary>
    /// ID do usuário
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Endereço IP
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Apenas logs que requerem atenção
    /// </summary>
    public bool OnlyRequiringAttention { get; set; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public GetModerationAuditLogsQuery() { }

    /// <summary>
    /// Construtor a partir de filtros
    /// </summary>
    public static GetModerationAuditLogsQuery FromFilters(ModerationAuditFilterDto filters)
    {
        return new GetModerationAuditLogsQuery
        {
            StartDate = filters.StartDate,
            EndDate = filters.EndDate,
            ActionType = filters.ActionType,
            Severity = filters.Severity,
            Category = filters.Category,
            UserId = filters.UserId,
            IpAddress = filters.IpAddress,
            OnlyRequiringAttention = filters.OnlyRequiringAttention,
            Page = filters.Page,
            PageSize = filters.PageSize
        };
    }
}

/// <summary>
/// Query para obter estatísticas do dashboard de moderação
/// </summary>
public class GetModerationDashboardStatsQuery : IRequest<ModerationDashboardStatsDto>
{
    /// <summary>
    /// Data de referência para cálculos (padrão: agora)
    /// </summary>
    public DateTime ReferenceDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Query para obter logs de auditoria que requerem atenção
/// </summary>
public class GetLogsRequiringAttentionQuery : IRequest<PagedResult<ModerationAuditLogDto>>
{
    /// <summary>
    /// Página atual
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Severidade mínima
    /// </summary>
    public AuditSeverity MinimumSeverity { get; set; } = AuditSeverity.Warning;
}

/// <summary>
/// Query para obter eventos de segurança recentes
/// </summary>
public class GetRecentSecurityEventsQuery : IRequest<List<ModerationAuditLogDto>>
{
    /// <summary>
    /// Horas atrás para buscar eventos
    /// </summary>
    public int HoursBack { get; set; } = 24;

    /// <summary>
    /// Limite de resultados
    /// </summary>
    public int Limit { get; set; } = 50;
}

/// <summary>
/// Query para obter atividade por IP
/// </summary>
public class GetIpActivityQuery : IRequest<List<IpActivityDto>>
{
    /// <summary>
    /// Data de início
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-1);

    /// <summary>
    /// Data de fim
    /// </summary>
    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Limite de resultados
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Incluir apenas IPs suspeitos
    /// </summary>
    public bool OnlySuspicious { get; set; }
}

/// <summary>
/// Query para exportar logs de auditoria
/// </summary>
public class ExportAuditLogsQuery : IRequest<byte[]>
{
    /// <summary>
    /// Data de início
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Data de fim
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Formato de exportação (CSV, Excel, JSON)
    /// </summary>
    public string Format { get; set; } = "CSV";

    /// <summary>
    /// Filtros adicionais
    /// </summary>
    public ModerationAuditFilterDto? Filters { get; set; }
}

/// <summary>
/// Query para obter estatísticas de usuário específico
/// </summary>
public class GetUserAuditStatsQuery : IRequest<UserAuditStatsDto>
{
    /// <summary>
    /// ID do usuário
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Período em dias
    /// </summary>
    public int PeriodDays { get; set; } = 30;
}

/// <summary>
/// DTO para estatísticas de usuário
/// </summary>
public class UserAuditStatsDto
{
    /// <summary>
    /// ID do usuário
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Total de ações
    /// </summary>
    public int TotalActions { get; set; }

    /// <summary>
    /// Ações bem-sucedidas
    /// </summary>
    public int SuccessfulActions { get; set; }

    /// <summary>
    /// Ações com falha
    /// </summary>
    public int FailedActions { get; set; }

    /// <summary>
    /// Taxa de sucesso
    /// </summary>
    public decimal SuccessRate { get; set; }

    /// <summary>
    /// Última atividade
    /// </summary>
    public DateTime? LastActivity { get; set; }

    /// <summary>
    /// IPs únicos utilizados
    /// </summary>
    public int UniqueIpsUsed { get; set; }

    /// <summary>
    /// Ações por tipo
    /// </summary>
    public Dictionary<string, int> ActionsByType { get; set; } = new();

    /// <summary>
    /// Eventos de segurança
    /// </summary>
    public int SecurityEvents { get; set; }

    /// <summary>
    /// Score de risco (0-100)
    /// </summary>
    public int RiskScore { get; set; }
}
