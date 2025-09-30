using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SynQcore.BlazorApp.Client.Services;
using Blazored.LocalStorage;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configurar HttpClient para comunicação com API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "https://localhost:5001/")
});

// Serviços de armazenamento local
builder.Services.AddBlazoredLocalStorage();

// Serviços corporativos SynQcore
builder.Services.AddScoped<ThemeService>();

// MudBlazor (biblioteca de componentes)
builder.Services.AddMudServices();

// Fluxor (gerenciamento de estado) - será implementado na Fase 5.3
// builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly));

await builder.Build().RunAsync();
