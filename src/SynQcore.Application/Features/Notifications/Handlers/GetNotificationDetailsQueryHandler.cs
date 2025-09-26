using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.DTOs.Notifications;
using SynQcore.Application.Features.Notifications.Queries;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para obter detalhes de uma notificação específica
/// </summary>
public partial class GetNotificationDetailsQueryHandler : IRequestHandler<GetNotificationDetailsQuery, GetNotificationDetailsResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetNotificationDetailsQueryHandler> _logger;

    public GetNotificationDetailsQueryHandler(
        ISynQcoreDbContext context,
        ILogger<GetNotificationDetailsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetNotificationDetailsResponse> Handle(GetNotificationDetailsQuery request, CancellationToken cancellationToken)
    {
        LogGettingNotificationDetails(_logger, request.NotificationId);

        var notification = await _context.CorporateNotifications
            .Include(n => n.CreatedByEmployee)
            .Include(n => n.TargetDepartment)
            .Include(n => n.ApprovedByEmployee)
            .Include(n => n.Deliveries)
                .ThenInclude(d => d.Employee)
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (notification == null)
        {
            LogNotificationNotFound(_logger, request.NotificationId);
            return new GetNotificationDetailsResponse
            {
                Notification = new CorporateNotificationDto()
            };
        }

        LogNotificationDetailsFound(_logger, request.NotificationId);

        var response = new GetNotificationDetailsResponse
        {
            Notification = notification.ToCorporateNotificationDto()
        };

        // Incluir entregas se solicitado
        if (request.IncludeDeliveries)
        {
            response.Deliveries = notification.Deliveries.ToNotificationDeliveryDtos();
        }

        return response;
    }

    [LoggerMessage(EventId = 5013, Level = LogLevel.Information,
        Message = "Obtendo detalhes da notificação: {NotificationId}")]
    private static partial void LogGettingNotificationDetails(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5014, Level = LogLevel.Warning,
        Message = "Notificação não encontrada: {NotificationId}")]
    private static partial void LogNotificationNotFound(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5015, Level = LogLevel.Information,
        Message = "Detalhes da notificação {NotificationId} obtidos com sucesso")]
    private static partial void LogNotificationDetailsFound(ILogger logger, Guid notificationId);
}