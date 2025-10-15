using Microsoft.JSInterop;

namespace SynQcore.BlazorApp.Client.Services;

/// <summary>
/// Serviço de tema simplificado - SEMPRE MODO CLARO
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private const string CurrentTheme = "light"; // FORÇADO PARA SEMPRE SER CLARO

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Tema atual - sempre "light"
    /// </summary>
    public static string GetCurrentTheme() => CurrentTheme;

    /// <summary>
    /// Verifica se o tema atual é escuro - sempre false
    /// </summary>
    public static bool IsDarkTheme => false;

    /// <summary>
    /// Inicializa o serviço - força tema claro
    /// </summary>
    public async Task InitializeAsync()
    {
        await ApplyLightThemeAsync();
    }

    /// <summary>
    /// Toggle desabilitado - sempre mantém modo claro
    /// </summary>
    public static async Task ToggleThemeAsync()
    {
        // Não faz nada - sempre modo claro
        await Task.CompletedTask;
    }

    /// <summary>
    /// Força tema claro no DOM
    /// </summary>
    private async Task ApplyLightThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", @"
                document.documentElement.setAttribute('data-theme', 'light');
                document.documentElement.style.colorScheme = 'light only';
                document.body.className = document.body.className.replace(/\btheme-\w+\b/g, '') + ' theme-light';
            ");
        }
        catch
        {
            // Silenciar erros de JS se não estiver disponível
        }
    }

    /// <summary>
    /// Classe CSS - sempre tema claro
    /// </summary>
    public static string GetThemeClass() => "theme-light";

    /// <summary>
    /// Ícone - sempre sol (tema claro ativo)
    /// </summary>
    public static string GetThemeToggleIcon() => "☀️";

    /// <summary>
    /// Nome - sempre tema claro
    /// </summary>
    public static string GetThemeDisplayName() => "Tema Claro";

    /// <summary>
    /// Cor primária - tema claro
    /// </summary>
    public static string GetPrimaryColor() => "#1976d2";

    /// <summary>
    /// Cor de fundo - tema claro
    /// </summary>
    public static string GetBackgroundColor() => "#ffffff";

    /// <summary>
    /// Cor do texto - tema claro
    /// </summary>
    public static string GetTextColor() => "#212121";
}
