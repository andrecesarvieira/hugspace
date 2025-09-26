using MediatR;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para aprovação de notificações corporativas
/// </summary>
public partial class ApproveNotificationCommandHandler : IRequestHandler<ApproveNotificationCommand, ApproveNotificationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApproveNotificationCommandHandler> _logger;

    public ApproveNotificationCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ApproveNotificationCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ApproveNotificationResponse> Handle(ApproveNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        LogApprovingNotification(_logger, request.NotificationId, currentUserId);

        var notification = await _context.CorporateNotifications
            .FindAsync([request.NotificationId], cancellationToken);

        if (notification == null)
        {
            LogNotificationNotFound(_logger, request.NotificationId);
            return new ApproveNotificationResponse 
            { 
                Success = false, 
                Message = "Notificação não encontrada" 
            };
        }

        if (notification.Status != NotificationStatus.PendingApproval)
        {
            LogInvalidNotificationStatus(_logger, request.NotificationId, notification.Status);
            return new ApproveNotificationResponse 
            { 
                Success = false, 
                Message = $"Notificação não pode ser processada. Status atual: {notification.Status}" 
            };
        }

        // Atualizar status da notificação baseado na aprovação
        if (request.IsApproved)
        {
            notification.Status = NotificationStatus.Approved;
            notification.ApprovedByEmployeeId = currentUserId;
            notification.ApprovedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            notification.Status = NotificationStatus.Rejected;
        }

        notification.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var action = request.IsApproved ? "aprovada" : "rejeitada";
        LogNotificationProcessed(_logger, request.NotificationId, currentUserId, action);

        return new ApproveNotificationResponse 
        { 
            Success = true, 
            Message = $"Notificação {action} com sucesso" 
        };
    }

    [LoggerMessage(EventId = 5001, Level = LogLevel.Information,
        Message = "Processando notificação: {NotificationId} por usuário: {UserId}")]
    private static partial void LogApprovingNotification(ILogger logger, Guid notificationId, Guid userId);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Warning,
        Message = "Notificação não encontrada: {NotificationId}")]
    private static partial void LogNotificationNotFound(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5003, Level = LogLevel.Warning,
        Message = "Status inválido para processamento da notificação {NotificationId}: {Status}")]
    private static partial void LogInvalidNotificationStatus(ILogger logger, Guid notificationId, NotificationStatus status);

    [LoggerMessage(EventId = 5004, Level = LogLevel.Information,
        Message = "Notificação {NotificationId} {Action} com sucesso por {UserId}")]
    private static partial void LogNotificationProcessed(ILogger logger, Guid notificationId, Guid userId, string action);
}