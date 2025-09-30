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
builder.Services.AddScoped<ILocalAuthService, LocalAuthService>(); // Backup auth service

// HttpClient com interceptor de autenticação
builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    // Configuração da base URL da API SynQcore
    client.BaseAddress = new Uri("http://localhost:5000/"); // URL da API SynQcore
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<AuthenticationHandler>();

// HttpClient para PostService
builder.Services.AddHttpClient<IPostService, PostService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<AuthenticationHandler>();

// HttpClient adicional para requisições sem interceptor
builder.Services.AddHttpClient("NoAuth", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
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
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SynQcore.BlazorApp.Client._Imports).Assembly);

app.Run();
