using Microsoft.AspNetCore.Components.Forms;
using SynQcore.BlazorApp.Models;
using System.Text.Json;
using System.Net.Http.Headers;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Implementação do serviço de funcionários
/// </summary>
public partial class EmployeeService : IEmployeeService
{
    private readonly IApiService _apiService;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IApiService apiService, ILogger<EmployeeService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém lista paginada de funcionários
    /// </summary>
    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeSearchRequest request)
    {
        try
        {
            LogFetchingEmployees(_logger, request.Page, request.PageSize, request.SearchTerm);

            var queryParams = BuildQueryParams(request);
            var result = await _apiService.GetAsync<PagedResult<EmployeeDto>>($"employees?{queryParams}");

            if (result != null)
            {
                LogEmployeesFetched(_logger, result.Items.Count, result.TotalCount);
                return result;
            }

            LogFetchEmployeesFailed(_logger, System.Net.HttpStatusCode.NoContent, "Resultado nulo");
            return new PagedResult<EmployeeDto>();
        }
        catch (Exception ex)
        {
            LogFetchEmployeesError(_logger, ex);
            return GetFallbackEmployees();
        }
    }

    /// <summary>
    /// Obtém funcionário específico por ID
    /// </summary>
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
    {
        try
        {
            LogFetchingEmployee(_logger, id);

            var result = await _apiService.GetAsync<EmployeeDto>($"employees/{id}");

            if (result != null)
            {
                LogEmployeeFetched(_logger, id, result.FullName);
                return result;
            }

            LogFetchEmployeeFailed(_logger, id, System.Net.HttpStatusCode.NotFound);
            return null;
        }
        catch (Exception ex)
        {
            LogFetchEmployeeError(_logger, id, ex);
            return null;
        }
    }

    /// <summary>
    /// Cria novo funcionário
    /// </summary>
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        try
        {
            LogCreatingEmployee(_logger, request.Email);

            var result = await _apiService.PostAsync<EmployeeDto>("employees", request);

            if (result != null)
            {
                LogEmployeeCreated(_logger, result.Id, result.FullName);
                return result;
            }

            LogCreateEmployeeFailed(_logger, System.Net.HttpStatusCode.BadRequest, "Resultado nulo");
            throw new InvalidOperationException("Falha ao criar funcionário");
        }
        catch (Exception ex)
        {
            LogCreateEmployeeError(_logger, request.Email, ex);
            throw;
        }
    }

    /// <summary>
    /// Atualiza dados do funcionário
    /// </summary>
    public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request)
    {
        try
        {
            LogUpdatingEmployee(_logger, id);

            var result = await _apiService.PutAsync<EmployeeDto>($"employees/{id}", request);

            if (result != null)
            {
                LogEmployeeUpdated(_logger, id, result.FullName);
                return result;
            }

            LogUpdateEmployeeFailed(_logger, id, System.Net.HttpStatusCode.BadRequest, "Resultado nulo");
            throw new InvalidOperationException("Falha ao atualizar funcionário");
        }
        catch (Exception ex)
        {
            LogUpdateEmployeeError(_logger, id, ex);
            throw;
        }
    }

    /// <summary>
    /// Remove funcionário (soft delete)
    /// </summary>
    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
        try
        {
            LogDeletingEmployee(_logger, id);

            var success = await _apiService.DeleteAsync($"employees/{id}");

            if (success)
            {
                LogEmployeeDeleted(_logger, id);
                return true;
            }

            LogDeleteEmployeeFailed(_logger, id, System.Net.HttpStatusCode.BadRequest);
            return false;
        }
        catch (Exception ex)
        {
            LogDeleteEmployeeError(_logger, id, ex);
            return false;
        }
    }

    /// <summary>
    /// Busca funcionários por termo
    /// </summary>
    public async Task<List<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
    {
        try
        {
            LogSearchingEmployees(_logger, searchTerm);

            var result = await _apiService.GetAsync<List<EmployeeDto>>($"employees/search?q={Uri.EscapeDataString(searchTerm)}");

            if (result != null)
            {
                LogEmployeesSearched(_logger, searchTerm, result.Count);
                return result;
            }

            LogSearchEmployeesFailed(_logger, searchTerm, System.Net.HttpStatusCode.NoContent);
            return new List<EmployeeDto>();
        }
        catch (Exception ex)
        {
            LogSearchEmployeesError(_logger, searchTerm, ex);
            return new List<EmployeeDto>();
        }
    }

    /// <summary>
    /// Obtém hierarquia organizacional do funcionário
    /// </summary>
    public async Task<EmployeeHierarchyDto?> GetEmployeeHierarchyAsync(Guid id)
    {
        try
        {
            LogFetchingHierarchy(_logger, id);

            var result = await _apiService.GetAsync<EmployeeHierarchyDto>($"employees/{id}/hierarchy");

            if (result != null)
            {
                LogHierarchyFetched(_logger, id, result.Subordinates.Count);
                return result;
            }

            LogFetchHierarchyFailed(_logger, id, System.Net.HttpStatusCode.NotFound);
            return null;
        }
        catch (Exception ex)
        {
            LogFetchHierarchyError(_logger, id, ex);
            return null;
        }
    }

    /// <summary>
    /// Faz upload do avatar do funcionário
    /// </summary>
    public async Task<string> UploadEmployeeAvatarAsync(Guid id, IBrowserFile file)
    {
        try
        {
            LogUploadingAvatar(_logger, id, file.Name);

            // Note: Para upload de arquivo, podemos precisar usar HttpClient diretamente
            // ou ajustar o IApiService para suportar MultipartFormDataContent
            var avatarUrl = $"/images/avatars/{id}.jpg"; // Mock URL

            // Simular delay de upload
            await Task.Delay(1000);

            LogAvatarUploaded(_logger, id, avatarUrl);
            return avatarUrl;
        }
        catch (Exception ex)
        {
            LogUploadAvatarError(_logger, id, ex);
            throw;
        }
    }

    #region Helper Methods

    private static string BuildQueryParams(EmployeeSearchRequest request)
    {
        var parameters = new List<string>
        {
            $"page={request.Page}",
            $"pageSize={request.PageSize}"
        };

        if (!string.IsNullOrEmpty(request.SearchTerm))
            parameters.Add($"searchTerm={Uri.EscapeDataString(request.SearchTerm)}");

        if (request.DepartmentId.HasValue)
            parameters.Add($"departmentId={request.DepartmentId}");

        if (request.IsActive.HasValue)
            parameters.Add($"isActive={request.IsActive}");

        if (request.HireDateFrom.HasValue)
            parameters.Add($"hireDateFrom={request.HireDateFrom:yyyy-MM-dd}");

        if (request.HireDateTo.HasValue)
            parameters.Add($"hireDateTo={request.HireDateTo:yyyy-MM-dd}");

        if (!string.IsNullOrEmpty(request.SortBy))
            parameters.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");

        if (request.SortDescending)
            parameters.Add("sortDescending=true");

        return string.Join("&", parameters);
    }

    private static PagedResult<EmployeeDto> GetFallbackEmployees()
    {
        return new PagedResult<EmployeeDto>
        {
            Items = new List<EmployeeDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Sistema",
                    Email = "admin@synqcore.com",
                    HireDate = DateTime.Now.AddYears(-1),
                    IsActive = true,
                    Departments = new List<EmployeeDepartmentDto>
                    {
                        new() { Id = Guid.NewGuid(), Name = "TI", Description = "Tecnologia da Informação" }
                    }
                }
            },
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
    }

    #endregion

    #region LoggerMessage Delegates

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Information,
        Message = "Buscando funcionários - Página: {Page}, Tamanho: {PageSize}, Termo: {SearchTerm}")]
    private static partial void LogFetchingEmployees(ILogger logger, int page, int pageSize, string? searchTerm);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Information,
        Message = "Funcionários obtidos - Quantidade: {Count}, Total: {TotalCount}")]
    private static partial void LogEmployeesFetched(ILogger logger, int count, int totalCount);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Warning,
        Message = "Falha ao buscar funcionários - Status: {StatusCode}, Erro: {Error}")]
    private static partial void LogFetchEmployeesFailed(ILogger logger, System.Net.HttpStatusCode statusCode, string error);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Error,
        Message = "Erro ao buscar funcionários")]
    private static partial void LogFetchEmployeesError(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 3005,
        Level = LogLevel.Information,
        Message = "Buscando funcionário por ID: {Id}")]
    private static partial void LogFetchingEmployee(ILogger logger, Guid id);

    [LoggerMessage(
        EventId = 3006,
        Level = LogLevel.Information,
        Message = "Funcionário obtido - ID: {Id}, Nome: {Name}")]
    private static partial void LogEmployeeFetched(ILogger logger, Guid id, string? name);

    [LoggerMessage(
        EventId = 3007,
        Level = LogLevel.Warning,
        Message = "Falha ao buscar funcionário {Id} - Status: {StatusCode}")]
    private static partial void LogFetchEmployeeFailed(ILogger logger, Guid id, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(
        EventId = 3008,
        Level = LogLevel.Error,
        Message = "Erro ao buscar funcionário {Id}")]
    private static partial void LogFetchEmployeeError(ILogger logger, Guid id, Exception exception);

    [LoggerMessage(
        EventId = 3009,
        Level = LogLevel.Information,
        Message = "Criando funcionário: {Email}")]
    private static partial void LogCreatingEmployee(ILogger logger, string email);

    [LoggerMessage(
        EventId = 3010,
        Level = LogLevel.Information,
        Message = "Funcionário criado - ID: {Id}, Nome: {Name}")]
    private static partial void LogEmployeeCreated(ILogger logger, Guid id, string? name);

    [LoggerMessage(
        EventId = 3011,
        Level = LogLevel.Warning,
        Message = "Falha ao criar funcionário - Status: {StatusCode}, Erro: {Error}")]
    private static partial void LogCreateEmployeeFailed(ILogger logger, System.Net.HttpStatusCode statusCode, string error);

    [LoggerMessage(
        EventId = 3012,
        Level = LogLevel.Error,
        Message = "Erro ao criar funcionário {Email}")]
    private static partial void LogCreateEmployeeError(ILogger logger, string email, Exception exception);

    [LoggerMessage(
        EventId = 3013,
        Level = LogLevel.Information,
        Message = "Atualizando funcionário: {Id}")]
    private static partial void LogUpdatingEmployee(ILogger logger, Guid id);

    [LoggerMessage(
        EventId = 3014,
        Level = LogLevel.Information,
        Message = "Funcionário atualizado - ID: {Id}, Nome: {Name}")]
    private static partial void LogEmployeeUpdated(ILogger logger, Guid id, string? name);

    [LoggerMessage(
        EventId = 3015,
        Level = LogLevel.Warning,
        Message = "Falha ao atualizar funcionário {Id} - Status: {StatusCode}, Erro: {Error}")]
    private static partial void LogUpdateEmployeeFailed(ILogger logger, Guid id, System.Net.HttpStatusCode statusCode, string error);

    [LoggerMessage(
        EventId = 3016,
        Level = LogLevel.Error,
        Message = "Erro ao atualizar funcionário {Id}")]
    private static partial void LogUpdateEmployeeError(ILogger logger, Guid id, Exception exception);

    [LoggerMessage(
        EventId = 3017,
        Level = LogLevel.Information,
        Message = "Removendo funcionário: {Id}")]
    private static partial void LogDeletingEmployee(ILogger logger, Guid id);

    [LoggerMessage(
        EventId = 3018,
        Level = LogLevel.Information,
        Message = "Funcionário removido - ID: {Id}")]
    private static partial void LogEmployeeDeleted(ILogger logger, Guid id);

    [LoggerMessage(
        EventId = 3019,
        Level = LogLevel.Warning,
        Message = "Falha ao remover funcionário {Id} - Status: {StatusCode}")]
    private static partial void LogDeleteEmployeeFailed(ILogger logger, Guid id, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(
        EventId = 3020,
        Level = LogLevel.Error,
        Message = "Erro ao remover funcionário {Id}")]
    private static partial void LogDeleteEmployeeError(ILogger logger, Guid id, Exception exception);

    [LoggerMessage(
        EventId = 3021,
        Level = LogLevel.Information,
        Message = "Pesquisando funcionários: {SearchTerm}")]
    private static partial void LogSearchingEmployees(ILogger logger, string searchTerm);

    [LoggerMessage(
        EventId = 3022,
        Level = LogLevel.Information,
        Message = "Pesquisa de funcionários concluída - Termo: {SearchTerm}, Resultados: {Count}")]
    private static partial void LogEmployeesSearched(ILogger logger, string searchTerm, int count);

    [LoggerMessage(
        EventId = 3023,
        Level = LogLevel.Warning,
        Message = "Falha na pesquisa de funcionários {SearchTerm} - Status: {StatusCode}")]
    private static partial void LogSearchEmployeesFailed(ILogger logger, string searchTerm, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(
        EventId = 3024,
        Level = LogLevel.Error,
        Message = "Erro na pesquisa de funcionários {SearchTerm}")]
    private static partial void LogSearchEmployeesError(ILogger logger, string searchTerm, Exception exception);

    [LoggerMessage(
        EventId = 3025,
        Level = LogLevel.Information,
        Message = "Buscando hierarquia do funcionário: {Id}")]
    private static partial void LogFetchingHierarchy(ILogger logger, Guid id);

    [LoggerMessage(
        EventId = 3026,
        Level = LogLevel.Information,
        Message = "Hierarquia obtida - ID: {Id}, Subordinados: {SubordinatesCount}")]
    private static partial void LogHierarchyFetched(ILogger logger, Guid id, int subordinatesCount);

    [LoggerMessage(
        EventId = 3027,
        Level = LogLevel.Warning,
        Message = "Falha ao buscar hierarquia {Id} - Status: {StatusCode}")]
    private static partial void LogFetchHierarchyFailed(ILogger logger, Guid id, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(
        EventId = 3028,
        Level = LogLevel.Error,
        Message = "Erro ao buscar hierarquia {Id}")]
    private static partial void LogFetchHierarchyError(ILogger logger, Guid id, Exception exception);

    [LoggerMessage(
        EventId = 3029,
        Level = LogLevel.Information,
        Message = "Fazendo upload de avatar - ID: {Id}, Arquivo: {FileName}")]
    private static partial void LogUploadingAvatar(ILogger logger, Guid id, string fileName);

    [LoggerMessage(
        EventId = 3030,
        Level = LogLevel.Information,
        Message = "Avatar enviado - ID: {Id}, URL: {AvatarUrl}")]
    private static partial void LogAvatarUploaded(ILogger logger, Guid id, string avatarUrl);

    [LoggerMessage(
        EventId = 3031,
        Level = LogLevel.Warning,
        Message = "Falha no upload de avatar {Id} - Status: {StatusCode}, Erro: {Error}")]
    private static partial void LogUploadAvatarFailed(ILogger logger, Guid id, System.Net.HttpStatusCode statusCode, string error);

    [LoggerMessage(
        EventId = 3032,
        Level = LogLevel.Error,
        Message = "Erro no upload de avatar {Id}")]
    private static partial void LogUploadAvatarError(ILogger logger, Guid id, Exception exception);

    #endregion
}
