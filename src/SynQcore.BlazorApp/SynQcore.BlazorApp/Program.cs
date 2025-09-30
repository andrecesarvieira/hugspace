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
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
});

// Serviços corporativos SynQcore
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<IStateInitializationService, StateInitializationService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
