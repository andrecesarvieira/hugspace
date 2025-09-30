using Microsoft.JSInterop;

namespace SynQcore.BlazorApp.Client.Services;

/// <summary>
/// Serviço para gerenciamento de temas corporativos (claro/escuro)
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _themeKey = "synqcore-theme";
    private string _currentTheme = "light";

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Evento disparado quando o tema é alterado
    /// </summary>
    public event Action<string>? ThemeChanged;

    /// <summary>
    /// Tema atual
    /// </summary>
    public string CurrentTheme => _currentTheme;

    /// <summary>
    /// Verifica se o tema atual é escuro
    /// </summary>
    public bool IsDarkTheme => _currentTheme == "dark";

    /// <summary>
    /// Inicializa o serviço de tema carregando o tema salvo
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Tentar carregar tema salvo do localStorage
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", _themeKey);

            if (!string.IsNullOrEmpty(savedTheme) && (savedTheme == "light" || savedTheme == "dark"))
            {
                _currentTheme = savedTheme;
            }
            else
            {
                // Detectar preferência do sistema
                var prefersDark = await _jsRuntime.InvokeAsync<bool>("window.matchMedia", "(prefers-color-scheme: dark)");
                _currentTheme = prefersDark ? "dark" : "light";
            }

            await ApplyThemeAsync(_currentTheme);
        }
        catch
        {
            // Em caso de erro, usar tema claro por padrão
            _currentTheme = "light";
            await ApplyThemeAsync(_currentTheme);
        }
    }

    /// <summary>
    /// Alterna entre tema claro e escuro
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        var newTheme = _currentTheme == "light" ? "dark" : "light";
        await SetThemeAsync(newTheme);
    }

    /// <summary>
    /// Define um tema específico
    /// </summary>
    public async Task SetThemeAsync(string theme)
    {
        if (theme != "light" && theme != "dark")
            throw new ArgumentException("Tema deve ser 'light' ou 'dark'", nameof(theme));

        if (_currentTheme == theme)
            return;

        _currentTheme = theme;

        await ApplyThemeAsync(theme);
        await SaveThemeAsync(theme);

        ThemeChanged?.Invoke(theme);
    }

    /// <summary>
    /// Aplica o tema no DOM
    /// </summary>
    private async Task ApplyThemeAsync(string theme)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                document.documentElement.setAttribute('data-theme', '{theme}');
                document.body.className = document.body.className.replace(/\btheme-\w+\b/g, '') + ' theme-{theme}';
            ");
        }
        catch
        {
            // Silenciar erros de JS se não estiver disponível
        }
    }

    /// <summary>
    /// Salva o tema no localStorage
    /// </summary>
    private async Task SaveThemeAsync(string theme)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _themeKey, theme);
        }
        catch
        {
            // Silenciar erros se localStorage não estiver disponível
        }
    }

    /// <summary>
    /// Obtém a classe CSS para o tema atual
    /// </summary>
    public string GetThemeClass() => $"theme-{_currentTheme}";

    /// <summary>
    /// Obtém o ícone apropriado para o botão de alternância de tema
    /// </summary>
    public string GetThemeToggleIcon() => _currentTheme == "light" ? "🌙" : "☀️";

    /// <summary>
    /// Obtém o texto descritivo do tema atual
    /// </summary>
    public string GetThemeDisplayName() => _currentTheme == "light" ? "Tema Claro" : "Tema Escuro";

    /// <summary>
    /// Obtém a cor primária do tema atual
    /// </summary>
    public string GetPrimaryColor() => _currentTheme == "light" ? "#1976d2" : "#42a5f5";

    /// <summary>
    /// Obtém a cor de fundo do tema atual
    /// </summary>
    public string GetBackgroundColor() => _currentTheme == "light" ? "#fafafa" : "#1a1a1a";

    /// <summary>
    /// Obtém a cor do texto do tema atual
    /// </summary>
    public string GetTextColor() => _currentTheme == "light" ? "#212121" : "#ffffff";
}
