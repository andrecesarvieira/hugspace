using Fluxor;
using Microsoft.JSInterop;
using SynQcore.BlazorApp.Store.User;
using System.Text.Json;

namespace SynQcore.BlazorApp.Store.Effects;

/// <summary>
/// Effects para o estado do usuário
/// </summary>
public class UserEffects
{
    private readonly IJSRuntime _jsRuntime;

    public UserEffects(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Effect para login bem-sucedido - persiste dados no localStorage
    /// </summary>
    [EffectMethod]
    public async Task HandleLoginSuccessAction(UserActions.LoginSuccessAction action, IDispatcher dispatcher)
    {
        try
        {
            // Persiste token no localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_access_token", action.AccessToken);

            if (action.RefreshToken != null)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_refresh_token", action.RefreshToken);
            }

            if (action.ExpiresAt.HasValue)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_token_expires", action.ExpiresAt.Value.ToString("O"));
            }

            // Persiste dados do usuário
            var userJson = JsonSerializer.Serialize(action.User);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_user", userJson);

            Console.WriteLine($"[UserEffects] Login bem-sucedido para {action.User.Email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserEffects] Erro ao persistir dados do login: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para logout - limpa dados do localStorage
    /// </summary>
    [EffectMethod]
    public async Task HandleLogoutAction(UserActions.LogoutAction action, IDispatcher dispatcher)
    {
        try
        {
            // Remove tokens e dados do usuário
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_access_token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_refresh_token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_token_expires");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_user");

            Console.WriteLine("[UserEffects] Logout realizado - dados limpos do localStorage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserEffects] Erro ao limpar dados do logout: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para atualizar preferências - persiste no localStorage
    /// </summary>
    [EffectMethod]
    public async Task HandleUpdateUserPreferencesAction(UserActions.UpdateUserPreferencesAction action, IDispatcher dispatcher)
    {
        try
        {
            var preferencesJson = JsonSerializer.Serialize(action.Preferences);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_user_preferences", preferencesJson);

            Console.WriteLine("[UserEffects] Preferências do usuário atualizadas");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserEffects] Erro ao salvar preferências: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para alternar tema - aplica mudanças no DOM
    /// </summary>
    [EffectMethod]
    public async Task HandleToggleThemeAction(UserActions.ToggleThemeAction action, IDispatcher dispatcher)
    {
        try
        {
            // Aplica tema no DOM
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme",
                action.GetType().Name.Contains("dark") ? "dark" : "light");

            Console.WriteLine("[UserEffects] Tema alterado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserEffects] Erro ao aplicar tema: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para refresh token bem-sucedido - atualiza localStorage
    /// </summary>
    [EffectMethod]
    public async Task HandleRefreshTokenSuccessAction(UserActions.RefreshTokenSuccessAction action, IDispatcher dispatcher)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_access_token", action.AccessToken);

            if (action.RefreshToken != null)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_refresh_token", action.RefreshToken);
            }

            if (action.ExpiresAt.HasValue)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_token_expires", action.ExpiresAt.Value.ToString("O"));
            }

            Console.WriteLine("[UserEffects] Token atualizado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserEffects] Erro ao atualizar token: {ex.Message}");
        }
    }
}
