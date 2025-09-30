using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Fluxor;
using SynQcore.BlazorApp.Store.User;
using SynQcore.BlazorApp.Store.UI;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço para comunicação com a API SynQcore
/// </summary>
public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    Task<string?> GetAsync(string endpoint);
    Task<bool> PostAsync(string endpoint, object data);
    void SetAuthorizationHeader(string token);
    void ClearAuthorizationHeader();
}

/// <summary>
/// Implementação do serviço de API
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IDispatcher _dispatcher;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, IDispatcher dispatcher)
    {
        _httpClient = httpClient;
        _dispatcher = dispatcher;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        // Configurações padrão
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _httpClient.DefaultRequestHeaders.Add("User-Agent", "SynQcore-Blazor/1.0");
    }

    /// <summary>
    /// Realiza requisição GET e retorna objeto tipado
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(true));
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connecting));

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);

                _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connected));
                return result;
            }
            else
            {
                await HandleErrorResponse(response);
                return default;
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex, $"GET {endpoint}");
            return default;
        }
        finally
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(false));
        }
    }

    /// <summary>
    /// Realiza requisição GET e retorna string
    /// </summary>
    public async Task<string?> GetAsync(string endpoint)
    {
        try
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(true));
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connecting));

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connected));
                return content;
            }
            else
            {
                await HandleErrorResponse(response);
                return null;
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex, $"GET {endpoint}");
            return null;
        }
        finally
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(false));
        }
    }

    /// <summary>
    /// Realiza requisição POST e retorna objeto tipado
    /// </summary>
    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(true));
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connecting));

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);

                _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connected));
                return result;
            }
            else
            {
                await HandleErrorResponse(response);
                return default;
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex, $"POST {endpoint}");
            return default;
        }
        finally
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(false));
        }
    }

    /// <summary>
    /// Realiza requisição POST sem retorno tipado
    /// </summary>
    public async Task<bool> PostAsync(string endpoint, object data)
    {
        try
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(true));
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connecting));

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connected));
                return true;
            }
            else
            {
                await HandleErrorResponse(response);
                return false;
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex, $"POST {endpoint}");
            return false;
        }
        finally
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(false));
        }
    }

    /// <summary>
    /// Realiza requisição PUT
    /// </summary>
    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(true));
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connecting));

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);

                _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connected));
                return result;
            }
            else
            {
                await HandleErrorResponse(response);
                return default;
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex, $"PUT {endpoint}");
            return default;
        }
        finally
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(false));
        }
    }

    /// <summary>
    /// Realiza requisição DELETE
    /// </summary>
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(true));
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connecting));

            var response = await _httpClient.DeleteAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Connected));
                return true;
            }
            else
            {
                await HandleErrorResponse(response);
                return false;
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex, $"DELETE {endpoint}");
            return false;
        }
        finally
        {
            _dispatcher.Dispatch(new UIActions.SetLoadingAction(false));
        }
    }

    /// <summary>
    /// Define o header de autorização
    /// </summary>
    public void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Remove o header de autorização
    /// </summary>
    public void ClearAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Trata erros de resposta HTTP
    /// </summary>
    private async Task HandleErrorResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var errorMessage = $"Erro {(int)response.StatusCode}: {response.ReasonPhrase}";

        if (!string.IsNullOrEmpty(content))
        {
            try
            {
                var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(content, _jsonOptions);
                if (errorData?.ContainsKey("message") == true)
                {
                    errorMessage = errorData["message"].ToString() ?? errorMessage;
                }
            }
            catch
            {
                errorMessage = content.Length > 200 ? content[..200] + "..." : content;
            }
        }

        _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Error));
        _dispatcher.Dispatch(new UIActions.ShowErrorMessageAction("Erro de API", errorMessage));

        // Se for erro de autenticação, limpa a sessão
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _dispatcher.Dispatch(new UserActions.LogoutAction());
        }
    }

    /// <summary>
    /// Trata exceções
    /// </summary>
    private async Task HandleException(Exception ex, string operation)
    {
        await Task.Delay(1); // Para manter método async

        var errorMessage = ex.Message;

        if (ex is HttpRequestException)
        {
            errorMessage = "Erro de conexão com o servidor";
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Disconnected));
        }
        else if (ex is TaskCanceledException)
        {
            errorMessage = "Operação cancelada ou timeout";
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Disconnected));
        }
        else
        {
            _dispatcher.Dispatch(new UIActions.SetApiConnectionStatusAction(ConnectionStatus.Error));
        }

        _dispatcher.Dispatch(new UIActions.ShowErrorMessageAction($"Erro em {operation}", errorMessage));

        Console.WriteLine($"[ApiService] Erro em {operation}: {ex.Message}");
    }
}
