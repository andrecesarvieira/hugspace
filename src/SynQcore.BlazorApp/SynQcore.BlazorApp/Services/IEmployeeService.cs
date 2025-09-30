using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using SynQcore.BlazorApp.Models;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço para gerenciamento de funcionários
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Obtém lista paginada de funcionários
    /// </summary>
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeSearchRequest request);

    /// <summary>
    /// Obtém funcionário específico por ID
    /// </summary>
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);

    /// <summary>
    /// Cria novo funcionário
    /// </summary>
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);

    /// <summary>
    /// Atualiza dados do funcionário
    /// </summary>
    Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request);

    /// <summary>
    /// Remove funcionário (soft delete)
    /// </summary>
    Task<bool> DeleteEmployeeAsync(Guid id);

    /// <summary>
    /// Busca funcionários por termo
    /// </summary>
    Task<List<EmployeeDto>> SearchEmployeesAsync(string searchTerm);

    /// <summary>
    /// Obtém hierarquia organizacional do funcionário
    /// </summary>
    Task<EmployeeHierarchyDto?> GetEmployeeHierarchyAsync(Guid id);

    /// <summary>
    /// Faz upload do avatar do funcionário
    /// </summary>
    Task<string> UploadEmployeeAvatarAsync(Guid id, IBrowserFile file);
}

/// <summary>
/// DTO para funcionário
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public List<EmployeeDepartmentDto> Departments { get; set; } = new();
    public List<TeamDto> Teams { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public string DisplayName => string.IsNullOrEmpty(FullName.Trim()) ? Email : FullName;
}

/// <summary>
/// DTO para departamento do funcionário
/// </summary>
public class EmployeeDepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO para equipe do funcionário
/// </summary>
public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Role { get; set; } = "Member";
}

/// <summary>
/// DTO para hierarquia organizacional
/// </summary>
public class EmployeeHierarchyDto
{
    public EmployeeDto Employee { get; set; } = null!;
    public EmployeeDto? Manager { get; set; }
    public List<EmployeeDto> Subordinates { get; set; } = new();
    public List<EmployeeDto> Peers { get; set; } = new();
}

/// <summary>
/// Request para busca de funcionários
/// </summary>
public class EmployeeSearchRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? HireDateFrom { get; set; }
    public DateTime? HireDateTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

/// <summary>
/// Request para criação de funcionário
/// </summary>
public class CreateEmployeeRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sobrenome é obrigatório")]
    [StringLength(50, ErrorMessage = "Sobrenome deve ter no máximo 50 caracteres")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Data de admissão é obrigatória")]
    public DateTime HireDate { get; set; } = DateTime.Now;

    public Guid? ManagerId { get; set; }
    public List<Guid> DepartmentIds { get; set; } = new();
    public List<Guid> TeamIds { get; set; } = new();
}

/// <summary>
/// Request para atualização de funcionário
/// </summary>
public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ManagerId { get; set; }
    public List<Guid> DepartmentIds { get; set; } = new();
    public List<Guid> TeamIds { get; set; } = new();
}
