using SynQcore.BlazorApp.Components;
using SynQcore.BlazorApp.Client.Services;
using SynQcore.BlazorApp.Services;
using SynQcore.BlazorApp.Store.Effects;
using Blazored.LocalStorage;
using MudBlazor.Services;
using Fluxor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Configuração de Autenticação e Autorização
builder.Services.AddAuthentication("Cookies")
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

builder.Services.AddAuthorization(options =>
{
    // Políticas de autorização corporativa
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
    options.AddPolicy("EmployeeAccess", policy => policy.RequireAuthenticatedUser());
});

// Serviços necessários para autenticação
builder.Services.AddHttpContextAccessor();

// Serviços de armazenamento local
builder.Services.AddBlazoredLocalStorage();

// Fluxor - Gerenciamento de Estado Global
Console.WriteLine("🔄 Configurando Fluxor...");
builder.Services.AddFluxor(options =>
{
    // Escanear o assembly atual para encontrar todos os States e Reducers
    options.ScanAssemblies(typeof(Program).Assembly);

    // Debug: Mostrar assemblies sendo escaneados
    Console.WriteLine($"🔍 Fluxor escaneando assembly: {typeof(Program).Assembly.FullName}");

    // Forçar descoberta de features específicas se necessário
    Console.WriteLine("🔍 Escaneando por States e Reducers...");

    // Adicionar logs para descoberta
    var assembly = typeof(Program).Assembly;
    var states = assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(Fluxor.FeatureStateAttribute), false).Length > 0);
    var reducers = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes(typeof(Fluxor.ReducerMethodAttribute), false).Length > 0));

    Console.WriteLine($"🔍 States encontrados: {states.Count()}");
    foreach (var state in states)
    {
        Console.WriteLine($"   - {state.FullName}");
    }

    Console.WriteLine($"🔍 Classes com Reducers encontradas: {reducers.Count()}");
    foreach (var reducer in reducers)
    {
        Console.WriteLine($"   - {reducer.FullName}");
    }
});
Console.WriteLine("✅ Fluxor configurado!");// Serviços corporativos SynQcore
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<IStateInitializationService, StateInitializationService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICookieAuthService, CookieAuthService>(); // Novo serviço de autenticação com cookies
builder.Services.AddScoped<ILocalAuthService, LocalAuthService>(); // Backup auth service
builder.Services.AddScoped<IModerationService, ModerationService>(); // Serviço de moderação
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>(); // Serviço de permissões
builder.Services.AddScoped<IPlatformStatsService, PlatformStatsService>(); // Serviço de estatísticas
builder.Services.AddScoped<IEmployeeService, EmployeeService>(); // Serviço de funcionários
builder.Services.AddScoped<IEndorsementService, EndorsementService>(); // Serviço de endorsements
builder.Services.AddScoped<IKnowledgeService, KnowledgeService>(); // Serviço de gestão de conhecimento
builder.Services.AddScoped<IDiscussionThreadService, DiscussionThreadService>(); // Serviço de discussion threads
builder.Services.AddScoped<INotificationService, NotificationService>(); // Serviço de notificações real-time

// HttpClient com interceptor de autenticação
builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    // Configuração da base URL da API SynQcore
    client.BaseAddress = new Uri("http://localhost:5000/api/"); // URL da API SynQcore com prefixo /api/
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<AuthenticationHandler>();

// HttpClient para PostService
builder.Services.AddHttpClient<IPostService, PostService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<AuthenticationHandler>();

// HttpClient adicional para requisições sem interceptor
builder.Services.AddHttpClient("NoAuth", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Effects do Fluxor
builder.Services.AddScoped<UserEffects>();
builder.Services.AddScoped<UIEffects>();

// MudBlazor (biblioteca de componentes)
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
Console.WriteLine("🔄 Iniciando configuração do pipeline HTTP...");
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SynQcore.BlazorApp.Client._Imports).Assembly);

app.Run();
