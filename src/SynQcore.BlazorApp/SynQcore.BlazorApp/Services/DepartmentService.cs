using System.Net.Http.Json;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Common.DTOs;
using System.Text.Json;

namespace SynQcore.BlazorApp.Services;

public partial class DepartmentService : IDepartmentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DepartmentService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Obtendo departamentos")]
    private static partial void LogGetDepartmentsStarted(ILogger logger, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter departamentos")]
    private static partial void LogGetDepartmentsError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo departamento por ID: {departmentId}")]
    private static partial void LogGetDepartmentByIdStarted(ILogger logger, Guid departmentId, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter departamento por ID: {departmentId}")]
    private static partial void LogGetDepartmentByIdError(ILogger logger, Guid departmentId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Obtendo hierarquia de departamentos")]
    private static partial void LogGetHierarchyStarted(ILogger logger, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao obter hierarquia de departamentos")]
    private static partial void LogGetHierarchyError(ILogger logger, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Criando departamento: {name}")]
    private static partial void LogCreateDepartmentStarted(ILogger logger, string name, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao criar departamento: {name}")]
    private static partial void LogCreateDepartmentError(ILogger logger, string name, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Atualizando departamento: {departmentId}")]
    private static partial void LogUpdateDepartmentStarted(ILogger logger, Guid departmentId, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao atualizar departamento: {departmentId}")]
    private static partial void LogUpdateDepartmentError(ILogger logger, Guid departmentId, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Removendo departamento: {departmentId}")]
    private static partial void LogDeleteDepartmentStarted(ILogger logger, Guid departmentId, Exception? exception = null);

    [LoggerMessage(LogLevel.Error, "Erro ao remover departamento: {departmentId}")]
    private static partial void LogDeleteDepartmentError(ILogger logger, Guid departmentId, Exception? exception);

    public DepartmentService(HttpClient httpClient, ILogger<DepartmentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<DepartmentDto>> GetDepartmentsAsync()
    {
        try
        {
            LogGetDepartmentsStarted(_logger);

            var response = await _httpClient.GetAsync("/api/departments");
            response.EnsureSuccessStatusCode();

            var departments = await response.Content.ReadFromJsonAsync<List<DepartmentDto>>(_jsonOptions);
            return departments ?? new List<DepartmentDto>();
        }
        catch (Exception ex)
        {
            LogGetDepartmentsError(_logger, ex);
            return new List<DepartmentDto>();
        }
    }

    public async Task<PagedResult<DepartmentDto>> GetPagedDepartmentsAsync(int page = 1, int pageSize = 20, string? searchTerm = null)
    {
        try
        {
            LogGetDepartmentsStarted(_logger);

            var url = $"/api/departments/paged?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<DepartmentDto>>(_jsonOptions);
            return result ?? new PagedResult<DepartmentDto>();
        }
        catch (Exception ex)
        {
            LogGetDepartmentsError(_logger, ex);
            return new PagedResult<DepartmentDto>();
        }
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id)
    {
        try
        {
            LogGetDepartmentByIdStarted(_logger, id);

            var response = await _httpClient.GetAsync($"/api/departments/{id}");
            response.EnsureSuccessStatusCode();

            var department = await response.Content.ReadFromJsonAsync<DepartmentDto>(_jsonOptions);
            return department;
        }
        catch (Exception ex)
        {
            LogGetDepartmentByIdError(_logger, id, ex);
            return null;
        }
    }

    public async Task<List<DepartmentHierarchyDto>> GetDepartmentHierarchyAsync()
    {
        try
        {
            LogGetHierarchyStarted(_logger);

            var response = await _httpClient.GetAsync("/api/departments/hierarchy");
            response.EnsureSuccessStatusCode();

            var hierarchy = await response.Content.ReadFromJsonAsync<List<DepartmentHierarchyDto>>(_jsonOptions);
            return hierarchy ?? new List<DepartmentHierarchyDto>();
        }
        catch (Exception ex)
        {
            LogGetHierarchyError(_logger, ex);
            return new List<DepartmentHierarchyDto>();
        }
    }

    public async Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        try
        {
            LogCreateDepartmentStarted(_logger, request.Name);

            var response = await _httpClient.PostAsJsonAsync("/api/departments", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var department = await response.Content.ReadFromJsonAsync<DepartmentDto>(_jsonOptions);
            return department;
        }
        catch (Exception ex)
        {
            LogCreateDepartmentError(_logger, request.Name, ex);
            return null;
        }
    }

    public async Task<DepartmentDto?> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequest request)
    {
        try
        {
            LogUpdateDepartmentStarted(_logger, id);

            var response = await _httpClient.PutAsJsonAsync($"/api/departments/{id}", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var department = await response.Content.ReadFromJsonAsync<DepartmentDto>(_jsonOptions);
            return department;
        }
        catch (Exception ex)
        {
            LogUpdateDepartmentError(_logger, id, ex);
            return null;
        }
    }

    public async Task<bool> DeleteDepartmentAsync(Guid id)
    {
        try
        {
            LogDeleteDepartmentStarted(_logger, id);

            var response = await _httpClient.DeleteAsync($"/api/departments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LogDeleteDepartmentError(_logger, id, ex);
            return false;
        }
    }
}