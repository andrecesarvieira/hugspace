using MediatR;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para envio de notificações corporativas
/// </summary>
public partial class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, SendNotificationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<SendNotificationCommandHandler> _logger;

    public SendNotificationCommandHandler(
        ISynQcoreDbContext context,
        ILogger<SendNotificationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SendNotificationResponse> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        LogSendingNotification(_logger, request.NotificationId);

        var notification = await _context.CorporateNotifications
            .FindAsync([request.NotificationId], cancellationToken);

        if (notification == null)
        {
            LogNotificationNotFound(_logger, request.NotificationId);
            return new SendNotificationResponse 
            { 
                Success = false, 
                Message = "Notificação não encontrada" 
            };
        }

        if (notification.Status != NotificationStatus.Approved)
        {
            LogInvalidNotificationStatus(_logger, request.NotificationId, notification.Status);
            return new SendNotificationResponse 
            { 
                Success = false, 
                Message = $"Notificação deve estar aprovada para envio. Status atual: {notification.Status}" 
            };
        }

        // Atualizar status da notificação
        notification.Status = NotificationStatus.Sent;
        notification.UpdatedAt = DateTime.UtcNow;

        // Criar registros de entrega para cada canal configurado
        var deliveryRecords = new List<NotificationDelivery>();

        // Processar cada canal de entrega habilitado
        if (notification.EnabledChannels.HasFlag(NotificationChannels.InApp))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.InApp));
        }

        if (notification.EnabledChannels.HasFlag(NotificationChannels.Email))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.Email));
        }

        if (notification.EnabledChannels.HasFlag(NotificationChannels.Push))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.MobilePush));
        }

        if (notification.EnabledChannels.HasFlag(NotificationChannels.SMS))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.SMS));
        }

        if (notification.EnabledChannels.HasFlag(NotificationChannels.Webhook))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.Webhook));
        }

        if (notification.EnabledChannels.HasFlag(NotificationChannels.Teams))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.Teams));
        }

        if (notification.EnabledChannels.HasFlag(NotificationChannels.Slack))
        {
            deliveryRecords.Add(CreateDeliveryRecord(notification.Id, NotificationChannel.Slack));
        }

        // Adicionar registros de entrega ao contexto
        _context.NotificationDeliveries.AddRange(deliveryRecords);

        await _context.SaveChangesAsync(cancellationToken);

        LogNotificationSent(_logger, request.NotificationId, deliveryRecords.Count);

        return new SendNotificationResponse 
        { 
            Success = true, 
            Message = $"Notificação enviada com sucesso através de {deliveryRecords.Count} canal(is)",
            TotalRecipients = deliveryRecords.Count
        };
    }

    /// <summary>
    /// Cria um registro de entrega para um canal específico
    /// </summary>
    private static NotificationDelivery CreateDeliveryRecord(Guid notificationId, NotificationChannel channel, Guid employeeId = default)
    {
        return new NotificationDelivery
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            EmployeeId = employeeId,
            Channel = channel,
            Status = DeliveryStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [LoggerMessage(EventId = 5005, Level = LogLevel.Information,
        Message = "Enviando notificação: {NotificationId}")]
    private static partial void LogSendingNotification(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5006, Level = LogLevel.Warning,
        Message = "Notificação não encontrada para envio: {NotificationId}")]
    private static partial void LogNotificationNotFound(ILogger logger, Guid notificationId);

    [LoggerMessage(EventId = 5007, Level = LogLevel.Warning,
        Message = "Status inválido para envio da notificação {NotificationId}: {Status}")]
    private static partial void LogInvalidNotificationStatus(ILogger logger, Guid notificationId, NotificationStatus status);

    [LoggerMessage(EventId = 5008, Level = LogLevel.Information,
        Message = "Notificação {NotificationId} enviada com sucesso através de {ChannelCount} canal(is)")]
    private static partial void LogNotificationSent(ILogger logger, Guid notificationId, int channelCount);
}