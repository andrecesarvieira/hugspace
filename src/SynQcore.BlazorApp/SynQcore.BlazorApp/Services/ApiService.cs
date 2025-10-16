using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SynQcore.BlazorApp.Services.StateManagement;

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
/// Implementação do serviço de API usando StateManager
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly StateManager _stateManager;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, StateManager stateManager)
    {
        _httpClient = httpClient;
        _stateManager = stateManager;

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
            _stateManager.UI.IsLoading = true;
            Console.WriteLine($"🌐 [ApiService] GET {endpoint}");

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);

                Console.WriteLine($"✅ [ApiService] GET {endpoint} - Sucesso");
                return result;
            }
            else
            {
                await HandleErrorResponse(response, $"GET {endpoint}");
                return default;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"GET {endpoint}");
            return default;
        }
        finally
        {
            _stateManager.UI.IsLoading = false;
        }
    }

    /// <summary>
    /// Realiza requisição GET e retorna string
    /// </summary>
    public async Task<string?> GetAsync(string endpoint)
    {
        try
        {
            _stateManager.UI.IsLoading = true;
            Console.WriteLine($"🌐 [ApiService] GET {endpoint}");

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ [ApiService] GET {endpoint} - Sucesso");
                return content;
            }
            else
            {
                await HandleErrorResponse(response, $"GET {endpoint}");
                return null;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"GET {endpoint}");
            return null;
        }
        finally
        {
            _stateManager.UI.IsLoading = false;
        }
    }

    /// <summary>
    /// Realiza requisição POST e retorna objeto tipado
    /// </summary>
    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            _stateManager.UI.IsLoading = true;
            Console.WriteLine($"🌐 [ApiService] POST {endpoint} (URL: {_httpClient.BaseAddress}{endpoint})");

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"📤 [ApiService] Enviando requisição para {_httpClient.BaseAddress}{endpoint}...");
            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);

                Console.WriteLine($"✅ [ApiService] POST {endpoint} - Sucesso");
                return result;
            }
            else
            {
                await HandleErrorResponse(response, $"POST {endpoint}");
                return default;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"POST {endpoint}");
            return default;
        }
        finally
        {
            _stateManager.UI.IsLoading = false;
        }
    }

    /// <summary>
    /// Realiza requisição POST sem retorno tipado
    /// </summary>
    public async Task<bool> PostAsync(string endpoint, object data)
    {
        try
        {
            _stateManager.UI.IsLoading = true;
            Console.WriteLine($"🌐 [ApiService] POST {endpoint}");

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ [ApiService] POST {endpoint} - Sucesso");
                return true;
            }
            else
            {
                await HandleErrorResponse(response, $"POST {endpoint}");
                return false;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"POST {endpoint}");
            return false;
        }
        finally
        {
            _stateManager.UI.IsLoading = false;
        }
    }

    /// <summary>
    /// Realiza requisição PUT
    /// </summary>
    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            _stateManager.UI.IsLoading = true;
            Console.WriteLine($"🌐 [ApiService] PUT {endpoint}");

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);

                Console.WriteLine($"✅ [ApiService] PUT {endpoint} - Sucesso");
                return result;
            }
            else
            {
                await HandleErrorResponse(response, $"PUT {endpoint}");
                return default;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"PUT {endpoint}");
            return default;
        }
        finally
        {
            _stateManager.UI.IsLoading = false;
        }
    }

    /// <summary>
    /// Realiza requisição DELETE
    /// </summary>
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            _stateManager.UI.IsLoading = true;
            Console.WriteLine($"🌐 [ApiService] DELETE {endpoint}");

            var response = await _httpClient.DeleteAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ [ApiService] DELETE {endpoint} - Sucesso");
                return true;
            }
            else
            {
                await HandleErrorResponse(response, $"DELETE {endpoint}");
                return false;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"DELETE {endpoint}");
            return false;
        }
        finally
        {
            _stateManager.UI.IsLoading = false;
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
    private async Task HandleErrorResponse(HttpResponseMessage response, string operation)
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

        Console.WriteLine($"❌ [ApiService] Erro {operation}: {errorMessage}");

        // Se for erro de autenticação, fazer logout
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine($"🚫 [ApiService] Erro 401 - Fazendo logout");
            _stateManager.User.Logout();
        }
    }

    /// <summary>
    /// Trata exceções
    /// </summary>
    private static void HandleException(Exception ex, string operation)
    {
        var errorMessage = ex.Message;

        if (ex is HttpRequestException)
        {
            errorMessage = "Erro de conexão com o servidor";
        }
        else if (ex is TaskCanceledException)
        {
            errorMessage = "Operação cancelada ou timeout";
        }

        Console.WriteLine($"❌ [ApiService] Erro em {operation}: {errorMessage}");
    }
}
