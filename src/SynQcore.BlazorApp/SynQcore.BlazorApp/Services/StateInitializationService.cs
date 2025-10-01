using Fluxor;
using Microsoft.JSInterop;
using SynQcore.BlazorApp.Store.User;
using SynQcore.BlazorApp.Store.UI;
using System.Text.Json;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço para inicializar o estado da aplicação
/// </summary>
public interface IStateInitializationService
{
    Task InitializeAsync();
    Task InitializeAfterRenderAsync();
}

/// <summary>
/// Implementação do serviço de inicialização do estado
/// </summary>
public class StateInitializationService : IStateInitializationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IDispatcher _dispatcher;
    private bool _hasInitialized;

    public StateInitializationService(IJSRuntime jsRuntime, IDispatcher dispatcher)
    {
        _jsRuntime = jsRuntime;
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Inicializa o estado da aplicação carregando dados persistidos
    /// </summary>
    public async Task InitializeAsync()
    {
        // Durante pre-rendering, apenas marca como pronto
        await Task.CompletedTask;
    }

    /// <summary>
    /// Inicializa após primeira renderização quando JavaScript está disponível
    /// </summary>
    public async Task InitializeAfterRenderAsync()
    {
        if (_hasInitialized) return;

        _hasInitialized = true;

        try
        {
            await InitializeUserStateAsync();
            await InitializeUIStateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StateInitialization] Erro na inicialização: {ex.Message}");
        }
    }

    /// <summary>
    /// Inicializa o estado do usuário
    /// </summary>
    private async Task InitializeUserStateAsync()
    {
        try
        {
            // Verifica se há dados de usuário salvos
            var userJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "synqcore_user");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonSerializer.Deserialize<UserInfo>(userJson);
                if (user != null)
                {
                    // Verifica se há token válido
                    var accessToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "synqcore_access_token");
                    var refreshToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "synqcore_refresh_token");
                    var expiresAtString = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "synqcore_token_expires");

                    DateTime? expiresAt = null;
                    if (!string.IsNullOrEmpty(expiresAtString) && DateTime.TryParse(expiresAtString, out var parsedDate))
                    {
                        expiresAt = parsedDate;
                    }

                    // Se o token ainda é válido, faz login automaticamente
                    if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken) &&
                        expiresAt.HasValue && expiresAt.Value > DateTime.UtcNow)
                    {
                        _dispatcher.Dispatch(new UserActions.LoginSuccessAction(
                            user, accessToken, refreshToken, expiresAt.Value));
                    }
                    else
                    {
                        // Token expirado, fazer logout
                        await ClearUserDataAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StateInitialization] Erro ao inicializar estado do usuário: {ex.Message}");
            // Em caso de erro, limpa os dados potencialmente corrompidos
            await ClearUserDataAsync();
        }
    }

    /// <summary>
    /// Inicializa o estado da UI
    /// </summary>
    private async Task InitializeUIStateAsync()
    {
        try
        {
            // Carrega preferências da UI (sidebar apenas)
            var sidebarCollapsedJson = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "synqcore_ui_sidebar_collapsed");
            if (!string.IsNullOrEmpty(sidebarCollapsedJson) && bool.TryParse(sidebarCollapsedJson, out var isCollapsed))
            {
                // Se estava recolhido, alterna para recolher
                if (isCollapsed)
                {
                    _dispatcher.Dispatch(new UIActions.ToggleSidebarAction());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StateInitialization] Erro ao inicializar estado da UI: {ex.Message}");
        }
    }

    /// <summary>
    /// Limpa dados do usuário do localStorage
    /// </summary>
    private async Task ClearUserDataAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_user");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_access_token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_refresh_token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "synqcore_token_expires");
            _dispatcher.Dispatch(new UserActions.LogoutAction());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StateInitialization] Erro ao limpar dados do usuário: {ex.Message}");
        }
    }
}
