using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Notifications.Handlers;

/// <summary>
/// Handler para criação de notificações corporativas
/// </summary>
public partial class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, CreateNotificationResponse>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateNotificationCommandHandler> _logger;

    public CreateNotificationCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CreateNotificationCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CreateNotificationResponse> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        
        LogCreatingNotification(_logger, currentUserId.ToString(), request.Title, request.Type);

        try
        {
            // Converter strings para enums
            if (!Enum.TryParse<NotificationType>(request.Type, out var notificationType))
            {
                return new CreateNotificationResponse
                {
                    Success = false,
                    Message = $"Tipo de notificação inválido: {request.Type}"
                };
            }

            if (!Enum.TryParse<NotificationPriority>(request.Priority, out var priority))
            {
                return new CreateNotificationResponse
                {
                    Success = false,
                    Message = $"Prioridade inválida: {request.Priority}"
                };
            }

            // Converter canais para flags enum
            var enabledChannels = ConvertChannelsToFlags(request.EnabledChannels);

            // Verificar se o departamento alvo existe (se especificado)
            if (request.TargetDepartmentId.HasValue)
            {
                var departmentExists = await _context.Departments
                    .AnyAsync(d => d.Id == request.TargetDepartmentId.Value, cancellationToken);

                if (!departmentExists)
                {
                    return new CreateNotificationResponse
                    {
                        Success = false,
                        Message = "Departamento alvo não encontrado"
                    };
                }
            }

            // Aplicar template se especificado
            string title = request.Title;
            string content = request.Content;
            
            if (!string.IsNullOrEmpty(request.TemplateCode))
            {
                var template = await _context.NotificationTemplates
                    .FirstOrDefaultAsync(t => t.Code == request.TemplateCode && t.IsActive, cancellationToken);

                if (template != null)
                {
                    title = ApplyPlaceholders(template.TitleTemplate, request.PlaceholderData);
                    content = ApplyPlaceholders(template.ContentTemplate, request.PlaceholderData);
                }
            }

            // Criar notificação
            var notification = new CorporateNotification
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                Type = notificationType,
                Priority = priority,
                Status = request.RequiresApproval ? NotificationStatus.PendingApproval : NotificationStatus.Approved,
                CreatedByEmployeeId = currentUserId,
                TargetDepartmentId = request.TargetDepartmentId,
                ExpiresAt = request.ExpiresAt,
                ScheduledFor = request.ScheduledFor ?? DateTimeOffset.UtcNow,
                RequiresApproval = request.RequiresApproval,
                RequiresAcknowledgment = request.RequiresAcknowledgment,
                EnabledChannels = enabledChannels
            };

            _context.CorporateNotifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);

            // Calcular destinatários estimados
            var estimatedRecipients = await CalculateEstimatedRecipients(notification, cancellationToken);

            LogNotificationCreated(_logger, notification.Id.ToString(), notification.Status.ToString(), estimatedRecipients);

            return new CreateNotificationResponse
            {
                Success = true,
                Message = request.RequiresApproval 
                    ? "Notificação criada e enviada para aprovação"
                    : "Notificação criada com sucesso",
                NotificationId = notification.Id,
                Status = notification.Status.ToString(),
                EstimatedRecipients = estimatedRecipients,
                SendDateTime = notification.ScheduledFor
            };
        }
        catch (Exception ex)
        {
            LogNotificationCreationError(_logger, ex, currentUserId.ToString(), request.Title);
            
            return new CreateNotificationResponse
            {
                Success = false,
                Message = "Erro interno ao criar notificação"
            };
        }
    }

    /// <summary>
    /// Converte lista de canais para flags enum
    /// </summary>
    private static NotificationChannels ConvertChannelsToFlags(List<string> channels)
    {
        var flags = NotificationChannels.None;

        foreach (var channel in channels)
        {
            if (Enum.TryParse<NotificationChannels>(channel, out var channelFlag))
            {
                flags |= channelFlag;
            }
        }

        // Se nenhum canal foi especificado, usar InApp como padrão
        return flags == NotificationChannels.None ? NotificationChannels.InApp : flags;
    }

    /// <summary>
    /// Aplica placeholders ao template
    /// </summary>
    private static string ApplyPlaceholders(string template, Dictionary<string, string>? placeholderData)
    {
        if (placeholderData == null || placeholderData.Count == 0)
            return template;

        var result = template;
        foreach (var kvp in placeholderData)
        {
            result = result.Replace($"{{{kvp.Key}}}", kvp.Value, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    /// <summary>
    /// Calcula número estimado de destinatários
    /// </summary>
    private async Task<int> CalculateEstimatedRecipients(CorporateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.TargetDepartmentId.HasValue)
        {
            // Contar funcionários do departamento específico
            return await _context.EmployeeDepartments
                .Where(ed => ed.DepartmentId == notification.TargetDepartmentId.Value)
                .CountAsync(cancellationToken);
        }

        // Company-wide: todos os funcionários ativos
        return await _context.Employees
            .Where(e => !e.IsDeleted)
            .CountAsync(cancellationToken);
    }

    #region LoggerMessage Delegates

    [LoggerMessage(EventId = 4301, Level = LogLevel.Information,
        Message = "Criando notificação - UserId: {UserId} - Título: {Title} - Tipo: {Type}")]
    private static partial void LogCreatingNotification(ILogger logger, string userId, string title, string type);

    [LoggerMessage(EventId = 4302, Level = LogLevel.Information,
        Message = "Notificação criada com sucesso - ID: {NotificationId} - Status: {Status} - Destinatários estimados: {EstimatedRecipients}")]
    private static partial void LogNotificationCreated(ILogger logger, string notificationId, string status, int estimatedRecipients);

    [LoggerMessage(EventId = 4303, Level = LogLevel.Error,
        Message = "Erro ao criar notificação - UserId: {UserId} - Título: {Title}")]
    private static partial void LogNotificationCreationError(ILogger logger, Exception exception, string userId, string title);

    #endregion
}