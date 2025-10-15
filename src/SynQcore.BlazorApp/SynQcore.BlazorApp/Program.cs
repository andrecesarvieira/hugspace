using SynQcore.BlazorApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configuração de componentes Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Configuração SynQcore modular
builder.Services
    .AddSynQcoreAuthentication()
    .AddSynQcoreStateManager()
    .AddSynQcoreCorporateServices()
    .AddSynQcoreHttpClients()
    .AddExternalLibraries();

var app = builder.Build();

// Configuração do pipeline HTTP
app.ConfigureSynQcorePipeline();

app.Run();
