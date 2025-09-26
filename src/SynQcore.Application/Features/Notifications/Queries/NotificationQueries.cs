using MediatR;
using SynQcore.Application.DTOs.Notifications;

namespace SynQcore.Application.Features.Notifications.Queries;

/// <summary>
/// Query para obter notificações do funcionário
/// </summary>
public record GetEmployeeNotificationsQuery : IRequest<GetEmployeeNotificationsResponse>
{
    /// <summary>
    /// ID do funcionário
    /// </summary>
    public Guid EmployeeId { get; init; }

    /// <summary>
    /// Página para paginação
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Itens por página
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Filtro por status de leitura
    /// </summary>
    public bool? IsRead { get; init; }

    /// <summary>
    /// Filtro por tipo de notificação
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Filtro por prioridade
    /// </summary>
    public string? Priority { get; init; }

    /// <summary>
    /// Data de início para filtro
    /// </summary>
    public DateTimeOffset? DateFrom { get; init; }

    /// <summary>
    /// Data de fim para filtro
    /// </summary>
    public DateTimeOffset? DateTo { get; init; }
}

/// <summary>
/// Response para notificações do funcionário
/// </summary>
public class GetEmployeeNotificationsResponse
{
    /// <summary>
    /// Lista de notificações
    /// </summary>
    public List<CorporateNotificationDto> Notifications { get; set; } = new();

    /// <summary>
    /// Total de notificações
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Contadores por status
    /// </summary>
    public NotificationCountsDto Counts { get; set; } = new();
}

/// <summary>
/// Query para obter notificações corporativas (Admin/Manager)
/// </summary>
public record GetCorporateNotificationsQuery : IRequest<GetCorporateNotificationsResponse>
{
    /// <summary>
    /// Página para paginação
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Itens por página
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Filtro por status
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Filtro por tipo
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Filtro por prioridade
    /// </summary>
    public string? Priority { get; init; }

    /// <summary>
    /// Filtro por criador
    /// </summary>
    public Guid? CreatedBy { get; init; }

    /// <summary>
    /// Filtro por departamento alvo
    /// </summary>
    public Guid? TargetDepartmentId { get; init; }

    /// <summary>
    /// Data de início
    /// </summary>
    public DateTimeOffset? DateFrom { get; init; }

    /// <summary>
    /// Data de fim
    /// </summary>
    public DateTimeOffset? DateTo { get; init; }

    /// <summary>
    /// Busca por texto (título/conteúdo)
    /// </summary>
    public string? SearchText { get; init; }
}

/// <summary>
/// Response para notificações corporativas
/// </summary>
public class GetCorporateNotificationsResponse
{
    /// <summary>
    /// Lista de notificações
    /// </summary>
    public List<CorporateNotificationDto> Notifications { get; set; } = new();

    /// <summary>
    /// Total de notificações
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Contadores por status
    /// </summary>
    public Dictionary<string, int> StatusCounts { get; set; } = new();
}

/// <summary>
/// Query para obter detalhes de notificação específica
/// </summary>
public record GetNotificationDetailsQuery : IRequest<GetNotificationDetailsResponse>
{
    /// <summary>
    /// ID da notificação
    /// </summary>
    public Guid NotificationId { get; init; }

    /// <summary>
    /// Incluir estatísticas de entrega
    /// </summary>
    public bool IncludeStats { get; init; } = true;

    /// <summary>
    /// Incluir entregas detalhadas
    /// </summary>
    public bool IncludeDeliveries { get; init; }
}

/// <summary>
/// Response para detalhes de notificação
/// </summary>
public class GetNotificationDetailsResponse
{
    /// <summary>
    /// Notificação
    /// </summary>
    public CorporateNotificationDto Notification { get; set; } = null!;

    /// <summary>
    /// Entregas (se solicitado)
    /// </summary>
    public List<NotificationDeliveryDto>? Deliveries { get; set; }
}

/// <summary>
/// Query para obter estatísticas de notificações
/// </summary>
public record GetNotificationStatsQuery : IRequest<GetNotificationStatsResponse>
{
    /// <summary>
    /// ID da notificação específica (opcional)
    /// </summary>
    public Guid? NotificationId { get; init; }

    /// <summary>
    /// Data de início para estatísticas
    /// </summary>
    public DateTimeOffset? DateFrom { get; init; }

    /// <summary>
    /// Data de fim para estatísticas
    /// </summary>
    public DateTimeOffset? DateTo { get; init; }

    /// <summary>
    /// Filtro por departamento
    /// </summary>
    public Guid? DepartmentId { get; init; }

    /// <summary>
    /// Filtro por tipo de notificação
    /// </summary>
    public string? Type { get; init; }
}

/// <summary>
/// Response para estatísticas de notificações
/// </summary>
public class GetNotificationStatsResponse
{
    /// <summary>
    /// Estatísticas gerais
    /// </summary>
    public NotificationOverviewStatsDto OverviewStats { get; set; } = new();

    /// <summary>
    /// Estatísticas por tipo
    /// </summary>
    public List<TypeStatsDto> TypeStats { get; set; } = new();

    /// <summary>
    /// Estatísticas por canal
    /// </summary>
    public List<ChannelStatsDto> ChannelStats { get; set; } = new();

    /// <summary>
    /// Estatísticas por departamento
    /// </summary>
    public List<DepartmentStatsDto> DepartmentStats { get; set; } = new();

    /// <summary>
    /// Tendências por período
    /// </summary>
    public List<PeriodStatsDto> TrendStats { get; set; } = new();
}

/// <summary>
/// Query para obter templates disponíveis
/// </summary>
public record GetNotificationTemplatesQuery : IRequest<GetNotificationTemplatesResponse>
{
    /// <summary>
    /// Filtro por categoria
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Apenas templates ativos
    /// </summary>
    public bool ActiveOnly { get; init; } = true;

    /// <summary>
    /// Filtro por tipo
    /// </summary>
    public string? Type { get; init; }
}

/// <summary>
/// Response para templates de notificação
/// </summary>
public class GetNotificationTemplatesResponse
{
    /// <summary>
    /// Lista de templates
    /// </summary>
    public List<NotificationTemplateDto> Templates { get; set; } = new();

    /// <summary>
    /// Categorias disponíveis
    /// </summary>
    public List<string> AvailableCategories { get; set; } = new();
}

/// <summary>
/// Query para obter notificações pendentes de aprovação
/// </summary>
public record GetPendingApprovalsQuery : IRequest<GetPendingApprovalsResponse>
{
    /// <summary>
    /// ID do aprovador (usuário atual)
    /// </summary>
    public Guid ApproverId { get; init; }

    /// <summary>
    /// Página para paginação
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Itens por página
    /// </summary>
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Response para aprovações pendentes
/// </summary>
public class GetPendingApprovalsResponse
{
    /// <summary>
    /// Notificações pendentes de aprovação
    /// </summary>
    public List<CorporateNotificationDto> PendingNotifications { get; set; } = new();

    /// <summary>
    /// Total pendente
    /// </summary>
    public int TotalPending { get; set; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; set; }
}

#region DTOs Auxiliares

/// <summary>
/// Estatísticas gerais de notificações
/// </summary>
public class NotificationOverviewStatsDto
{
    /// <summary>
    /// Total de notificações no período
    /// </summary>
    public int TotalNotifications { get; set; }

    /// <summary>
    /// Total de entregas
    /// </summary>
    public int TotalDeliveries { get; set; }

    /// <summary>
    /// Taxa média de entrega
    /// </summary>
    public decimal AverageDeliveryRate { get; set; }

    /// <summary>
    /// Taxa média de leitura
    /// </summary>
    public decimal AverageReadRate { get; set; }

    /// <summary>
    /// Taxa média de confirmação
    /// </summary>
    public decimal AverageAcknowledgmentRate { get; set; }

    /// <summary>
    /// Tempo médio para leitura (em horas)
    /// </summary>
    public double AverageTimeToRead { get; set; }

    /// <summary>
    /// Canal mais efetivo
    /// </summary>
    public string? MostEffectiveChannel { get; set; }
}

/// <summary>
/// Estatísticas por tipo de notificação
/// </summary>
public class TypeStatsDto
{
    /// <summary>
    /// Tipo de notificação
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Total enviado
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Taxa de entrega
    /// </summary>
    public decimal DeliveryRate { get; set; }

    /// <summary>
    /// Taxa de leitura
    /// </summary>
    public decimal ReadRate { get; set; }
}

/// <summary>
/// Estatísticas por departamento
/// </summary>
public class DepartmentStatsDto
{
    /// <summary>
    /// Nome do departamento
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Total de notificações recebidas
    /// </summary>
    public int TotalReceived { get; set; }

    /// <summary>
    /// Taxa de leitura do departamento
    /// </summary>
    public decimal ReadRate { get; set; }

    /// <summary>
    /// Taxa de confirmação do departamento
    /// </summary>
    public decimal AcknowledgmentRate { get; set; }
}

/// <summary>
/// Estatísticas por período
/// </summary>
public class PeriodStatsDto
{
    /// <summary>
    /// Data do período
    /// </summary>
    public DateOnly Period { get; set; }

    /// <summary>
    /// Total de notificações enviadas
    /// </summary>
    public int NotificationsSent { get; set; }

    /// <summary>
    /// Total de entregas
    /// </summary>
    public int DeliveriesCount { get; set; }

    /// <summary>
    /// Taxa de sucesso do período
    /// </summary>
    public decimal SuccessRate { get; set; }
}

#endregion