using System.ComponentModel;
using System.Runtime.CompilerServices;
using SynQcore.BlazorApp.Store.User;

namespace SynQcore.BlazorApp.Services.StateManagement;

/// <summary>
/// Base class para gerenciamento de estado observável
/// </summary>
public abstract class BaseStateService : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? StateChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        StateChanged?.Invoke();
    }

    protected virtual void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return;

        backingStore = value;
        OnPropertyChanged(propertyName);
    }
}

/// <summary>
/// Serviço de gerenciamento de estado do usuário
/// </summary>
public class UserStateService : BaseStateService
{
    private bool _isAuthenticated;
    private UserInfo? _currentUser;
    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private DateTime? _tokenExpiresAt;
    private List<string> _permissions = new();
    private bool _isLoading;
    private string? _lastAuthError;

    // Propriedades públicas
    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set => SetProperty(ref _isAuthenticated, value);
    }

    public UserInfo? CurrentUser
    {
        get => _currentUser;
        private set => SetProperty(ref _currentUser, value);
    }

    public string AccessToken
    {
        get => _accessToken;
        private set => SetProperty(ref _accessToken, value);
    }

    public string RefreshToken
    {
        get => _refreshToken;
        private set => SetProperty(ref _refreshToken, value);
    }

    public DateTime? TokenExpiresAt
    {
        get => _tokenExpiresAt;
        private set => SetProperty(ref _tokenExpiresAt, value);
    }

    public List<string> Permissions
    {
        get => _permissions;
        private set => SetProperty(ref _permissions, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string? LastAuthError
    {
        get => _lastAuthError;
        private set => SetProperty(ref _lastAuthError, value);
    }

    // Ações públicas
    public async Task LoginAsync(string email, string password)
    {
        Console.WriteLine($"🟢 [UserStateService] LoginAsync iniciado para: {email}");
        
        IsLoading = true;
        LastAuthError = null;

        try
        {
            // Simular chamada de API
            await Task.Delay(1000);
            
            // Simular login bem-sucedido
            AccessToken = "fake-jwt-token";
            RefreshToken = "fake-refresh-token";
            TokenExpiresAt = DateTime.UtcNow.AddHours(1);
            Permissions = new List<string> { "read", "write", "admin" };
            
            // Definir dados do usuário
            CurrentUser = new UserInfo
            {
                Id = "1",
                Nome = "Usuário Administrador",
                Email = email,
                Username = "admin",
                Cargo = "Administrador",
                Departamento = "TI",
                DataCadastro = DateTime.Now.AddMonths(-6),
                UltimoAcesso = DateTime.Now,
                IsAtivo = true,
                Roles = new List<string> { "Admin", "User" }
            };
            
            IsAuthenticated = true;
            
            Console.WriteLine($"🟢 [UserStateService] Login bem-sucedido para: {email}");
        }
        catch (Exception ex)
        {
            LastAuthError = ex.Message;
            Console.WriteLine($"❌ [UserStateService] Erro no login: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Logout()
    {
        Console.WriteLine($"🟢 [UserStateService] Logout executado");
        
        IsAuthenticated = false;
        CurrentUser = null;
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
        TokenExpiresAt = null;
        Permissions.Clear();
        LastAuthError = null;
    }

    public void SetAuthenticatedUser(UserInfo userInfo, string accessToken, string refreshToken)
    {
        Console.WriteLine($"🟢 [UserStateService] SetAuthenticatedUser para: {userInfo.Nome}");
        
        IsAuthenticated = true;
        CurrentUser = userInfo;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TokenExpiresAt = DateTime.UtcNow.AddHours(1); // Default 1 hora
        LastAuthError = null;
        
        // Atualizar permissões se disponíveis
        if (userInfo.Roles != null && userInfo.Roles.Count > 0)
        {
            Permissions = new List<string>(userInfo.Roles);
        }
    }

    public void ClearError()
    {
        LastAuthError = null;
    }
}

/// <summary>
/// Serviço de gerenciamento de estado da UI
/// </summary>
public class UIStateService : BaseStateService
{
    private bool _isLoading;
    private bool _isSidebarExpanded = true;
    private string? _currentModal;
    private List<string> _notifications = new();
    private bool _isConnected = true;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsSidebarExpanded
    {
        get => _isSidebarExpanded;
        set => SetProperty(ref _isSidebarExpanded, value);
    }

    public string? CurrentModal
    {
        get => _currentModal;
        set => SetProperty(ref _currentModal, value);
    }

    public List<string> Notifications
    {
        get => _notifications;
        private set => SetProperty(ref _notifications, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        private set => SetProperty(ref _isConnected, value);
    }

    public void ToggleSidebar()
    {
        Console.WriteLine($"🟢 [UIStateService] ToggleSidebar: {!IsSidebarExpanded}");
        IsSidebarExpanded = !IsSidebarExpanded;
    }

    public void ShowModal(string modalName)
    {
        Console.WriteLine($"🟢 [UIStateService] ShowModal: {modalName}");
        CurrentModal = modalName;
    }

    public void CloseModal()
    {
        Console.WriteLine($"🟢 [UIStateService] CloseModal");
        CurrentModal = null;
    }

    public void AddNotification(string message)
    {
        Console.WriteLine($"🟢 [UIStateService] AddNotification: {message}");
        var newNotifications = new List<string>(Notifications) { message };
        Notifications = newNotifications;
    }

    public void RemoveNotification(string message)
    {
        Console.WriteLine($"🟢 [UIStateService] RemoveNotification: {message}");
        var newNotifications = Notifications.Where(n => n != message).ToList();
        Notifications = newNotifications;
    }

    public void SetConnectivityStatus(bool isConnected)
    {
        Console.WriteLine($"🟢 [UIStateService] SetConnectivityStatus: {isConnected}");
        IsConnected = isConnected;
    }
}

/// <summary>
/// Serviço simples para testes (substitui SimpleState do Fluxor)
/// </summary>
public class SimpleStateService : BaseStateService
{
    private string _message = "Estado inicial";
    private int _count;

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public void SetMessage(string message)
    {
        Console.WriteLine($"🟢 [SimpleStateService] SetMessage: {message}");
        Message = message;
    }

    public void Increment()
    {
        Console.WriteLine($"🟢 [SimpleStateService] Increment: {Count} -> {Count + 1}");
        Count++;
    }

    public void Reset()
    {
        Console.WriteLine($"🟢 [SimpleStateService] Reset");
        Message = "Estado inicial";
        Count = 0;
    }
}

/// <summary>
/// Gerenciador central de estado - substitui o Store do Fluxor
/// </summary>
public class StateManager
{
    public UserStateService User { get; }
    public UIStateService UI { get; }
    public SimpleStateService Simple { get; }

    public StateManager(UserStateService user, UIStateService ui, SimpleStateService simple)
    {
        User = user;
        UI = ui;
        Simple = simple;
        
        Console.WriteLine("🟢 [StateManager] Inicializado com todos os serviços de estado");
    }

    public void LogCurrentState()
    {
        Console.WriteLine("🔍 [StateManager] Estado atual:");
        Console.WriteLine($"  User.IsAuthenticated: {User.IsAuthenticated}");
        Console.WriteLine($"  UI.IsLoading: {UI.IsLoading}");
        Console.WriteLine($"  UI.IsSidebarExpanded: {UI.IsSidebarExpanded}");
        Console.WriteLine($"  Simple.Message: '{Simple.Message}'");
        Console.WriteLine($"  Simple.Count: {Simple.Count}");
    }
}