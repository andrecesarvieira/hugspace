using SynQcore.BlazorApp.Components;

namespace SynQcore.BlazorApp.Extensions;

/// <summary>
/// Métodos de extensão para configuração do pipeline HTTP do SynQcore
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura o pipeline HTTP do SynQcore
    /// </summary>
    public static WebApplication ConfigureSynQcorePipeline(this WebApplication app)
    {
        Console.WriteLine("🔄 Iniciando configuração do pipeline HTTP...");

        // Configuração por ambiente
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        // Pipeline HTTP
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        // Assets estáticos
        app.MapStaticAssets();

        // Componentes Razor
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(SynQcore.BlazorApp.Client._Imports).Assembly);

        Console.WriteLine("✅ Pipeline HTTP configurado!");

        return app;
    }
}