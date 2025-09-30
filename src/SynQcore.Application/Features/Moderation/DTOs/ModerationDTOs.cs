using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Moderation.DTOs;

/// <summary>
/// DTO para exibição de log de auditoria no dashboard de moderação
/// </summary>
public class ModerationAuditLogDto
{
    /// <summary>
    /// ID único do log
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do usuário que executou a ação
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Papel/função do usuário
    /// </summary>
    public string? UserRole { get; set; }

    /// <summary>
    /// Tipo de ação executada
    /// </summary>
    public AuditActionType ActionType { get; set; }

    /// <summary>
    /// Descrição amigável da ação
    /// </summary>
    public string ActionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Recurso/entidade afetada
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// ID do recurso afetado
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// Detalhes da ação
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Sucesso da operação
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de erro se aplicável
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// IP do cliente
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Nível de severidade
    /// </summary>
    public AuditSeverity Severity { get; set; }

    /// <summary>
    /// Descrição da severidade
    /// </summary>
    public string SeverityDescription { get; set; } = string.Empty;

    /// <summary>
    /// Categoria do evento
    /// </summary>
    public AuditCategory Category { get; set; }

    /// <summary>
    /// Descrição da categoria
    /// </summary>
    public string CategoryDescription { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora da criação
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indica se requer atenção
    /// </summary>
    public bool RequiresAttention { get; set; }

    /// <summary>
    /// Data de revisão se aplicável
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Usuário que revisou
    /// </summary>
    public string? ReviewedBy { get; set; }

    /// <summary>
    /// Tempo decorrido desde a criação
    /// </summary>
    public string TimeAgo { get; set; } = string.Empty;

    /// <summary>
    /// CSS class para indicar severidade visual
    /// </summary>
    public string SeverityCssClass { get; set; } = string.Empty;

    /// <summary>
    /// Ícone para o tipo de ação
    /// </summary>
    public string ActionIcon { get; set; } = string.Empty;
}

/// <summary>
/// DTO para estatísticas do dashboard de moderação
/// </summary>
public class ModerationDashboardStatsDto
{
    /// <summary>
    /// Total de logs hoje
    /// </summary>
    public int TotalLogsToday { get; set; }

    /// <summary>
    /// Total de logs esta semana
    /// </summary>
    public int TotalLogsThisWeek { get; set; }

    /// <summary>
    /// Total de logs críticos não resolvidos
    /// </summary>
    public int CriticalLogsUnresolved { get; set; }

    /// <summary>
    /// Total de tentativas de login falhadas hoje
    /// </summary>
    public int FailedLoginsToday { get; set; }

    /// <summary>
    /// Total de usuários ativos hoje
    /// </summary>
    public int ActiveUsersToday { get; set; }

    /// <summary>
    /// Total de eventos de segurança
    /// </summary>
    public int SecurityEventsToday { get; set; }

    /// <summary>
    /// Taxa de crescimento de logs (%)
    /// </summary>
    public decimal LogGrowthRate { get; set; }

    /// <summary>
    /// Logs por severidade
    /// </summary>
    public Dictionary<string, int> LogsBySeverity { get; set; } = new();

    /// <summary>
    /// Logs por categoria
    /// </summary>
    public Dictionary<string, int> LogsByCategory { get; set; } = new();

    /// <summary>
    /// Top 5 IPs com mais atividade
    /// </summary>
    public List<IpActivityDto> TopActiveIps { get; set; } = new();

    /// <summary>
    /// Últimas ações críticas
    /// </summary>
    public List<ModerationAuditLogDto> RecentCriticalActions { get; set; } = new();
}

/// <summary>
/// DTO para atividade por IP
/// </summary>
public class IpActivityDto
{
    /// <summary>
    /// Endereço IP
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade de requisições
    /// </summary>
    public int RequestCount { get; set; }

    /// <summary>
    /// Quantidade de falhas
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Taxa de falha (%)
    /// </summary>
    public decimal FailureRate { get; set; }

    /// <summary>
    /// Última atividade
    /// </summary>
    public DateTime LastActivity { get; set; }

    /// <summary>
    /// Indica se IP é suspeito
    /// </summary>
    public bool IsSuspicious { get; set; }
}

/// <summary>
/// DTO para filtros de busca de logs de auditoria
/// </summary>
public class ModerationAuditFilterDto
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
}
