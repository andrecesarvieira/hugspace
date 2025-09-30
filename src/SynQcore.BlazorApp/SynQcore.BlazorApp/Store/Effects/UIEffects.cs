using Fluxor;
using Microsoft.JSInterop;
using SynQcore.BlazorApp.Store.UI;

namespace SynQcore.BlazorApp.Store.Effects;

/// <summary>
/// Effects para o estado da UI
/// </summary>
public class UIEffects
{
    private readonly IJSRuntime _jsRuntime;

    public UIEffects(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Effect para adicionar notificação - configura auto-hide
    /// </summary>
    [EffectMethod]
    public Task HandleAddNotificationAction(UIActions.AddNotificationAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.AutoHideAfterMs.HasValue && action.AutoHideAfterMs > 0)
            {
                // Agenda remoção automática da notificação
                _ = Task.Delay(action.AutoHideAfterMs.Value).ContinueWith(_ =>
                {
                    // O auto-hide será implementado no componente de notificação
                });
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao processar notificação: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Effect para alterar tema - aplica no DOM
    /// </summary>
    [EffectMethod]
    public async Task HandleSetThemeAction(UIActions.SetThemeAction action, IDispatcher dispatcher)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", action.Theme);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "synqcore_theme", action.Theme);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao aplicar tema: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para abrir modal - foca no modal
    /// </summary>
    [EffectMethod]
    public async Task HandleOpenModalAction(UIActions.OpenModalAction action, IDispatcher dispatcher)
    {
        try
        {
            // Adiciona classe para prevenir scroll da página
            await _jsRuntime.InvokeVoidAsync("document.body.classList.add", "modal-open");

            // Foca no modal após um pequeno delay
            await Task.Delay(100);
            await _jsRuntime.InvokeVoidAsync("eval", $"document.querySelector('[data-modal=\"{action.ModalName}\"]')?.focus()");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao abrir modal: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para fechar modal - remove classes e foco
    /// </summary>
    [EffectMethod]
    public async Task HandleCloseModalAction(UIActions.CloseModalAction action, IDispatcher dispatcher)
    {
        try
        {
            // Remove classe que previne scroll
            await _jsRuntime.InvokeVoidAsync("document.body.classList.remove", "modal-open");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao fechar modal: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para toggle sidebar - persiste preferência
    /// </summary>
    [EffectMethod]
    public Task HandleToggleSidebarAction(UIActions.ToggleSidebarAction action, IDispatcher dispatcher)
    {
        try
        {
            // A preferência será salva quando o estado for atualizado
            // Aqui podemos adicionar animações ou outros efeitos visuais

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao alternar sidebar: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Effect para mostrar mensagem de sucesso - adiciona som se habilitado
    /// </summary>
    [EffectMethod]
    public async Task HandleShowSuccessMessageAction(UIActions.ShowSuccessMessageAction action, IDispatcher dispatcher)
    {
        try
        {
            // Aqui podemos adicionar som de sucesso ou outras interações
            await _jsRuntime.InvokeVoidAsync("eval", "console.log('✅ Sucesso:', arguments[0])", action.Message);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao mostrar sucesso: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para mostrar mensagem de erro - adiciona som se habilitado
    /// </summary>
    [EffectMethod]
    public async Task HandleShowErrorMessageAction(UIActions.ShowErrorMessageAction action, IDispatcher dispatcher)
    {
        try
        {
            // Log de erro no console do navegador
            await _jsRuntime.InvokeVoidAsync("eval", "console.error('❌ Erro:', arguments[0])", action.Message);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao mostrar erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Effect para atualizar configurações de acessibilidade - aplica no DOM
    /// </summary>
    [EffectMethod]
    public async Task HandleUpdateAccessibilitySettingsAction(UIActions.UpdateAccessibilitySettingsAction action, IDispatcher dispatcher)
    {
        try
        {
            var settings = action.Settings;

            // Aplica configurações de acessibilidade
            if (settings.HighContrast)
            {
                await _jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "high-contrast");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "high-contrast");
            }

            if (settings.ReducedMotion)
            {
                await _jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "reduced-motion");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "reduced-motion");
            }

            // Aplica escala de fonte
            await _jsRuntime.InvokeVoidAsync("document.documentElement.style.setProperty",
                "--font-size-scale", settings.FontSizeScale.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UIEffects] Erro ao aplicar acessibilidade: {ex.Message}");
        }
    }
}
