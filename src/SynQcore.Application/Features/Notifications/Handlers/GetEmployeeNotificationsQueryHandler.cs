using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Queries;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para obter notificações do funcionário
/// </summary>
public partial class GetEmployeeNotificationsQueryHandler : IRequestHandler<GetEmployeeNotificationsQuery, GetEmployeeNotificationsResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetEmployeeNotificationsQueryHandler> _logger;

    public GetEmployeeNotificationsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetEmployeeNotificationsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetEmployeeNotificationsResponse> Handle(GetEmployeeNotificationsQuery request, CancellationToken cancellationToken)
    {
        LogGettingEmployeeNotifications(_logger, request.EmployeeId.ToString(), request.Page);

        try
        {
            // Query base: notificações direcionadas ao funcionário
            var baseQuery = from n in _context.CorporateNotifications
                           join d in _context.NotificationDeliveries on n.Id equals d.NotificationId
                           where d.EmployeeId == request.EmployeeId && n.Status == Domain.Entities.NotificationStatus.Sent
                           select new { Notification = n, Delivery = d };

            // Aplicar filtros opcionais
            if (request.IsRead.HasValue)
            {
                if (request.IsRead.Value)
                {
                    baseQuery = baseQuery.Where(x => x.Delivery.ReadAt != null);
                }
                else
                {
                    baseQuery = baseQuery.Where(x => x.Delivery.ReadAt == null);
                }
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                if (Enum.TryParse<Domain.Entities.NotificationType>(request.Type, out var notificationType))
                {
                    baseQuery = baseQuery.Where(x => x.Notification.Type == notificationType);
                }
            }

            if (!string.IsNullOrEmpty(request.Priority))
            {
                if (Enum.TryParse<Domain.Entities.NotificationPriority>(request.Priority, out var priority))
                {
                    baseQuery = baseQuery.Where(x => x.Notification.Priority == priority);
                }
            }

            if (request.DateFrom.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Notification.CreatedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Notification.CreatedAt <= request.DateTo.Value);
            }

            // Contar total
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // Aplicar paginação e ordenação
            var notifications = await baseQuery
                .OrderByDescending(x => x.Notification.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => x.Notification)
                .Include(n => n.CreatedByEmployee)
                .Include(n => n.TargetDepartment)
                .ToListAsync(cancellationToken);

            // Calcular contadores por status
            var counts = await CalculateNotificationCounts(request.EmployeeId, cancellationToken);

            var response = new GetEmployeeNotificationsResponse
            {
                Notifications = notifications.ToCorporateNotificationDtos(),
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Counts = counts
            };

            LogEmployeeNotificationsRetrieved(_logger, request.EmployeeId.ToString(), response.TotalCount, response.TotalPages);

            return response;
        }
        catch (Exception ex)
        {
            LogEmployeeNotificationsError(_logger, ex, request.EmployeeId.ToString());
            
            return new GetEmployeeNotificationsResponse
            {
                Notifications = new List<SynQcore.Application.DTOs.Notifications.CorporateNotificationDto>(),
                TotalCount = 0,
                CurrentPage = request.Page,
                TotalPages = 0,
                Counts = new SynQcore.Application.DTOs.Notifications.NotificationCountsDto()
            };
        }
    }

    /// <summary>
    /// Calcula contadores de notificações por status para o funcionário
    /// </summary>
    private async Task<SynQcore.Application.DTOs.Notifications.NotificationCountsDto> CalculateNotificationCounts(Guid employeeId, CancellationToken cancellationToken)
    {
        var deliveries = await _context.NotificationDeliveries
            .Where(d => d.EmployeeId == employeeId)
            .Join(_context.CorporateNotifications,
                  d => d.NotificationId,
                  n => n.Id,
                  (d, n) => new { d.ReadAt, d.AcknowledgedAt, n.ExpiresAt, n.Status })
            .Where(x => x.Status == Domain.Entities.NotificationStatus.Sent)
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        return new SynQcore.Application.DTOs.Notifications.NotificationCountsDto
        {
            Total = deliveries.Count,
            Unread = deliveries.Count(d => d.ReadAt == null),
            Read = deliveries.Count(d => d.ReadAt != null && d.AcknowledgedAt == null),
            Acknowledged = deliveries.Count(d => d.AcknowledgedAt != null),
            Expired = deliveries.Count(d => d.ExpiresAt.HasValue && d.ExpiresAt.Value <= now)
        };
    }

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 4401, Level = LogLevel.Information,
        Message = "Obtendo notificações do funcionário - EmployeeId: {EmployeeId} - Página: {Page}")]
    private static partial void LogGettingEmployeeNotifications(ILogger logger, string employeeId, int page);

    [LoggerMessage(EventId = 4402, Level = LogLevel.Information,
        Message = "Notificações do funcionário recuperadas - EmployeeId: {EmployeeId} - Total: {TotalCount} - Páginas: {TotalPages}")]
    private static partial void LogEmployeeNotificationsRetrieved(ILogger logger, string employeeId, int totalCount, int totalPages);

    [LoggerMessage(EventId = 4403, Level = LogLevel.Error,
        Message = "Erro ao obter notificações do funcionário - EmployeeId: {EmployeeId}")]
    private static partial void LogEmployeeNotificationsError(ILogger logger, Exception exception, string employeeId);

    #endregion
}