namespace SynQcore.Application.Features.Departments.DTOs;

/// <summary>
/// DTO para visualização da hierarquia completa de departamentos.
/// Inclui estrutura organizacional, estatísticas e navegação hierárquica.
/// </summary>
public class DepartmentHierarchyDto
{
    /// <summary>
    /// Identificador único do departamento.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do departamento.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do departamento.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indica se o departamento está ativo.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Nível hierárquico do departamento (0 = raiz).
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Caminho hierárquico completo do departamento.
    /// </summary>
    public string HierarchyPath { get; set; } = string.Empty;

    /// <summary>
    /// Número de funcionários diretamente vinculados ao departamento.
    /// </summary>
    public int DirectEmployeesCount { get; set; }

    /// <summary>
    /// Total de funcionários incluindo subdepartamentos.
    /// </summary>
    public int TotalEmployeesInHierarchy { get; set; }

    /// <summary>
    /// Departamento pai na hierarquia (nulo se for raiz).
    /// </summary>
    public DepartmentHierarchyDto? Parent { get; set; }

    /// <summary>
    /// Lista de departamentos filhos.
    /// </summary>
    public List<DepartmentHierarchyDto> Children { get; set; } = new();

    /// <summary>
    /// Lista de funcionários diretamente vinculados ao departamento.
    /// </summary>
    public List<EmployeeSummaryDto> DirectEmployees { get; set; } = new();
}
