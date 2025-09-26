using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para marcação de notificação como lida
/// </summary>
public partial class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<MarkNotificationAsReadCommandHandler> _logger;

    public MarkNotificationAsReadCommandHandler(
        ISynQcoreDbContext context,
        ILogger<MarkNotificationAsReadCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MarkNotificationAsReadResponse> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        LogMarkingNotificationAsRead(_logger, request.NotificationId, request.EmployeeId);

        // Buscar a entrega da notificação para o funcionário específico
        var delivery = await _context.NotificationDeliveries
            .FirstOrDefaultAsync(d => 
                d.NotificationId == request.NotificationId && 
                d.EmployeeId == request.EmployeeId, 
                cancellationToken);

        if (delivery == null)
        {
            LogNotificationDeliveryNotFound(_logger, request.NotificationId, request.EmployeeId);
            return new MarkNotificationAsReadResponse 
            { 
                Success = false, 
                Message = "Entrega de notificação não encontrada para este funcionário" 
            };
        }

        // Verificar se já está marcada como lida
        if (delivery.Status == DeliveryStatus.Read || delivery.Status == DeliveryStatus.Acknowledged)
        {
            LogNotificationAlreadyRead(_logger, request.NotificationId, request.EmployeeId);
            return new MarkNotificationAsReadResponse 
            { 
                Success = true, 
                Message = "Notificação já estava marcada como lida" 
            };
        }

        // Atualizar status para lida
        delivery.Status = DeliveryStatus.Read;
        delivery.ReadAt = DateTimeOffset.UtcNow;
        delivery.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        LogNotificationMarkedAsRead(_logger, request.NotificationId, request.EmployeeId);

        return new MarkNotificationAsReadResponse 
        { 
            Success = true, 
            Message = "Notificação marcada como lida" 
        };
    }

    [LoggerMessage(EventId = 5016, Level = LogLevel.Information,
        Message = "Marcando notificação {NotificationId} como lida para funcionário {EmployeeId}")]
    private static partial void LogMarkingNotificationAsRead(ILogger logger, Guid notificationId, Guid employeeId);

    [LoggerMessage(EventId = 5017, Level = LogLevel.Warning,
        Message = "Entrega de notificação {NotificationId} não encontrada para funcionário {EmployeeId}")]
    private static partial void LogNotificationDeliveryNotFound(ILogger logger, Guid notificationId, Guid employeeId);

    [LoggerMessage(EventId = 5018, Level = LogLevel.Information,
        Message = "Notificação {NotificationId} já estava lida para funcionário {EmployeeId}")]
    private static partial void LogNotificationAlreadyRead(ILogger logger, Guid notificationId, Guid employeeId);

    [LoggerMessage(EventId = 5019, Level = LogLevel.Information,
        Message = "Notificação {NotificationId} marcada como lida para funcionário {EmployeeId}")]
    private static partial void LogNotificationMarkedAsRead(ILogger logger, Guid notificationId, Guid employeeId);
}