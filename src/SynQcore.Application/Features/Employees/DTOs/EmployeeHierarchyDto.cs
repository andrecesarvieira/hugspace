namespace SynQcore.Application.Features.Employees.DTOs;

/// <summary>
/// DTO para representar a hierarquia organizacional de um funcionário.
/// Inclui dados do funcionário, seu gestor, subordinados e pares.
/// </summary>
public record EmployeeHierarchyDto
{
    /// <summary>
    /// Dados completos do funcionário principal na hierarquia.
    /// </summary>
    public EmployeeDto Employee { get; init; } = null!;

    /// <summary>
    /// Dados do gerente/supervisor direto (nulo se for o topo da hierarquia).
    /// </summary>
    public EmployeeDto? Manager { get; init; }

    /// <summary>
    /// Lista de funcionários subordinados diretos.
    /// </summary>
    public List<EmployeeDto> Subordinates { get; init; } = new();

    /// <summary>
    /// Lista de funcionários do mesmo nível hierárquico (mesmo gestor).
    /// </summary>
    public List<EmployeeDto> Peers { get; init; } = new();
}
