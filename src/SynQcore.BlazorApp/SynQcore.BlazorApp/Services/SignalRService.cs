using Microsoft.AspNetCore.SignalR.Client;
using SynQcore.BlazorApp.Components.Social;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Eventos de post criado
/// </summary>
public class PostCreatedEventArgs : EventArgs
{
    public SimplePostCard.PostModel Post { get; set; } = default!;
}

/// <summary>
/// Eventos de comentário adicionado
/// </summary>
public class CommentAddedEventArgs : EventArgs
{
    public Guid PostId { get; set; }
    public SimplePostCard.CommentModel Comment { get; set; } = default!;
}

/// <summary>
/// Eventos de notificação recebida
/// </summary>
public class NotificationEventArgs : EventArgs
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Interface para serviço SignalR de comunicação em tempo real
/// </summary>
public interface ISignalRService
{
    Task StartAsync();
    Task StopAsync();
    bool IsConnected { get; }
    
    event EventHandler<PostCreatedEventArgs>? OnPostCreated;
    event EventHandler<CommentAddedEventArgs>? OnCommentAdded;
    event EventHandler<NotificationEventArgs>? OnNotificationReceived;
}

/// <summary>
/// Serviço de comunicação em tempo real usando SignalR
/// </summary>
public partial class SignalRService : ISignalRService, IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SignalRService> _logger;
    private HubConnection? _feedHub;
    private HubConnection? _notificationHub;
    private HubConnection? _collaborationHub;
    private bool _isStarted;

    // LoggerMessage delegates para alta performance
    [LoggerMessage(LogLevel.Information, "Iniciando conexões SignalR")]
    private static partial void LogStartingConnections(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Hub {HubName} conectado com sucesso")]
    private static partial void LogHubConnected(ILogger logger, string hubName);

    [LoggerMessage(LogLevel.Warning, "Hub {HubName} desconectado. Tentando reconectar...")]
    private static partial void LogHubDisconnected(ILogger logger, string hubName);

    [LoggerMessage(LogLevel.Error, "Erro ao conectar hub {HubName}")]
    private static partial void LogHubConnectionError(ILogger logger, string hubName, Exception exception);

    [LoggerMessage(LogLevel.Information, "Post criado recebido via SignalR: {PostId}")]
    private static partial void LogPostCreated(ILogger logger, Guid postId);

    [LoggerMessage(LogLevel.Information, "Comentário adicionado via SignalR no post: {PostId}")]
    private static partial void LogCommentAdded(ILogger logger, Guid postId);

    [LoggerMessage(LogLevel.Information, "Notificação recebida via SignalR: {Type}")]
    private static partial void LogNotificationReceived(ILogger logger, string type);

    [LoggerMessage(LogLevel.Error, "Erro ao processar post criado via SignalR")]
    private static partial void LogPostCreatedProcessingError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao processar comentário adicionado via SignalR")]
    private static partial void LogCommentAddedProcessingError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao processar notificação via SignalR")]
    private static partial void LogNotificationProcessingError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Erro ao parar conexões SignalR")]
    private static partial void LogStopConnectionsError(ILogger logger, Exception exception);

    public bool IsConnected => _feedHub?.State == HubConnectionState.Connected;

    public event EventHandler<PostCreatedEventArgs>? OnPostCreated;
    public event EventHandler<CommentAddedEventArgs>? OnCommentAdded;
    public event EventHandler<NotificationEventArgs>? OnNotificationReceived;

    public SignalRService(IConfiguration configuration, ILogger<SignalRService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        if (_isStarted) return;

        LogStartingConnections(_logger);

        try
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";

            // Configurar FeedHub
            _feedHub = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/hubs/feed")
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            _feedHub.On<object>("PostCreated", OnPostCreatedReceived);
            _feedHub.On<Guid, object>("CommentAdded", OnCommentAddedReceived);
            _feedHub.Closed += async (error) =>
            {
                LogHubDisconnected(_logger, "FeedHub");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await ConnectHub(_feedHub, "FeedHub");
            };

            // Configurar NotificationHub
            _notificationHub = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/hubs/notifications")
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            _notificationHub.On<string, string>("ReceiveNotification", OnNotificationReceivedReceived);
            _notificationHub.Closed += async (error) =>
            {
                LogHubDisconnected(_logger, "NotificationHub");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await ConnectHub(_notificationHub, "NotificationHub");
            };

            // Configurar CollaborationHub
            _collaborationHub = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/hubs/collaboration")
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            _collaborationHub.Closed += async (error) =>
            {
                LogHubDisconnected(_logger, "CollaborationHub");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await ConnectHub(_collaborationHub, "CollaborationHub");
            };

            // Iniciar conexões
            await ConnectHub(_feedHub, "FeedHub");
            await ConnectHub(_notificationHub, "NotificationHub");
            await ConnectHub(_collaborationHub, "CollaborationHub");

            _isStarted = true;
        }
        catch (Exception ex)
        {
            LogHubConnectionError(_logger, "All", ex);
            throw;
        }
    }

    private async Task ConnectHub(HubConnection? hub, string hubName)
    {
        if (hub == null) return;

        try
        {
            if (hub.State == HubConnectionState.Disconnected)
            {
                await hub.StartAsync();
                LogHubConnected(_logger, hubName);
            }
        }
        catch (Exception ex)
        {
            LogHubConnectionError(_logger, hubName, ex);
        }
    }

    private void OnPostCreatedReceived(object postData)
    {
        try
        {
            // Converter dados do post para PostModel
            var post = ConvertToPostModel(postData);
            if (post != null)
            {
                LogPostCreated(_logger, post.Id);
                OnPostCreated?.Invoke(this, new PostCreatedEventArgs { Post = post });
            }
        }
        catch (Exception ex)
        {
            LogPostCreatedProcessingError(_logger, ex);
        }
    }

    private void OnCommentAddedReceived(Guid postId, object commentData)
    {
        try
        {
            // Converter dados do comentário para CommentModel
            var comment = ConvertToCommentModel(commentData);
            if (comment != null)
            {
                LogCommentAdded(_logger, postId);
                OnCommentAdded?.Invoke(this, new CommentAddedEventArgs 
                { 
                    PostId = postId, 
                    Comment = comment 
                });
            }
        }
        catch (Exception ex)
        {
            LogCommentAddedProcessingError(_logger, ex);
        }
    }

    private void OnNotificationReceivedReceived(string message, string type)
    {
        try
        {
            LogNotificationReceived(_logger, type);
            OnNotificationReceived?.Invoke(this, new NotificationEventArgs
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            LogNotificationProcessingError(_logger, ex);
        }
    }

    private static SimplePostCard.PostModel? ConvertToPostModel(object data)
    {
        // Implementação simplificada - pode ser melhorada com AutoMapper ou conversão mais robusta
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return System.Text.Json.JsonSerializer.Deserialize<SimplePostCard.PostModel>(json);
        }
        catch
        {
            return null;
        }
    }

    private static SimplePostCard.CommentModel? ConvertToCommentModel(object data)
    {
        // Implementação simplificada - pode ser melhorada com AutoMapper ou conversão mais robusta
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return System.Text.Json.JsonSerializer.Deserialize<SimplePostCard.CommentModel>(json);
        }
        catch
        {
            return null;
        }
    }

    public async Task StopAsync()
    {
        if (!_isStarted) return;

        try
        {
            if (_feedHub != null)
                await _feedHub.StopAsync();
            
            if (_notificationHub != null)
                await _notificationHub.StopAsync();
            
            if (_collaborationHub != null)
                await _collaborationHub.StopAsync();

            _isStarted = false;
        }
        catch (Exception ex)
        {
            LogStopConnectionsError(_logger, ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();

        if (_feedHub != null)
            await _feedHub.DisposeAsync();
        
        if (_notificationHub != null)
            await _notificationHub.DisposeAsync();
        
        if (_collaborationHub != null)
            await _collaborationHub.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
