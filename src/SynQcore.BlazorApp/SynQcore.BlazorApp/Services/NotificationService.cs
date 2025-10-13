using Microsoft.AspNetCore.SignalR.Client;
using SynQcore.BlazorApp.Models;
using System.Text.Json;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço para gerenciamento de notificações corporativas em tempo real
/// Integra com SignalR Hub para recebimento de push notifications
/// </summary>
public interface INotificationService : IAsyncDisposable
{
    /// <summary>
    /// Evento disparado quando uma nova notificação é recebida
    /// </summary>
    event Func<NotificationModel, Task>? NotificationReceived;

    /// <summary>
    /// Evento disparado quando contagem de não lidas é atualizada
    /// </summary>
    event Func<int, Task>? UnreadCountUpdated;

    /// <summary>
    /// Inicia conexão com o hub de notificações
    /// </summary>
    Task StartAsync(string accessToken);

    /// <summary>
    /// Para conexão com o hub
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Marca notificação como lida
    /// </summary>
    Task MarkAsReadAsync(string notificationId);

    /// <summary>
    /// Subscreve a tópico específico
    /// </summary>
    Task SubscribeToTopicAsync(string topic);

    /// <summary>
    /// Remove subscrição de tópico
    /// </summary>
    Task UnsubscribeFromTopicAsync(string topic);

    /// <summary>
    /// Solicita contagem atual de não lidas
    /// </summary>
    Task RequestUnreadCountAsync();

    /// <summary>
    /// Atualiza preferências de notificação
    /// </summary>
    Task UpdatePreferencesAsync(NotificationPreferences preferences);

    /// <summary>
    /// Status da conexão
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Lista de notificações recentes
    /// </summary>
    List<NotificationModel> RecentNotifications { get; }

    /// <summary>
    /// Contagem de notificações não lidas
    /// </summary>
    int UnreadCount { get; }
}

public partial class NotificationService : INotificationService
{
    private HubConnection? _hubConnection;
    private readonly ILogger<NotificationService> _logger;
    private readonly List<NotificationModel> _recentNotifications;
    private int _unreadCount;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(Level = LogLevel.Information, Message = "Conexão com NotificationHub estabelecida com sucesso")]
    private static partial void LogHubConnected(ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao conectar com NotificationHub: {message}")]
    private static partial void LogHubConnectionError(ILogger logger, string message, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Conexão com NotificationHub encerrada")]
    private static partial void LogHubDisconnected(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscrito ao tópico: {topic}")]
    private static partial void LogTopicSubscribed(ILogger logger, string topic);

    [LoggerMessage(Level = LogLevel.Information, Message = "Removida subscrição do tópico: {topic}")]
    private static partial void LogTopicUnsubscribed(ILogger logger, string topic);

    [LoggerMessage(Level = LogLevel.Information, Message = "Preferências de notificação atualizadas")]
    private static partial void LogPreferencesUpdated(ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao processar contagem de não lidas")]
    private static partial void LogUnreadCountError(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "NotificationHub conectado com sucesso: {data}")]
    private static partial void LogHubConnectedWithData(ILogger logger, string data);

    [LoggerMessage(Level = LogLevel.Information, Message = "Notificação marcada como lida: {data}")]
    private static partial void LogNotificationMarkedRead(ILogger logger, string data);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscrito ao tópico: {data}")]
    private static partial void LogTopicSubscribedWithData(ILogger logger, string data);

    [LoggerMessage(Level = LogLevel.Information, Message = "Removida subscrição do tópico: {data}")]
    private static partial void LogTopicUnsubscribedWithData(ILogger logger, string data);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao parsear notificação: {data}")]
    private static partial void LogNotificationParseError(ILogger logger, string data, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Nova notificação processada: {title} - Tipo: {type}")]
    private static partial void LogNotificationProcessed(ILogger logger, string title, NotificationType type);

    public event Func<NotificationModel, Task>? NotificationReceived;
    public event Func<int, Task>? UnreadCountUpdated;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    public List<NotificationModel> RecentNotifications => _recentNotifications.ToList();
    public int UnreadCount => _unreadCount;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
        _recentNotifications = new List<NotificationModel>();
        _unreadCount = 0;
    }

    public async Task StartAsync(string accessToken)
    {
        try
        {
            // Configurar conexão SignalR
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5005/hubs/corporate-notifications", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
                .Build();

            // Configurar handlers para diferentes tipos de notificação
            ConfigureNotificationHandlers();

            // Iniciar conexão
            await _hubConnection.StartAsync();
            LogHubConnected(_logger);

            // Solicitar contagem inicial de não lidas
            await RequestUnreadCountAsync();
        }
        catch (Exception ex)
        {
            LogHubConnectionError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            LogHubDisconnected(_logger);
        }
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("MarkNotificationAsRead", notificationId);
            
            // Atualizar localmente
            var notification = _recentNotifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                _unreadCount = Math.Max(0, _unreadCount - 1);
                await OnUnreadCountUpdated(_unreadCount);
            }
        }
    }

    public async Task SubscribeToTopicAsync(string topic)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SubscribeToTopic", topic);
            LogTopicSubscribed(_logger, topic);
        }
    }

    public async Task UnsubscribeFromTopicAsync(string topic)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("UnsubscribeFromTopic", topic);
            LogTopicUnsubscribed(_logger, topic);
        }
    }

    public async Task RequestUnreadCountAsync()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("RequestUnreadCount");
        }
    }

    public async Task UpdatePreferencesAsync(NotificationPreferences preferences)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("UpdateNotificationPreferences", preferences);
            LogPreferencesUpdated(_logger);
        }
    }

    private void ConfigureNotificationHandlers()
    {
        if (_hubConnection == null) return;

        // Handler para notificações pessoais
        _hubConnection.On<object>("PersonalNotification", async (data) =>
        {
            var notification = ParseNotification(data, NotificationType.Personal);
            await ProcessNotification(notification);
        });

        // Handler para notificações do departamento
        _hubConnection.On<object>("DepartmentNotification", async (data) =>
        {
            var notification = ParseNotification(data, NotificationType.Department);
            await ProcessNotification(notification);
        });

        // Handler para notificações corporativas
        _hubConnection.On<object>("CorporateNotification", async (data) =>
        {
            var notification = ParseNotification(data, NotificationType.Corporate);
            await ProcessNotification(notification);
        });

        // Handler para notificações por função
        _hubConnection.On<object>("RoleNotification", async (data) =>
        {
            var notification = ParseNotification(data, NotificationType.Role);
            await ProcessNotification(notification);
        });

        // Handler para notificações de tópico
        _hubConnection.On<object>("TopicNotification", async (data) =>
        {
            var notification = ParseNotification(data, NotificationType.Topic);
            await ProcessNotification(notification);
        });

        // Handler para contagem de não lidas
        _hubConnection.On<object>("UnreadNotificationCount", async (data) =>
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(data);
                var countData = JsonSerializer.Deserialize<UnreadCountResponse>(jsonData);
                if (countData != null)
                {
                    _unreadCount = countData.Count;
                    await OnUnreadCountUpdated(_unreadCount);
                }
            }
            catch (Exception ex)
            {
                LogUnreadCountError(_logger, ex);
            }
        });

        // Handler para confirmação de conexão
        _hubConnection.On<object>("NotificationHubConnected", (data) =>
        {
            LogHubConnectedWithData(_logger, JsonSerializer.Serialize(data));
            return Task.CompletedTask;
        });

        // Handler para notificação marcada como lida
        _hubConnection.On<object>("NotificationMarkedAsRead", (data) =>
        {
            LogNotificationMarkedRead(_logger, JsonSerializer.Serialize(data));
            return Task.CompletedTask;
        });

        // Handlers para subscrições de tópicos
        _hubConnection.On<object>("SubscribedToTopic", (data) =>
        {
            LogTopicSubscribedWithData(_logger, JsonSerializer.Serialize(data));
            return Task.CompletedTask;
        });

        _hubConnection.On<object>("UnsubscribedFromTopic", (data) =>
        {
            LogTopicUnsubscribedWithData(_logger, JsonSerializer.Serialize(data));
            return Task.CompletedTask;
        });
    }

    private NotificationModel ParseNotification(object data, NotificationType type)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(data);
            var notificationData = JsonSerializer.Deserialize<NotificationData>(jsonData);

            return new NotificationModel
            {
                Id = notificationData?.Id ?? Guid.NewGuid().ToString(),
                Title = notificationData?.Title ?? "Nova Notificação",
                Message = notificationData?.Message ?? "Mensagem da notificação",
                Type = type,
                Priority = notificationData?.Priority ?? NotificationPriority.Normal,
                IsRead = false,
                Timestamp = DateTimeOffset.UtcNow,
                IconClass = GetIconForType(type),
                ActionUrl = notificationData?.ActionUrl,
                Category = notificationData?.Category ?? "geral"
            };
        }
        catch (Exception ex)
        {
            LogNotificationParseError(_logger, JsonSerializer.Serialize(data), ex);
            
            // Retornar notificação padrão em caso de erro
            return new NotificationModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Nova Notificação",
                Message = "Erro ao carregar detalhes da notificação",
                Type = type,
                Priority = NotificationPriority.Normal,
                IsRead = false,
                Timestamp = DateTimeOffset.UtcNow,
                IconClass = "fas fa-bell"
            };
        }
    }

    private async Task ProcessNotification(NotificationModel notification)
    {
        // Adicionar à lista de recentes (manter apenas as 50 mais recentes)
        _recentNotifications.Insert(0, notification);
        if (_recentNotifications.Count > 50)
        {
            _recentNotifications.RemoveRange(50, _recentNotifications.Count - 50);
        }

        // Incrementar contador de não lidas
        _unreadCount++;

        // Disparar eventos
        await OnNotificationReceived(notification);
        await OnUnreadCountUpdated(_unreadCount);

        LogNotificationProcessed(_logger, notification.Title, notification.Type);
    }

    private static string GetIconForType(NotificationType type)
    {
        return type switch
        {
            NotificationType.Personal => "fas fa-user",
            NotificationType.Department => "fas fa-building",
            NotificationType.Corporate => "fas fa-building-columns",
            NotificationType.Role => "fas fa-user-tie",
            NotificationType.Topic => "fas fa-hashtag",
            _ => "fas fa-bell"
        };
    }

    private async Task OnNotificationReceived(NotificationModel notification)
    {
        if (NotificationReceived != null)
        {
            await NotificationReceived.Invoke(notification);
        }
    }

    private async Task OnUnreadCountUpdated(int count)
    {
        if (UnreadCountUpdated != null)
        {
            await UnreadCountUpdated.Invoke(count);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
        
        GC.SuppressFinalize(this);
    }
}

// Classes de apoio para desserialização
public class NotificationData
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public NotificationPriority Priority { get; set; }
    public string? ActionUrl { get; set; }
    public string? Category { get; set; }
}

public class UnreadCountResponse
{
    public string? UserId { get; set; }
    public int Count { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}