using Blazored.LocalStorage;
using MudBlazor.Services;
using SynQcore.BlazorApp.Services;
using SynQcore.BlazorApp.Services.StateManagement;
using SynQcore.BlazorApp.Client.Services;

namespace SynQcore.BlazorApp.Extensions;

/// <summary>
/// Métodos de extensão para configuração de serviços do SynQcore
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura autenticação e autorização corporativa
    /// </summary>
    public static IServiceCollection AddSynQcoreAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                options.Cookie.Name = "SynQcore.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

        services.AddAuthorization(options =>
        {
            // Políticas de autorização corporativa
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
            options.AddPolicy("EmployeeAccess", policy => policy.RequireAuthenticatedUser());
        });

        services.AddHttpContextAccessor();
        
        return services;
    }

    /// <summary>
    /// Configura o SynQcore State Manager
    /// </summary>
    public static IServiceCollection AddSynQcoreStateManager(this IServiceCollection services)
    {
        Console.WriteLine("🔄 Configurando SynQcore State Manager...");
        
        services.AddSingleton<UserStateService>();
        services.AddSingleton<UIStateService>();
        services.AddSingleton<SimpleStateService>();
        services.AddSingleton<StateManager>();
        
        Console.WriteLine("✅ SynQcore State Manager configurado!");
        
        return services;
    }

    /// <summary>
    /// Configura serviços corporativos do SynQcore
    /// </summary>
    public static IServiceCollection AddSynQcoreCorporateServices(this IServiceCollection services)
    {
        // Serviços base
        services.AddScoped<ThemeService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILocalAuthService, LocalAuthService>();
        services.AddScoped<ICookieAuthService, CookieAuthService>();
        services.AddScoped<IModerationService, ModerationService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Serviços de negócio
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IPlatformStatsService, PlatformStatsService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEndorsementService, EndorsementService>();
        services.AddScoped<IKnowledgeService, KnowledgeService>();
        services.AddScoped<IDiscussionThreadService, DiscussionThreadService>();
        
        return services;
    }

    /// <summary>
    /// Configura HttpClients com autenticação
    /// </summary>
    public static IServiceCollection AddSynQcoreHttpClients(this IServiceCollection services)
    {
        const string ApiBaseUrl = "http://localhost:5005/api/"; // Porta corrigida para 5005
        const int TimeoutSeconds = 30;

        // Handler de autenticação
        services.AddScoped<AuthenticationHandler>();

        // ApiService principal
        services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Serviços do Feed
        services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        services.AddHttpClient<ISearchService, SearchService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        services.AddHttpClient<IPostService, PostService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        // HttpClient sem autenticação
        services.AddHttpClient("NoAuth", client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
        });

        return services;
    }

    /// <summary>
    /// Configura bibliotecas externas
    /// </summary>
    public static IServiceCollection AddExternalLibraries(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorage();
        services.AddMudServices();
        
        return services;
    }
}