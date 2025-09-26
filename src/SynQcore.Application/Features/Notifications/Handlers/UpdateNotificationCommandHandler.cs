using MediatR;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para atualização de notificações corporativas (apenas rascunhos)
/// </summary>
public partial class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, UpdateNotificationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateNotificationCommandHandler> _logger;

    public UpdateNotificationCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<UpdateNotificationCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UpdateNotificationResponse> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        LogUpdatingNotification(_logger, request.NotificationId, currentUserId);

        var notification = await _context.CorporateNotifications
            .FindAsync([request.NotificationId], cancellationToken);

        if (notification == null)
        {
            LogNotificationNotFound(_logger, request.NotificationId);
            return new UpdateNotificationResponse 
            { 
                Success = false, 
                Message = "Notificação não encontrada" 
            };
        }

        // Verificar se é o criador da notificação
        if (notification.CreatedByEmployeeId != currentUserId)
        {
            LogUnauthorizedUpdate(_logger, request.NotificationId, currentUserId);
            return new UpdateNotificationResponse 
            { 
                Success = false, 
                Message = "Você não tem permissão para atualizar esta notificação" 
            };
        }

        // Verificar se ainda é um rascunho
        if (notification.Status != NotificationStatus.Draft)
        {
            LogInvalidStatusForUpdate(_logger, request.NotificationId, notification.Status);
            return new UpdateNotificationResponse 
            { 
                Success = false, 
                Message = $"Apenas rascunhos podem ser atualizados. Status atual: {notification.Status}" 
            };
        }

        // Atualizar campos fornecidos
        var hasChanges = false;

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            notification.Title = request.Title;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Content))
        {
            notification.Content = request.Content;
            hasChanges = true;
        }

        if (request.Type.HasValue)
        {
            notification.Type = request.Type.Value;
            hasChanges = true;
        }

        if (request.Priority.HasValue)
        {
            notification.Priority = request.Priority.Value;
            hasChanges = true;
        }

        if (request.TargetDepartmentId.HasValue)
        {
            notification.TargetDepartmentId = request.TargetDepartmentId;
            hasChanges = true;
        }

        if (request.EnabledChannels.HasValue)
        {
            notification.EnabledChannels = request.EnabledChannels.Value;
            hasChanges = true;
        }

        if (request.ScheduledFor.HasValue)
        {
            notification.ScheduledFor = request.ScheduledFor;
            hasChanges = true;
        }

        if (request.ExpiresAt.HasValue)
        {
            notification.ExpiresAt = request.ExpiresAt;
            hasChanges = true;
        }

        if (request.RequiresApproval.HasValue)
        {
            notification.RequiresApproval = request.RequiresApproval.Value;
            hasChanges = true;
        }

        if (request.RequiresAcknowledgment.HasValue)
        {
            notification.RequiresAcknowledgment = request.RequiresAcknowledgment.Value;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(request.Metadata))
        {
            notification.Metadata = request.Metadata;
            hasChanges = true;
        }

        if (!hasChanges)
        {
            LogNoChangesProvided(_logger, request.NotificationId);
            return new UpdateNotificationResponse 
            { 
                Success = false, 
                Message = "Nenhuma alteração fornecida" 
            };
        }

        // Atualizar timestamp de modificação
        notification.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        LogNotificationUpdated(_logger, request.NotificationId, currentUserId);

        return new UpdateNotificationResponse 
        { 
            Success = true, 
            Message = "Notificação atualizada com sucesso" 
        };
    }

    [LoggerMessage(EventId = 5020, Level = LogLevel.Information,
        Message = "Atualizando notificação {NotificationId} por usuário {UserId}")]
    private static partial void LogUpdatingNotification(ILogger logger, Guid notificationId, Guid userId);

    [LoggerMessage(EventId = 5021, Level = LogLevel.Warning,
        Message = "Notificação não encontrada para atualização: {NotificationId}")]
    private static partial void LogNotificationNotFound(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5022, Level = LogLevel.Warning,
        Message = "Usuário {UserId} não autorizado a atualizar notificação {NotificationId}")]
    private static partial void LogUnauthorizedUpdate(ILogger logger, Guid notificationId, Guid userId);

    [LoggerMessage(EventId = 5023, Level = LogLevel.Warning,
        Message = "Status inválido para atualização da notificação {NotificationId}: {Status}")]
    private static partial void LogInvalidStatusForUpdate(ILogger logger, Guid notificationId, NotificationStatus status);

    [LoggerMessage(EventId = 5024, Level = LogLevel.Information,
        Message = "Nenhuma alteração fornecida para notificação {NotificationId}")]
    private static partial void LogNoChangesProvided(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5025, Level = LogLevel.Information,
        Message = "Notificação {NotificationId} atualizada com sucesso por usuário {UserId}")]
    private static partial void LogNotificationUpdated(ILogger logger, Guid notificationId, Guid userId);
}