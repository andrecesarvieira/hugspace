using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.BlazorApp.Services;

public interface IDepartmentService
{
    /// <summary>
    /// Obtém lista de departamentos
    /// </summary>
    Task<List<DepartmentDto>> GetDepartmentsAsync();

    /// <summary>
    /// Obtém departamentos com paginação
    /// </summary>
    Task<PagedResult<DepartmentDto>> GetPagedDepartmentsAsync(int page = 1, int pageSize = 20, string? searchTerm = null);

    /// <summary>
    /// Obtém departamento por ID
    /// </summary>
    Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id);

    /// <summary>
    /// Obtém hierarquia de departamentos
    /// </summary>
    Task<List<DepartmentHierarchyDto>> GetDepartmentHierarchyAsync();

    /// <summary>
    /// Cria novo departamento
    /// </summary>
    Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request);

    /// <summary>
    /// Atualiza departamento existente
    /// </summary>
    Task<DepartmentDto?> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequest request);

    /// <summary>
    /// Remove departamento
    /// </summary>
    Task<bool> DeleteDepartmentAsync(Guid id);
}
