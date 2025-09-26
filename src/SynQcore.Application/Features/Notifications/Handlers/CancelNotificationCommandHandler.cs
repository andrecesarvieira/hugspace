using MediatR;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para cancelamento de notificações corporativas
/// </summary>
public partial class CancelNotificationCommandHandler : IRequestHandler<CancelNotificationCommand, CancelNotificationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CancelNotificationCommandHandler> _logger;

    public CancelNotificationCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CancelNotificationCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CancelNotificationResponse> Handle(CancelNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        LogCancellingNotification(_logger, request.NotificationId, currentUserId);

        var notification = await _context.CorporateNotifications
            .FindAsync([request.NotificationId], cancellationToken);

        if (notification == null)
        {
            LogNotificationNotFound(_logger, request.NotificationId);
            return new CancelNotificationResponse 
            { 
                Success = false, 
                Message = "Notificação não encontrada" 
            };
        }

        // Verificar se é o criador da notificação (apenas criadores podem cancelar)
        var isCreator = notification.CreatedByEmployeeId == currentUserId;

        if (!isCreator)
        {
            LogUnauthorizedCancel(_logger, request.NotificationId, currentUserId);
            return new CancelNotificationResponse 
            { 
                Success = false, 
                Message = "Você não tem permissão para cancelar esta notificação" 
            };
        }

        // Verificar se pode ser cancelada
        var cancellableStatuses = new[] 
        { 
            NotificationStatus.Draft, 
            NotificationStatus.Scheduled, 
            NotificationStatus.PendingApproval,
            NotificationStatus.Approved
        };

        if (!cancellableStatuses.Contains(notification.Status))
        {
            LogInvalidStatusForCancel(_logger, request.NotificationId, notification.Status);
            return new CancelNotificationResponse 
            { 
                Success = false, 
                Message = $"Notificação não pode ser cancelada. Status atual: {notification.Status}" 
            };
        }

        // Cancelar a notificação
        notification.Status = NotificationStatus.Cancelled;
        notification.UpdatedAt = DateTime.UtcNow;

        // Adicionar motivo do cancelamento aos metadados se fornecido
        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            var cancelReason = new
            {
                CancelledBy = currentUserId,
                CancelledAt = DateTime.UtcNow,
                Reason = request.Reason
            };

            notification.Metadata = System.Text.Json.JsonSerializer.Serialize(new
            {
                OriginalMetadata = notification.Metadata,
                CancellationInfo = cancelReason
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        LogNotificationCancelled(_logger, request.NotificationId, currentUserId);

        return new CancelNotificationResponse 
        { 
            Success = true, 
            Message = "Notificação cancelada com sucesso" 
        };
    }

    [LoggerMessage(EventId = 5026, Level = LogLevel.Information,
        Message = "Cancelando notificação {NotificationId} por usuário {UserId}")]
    private static partial void LogCancellingNotification(ILogger logger, Guid notificationId, Guid userId);

    [LoggerMessage(EventId = 5027, Level = LogLevel.Warning,
        Message = "Notificação não encontrada para cancelamento: {NotificationId}")]
    private static partial void LogNotificationNotFound(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5028, Level = LogLevel.Warning,
        Message = "Usuário {UserId} não autorizado a cancelar notificação {NotificationId}")]
    private static partial void LogUnauthorizedCancel(ILogger logger, Guid notificationId, Guid userId);

    [LoggerMessage(EventId = 5029, Level = LogLevel.Warning,
        Message = "Status inválido para cancelamento da notificação {NotificationId}: {Status}")]
    private static partial void LogInvalidStatusForCancel(ILogger logger, Guid notificationId, NotificationStatus status);

    [LoggerMessage(EventId = 5030, Level = LogLevel.Information,
        Message = "Notificação {NotificationId} cancelada com sucesso por usuário {UserId}")]
    private static partial void LogNotificationCancelled(ILogger logger, Guid notificationId, Guid userId);
}