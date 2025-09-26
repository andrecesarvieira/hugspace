namespace SynQcore.Application.Features.Employees.DTOs;

// Dados hierárquicos do funcionário com superior, subordinados e pares
public record EmployeeHierarchyDto
{
    // Dados do funcionário principal
    public EmployeeDto Employee { get; init; } = null!;
    
    // Dados do gerente/supervisor direto
    public EmployeeDto? Manager { get; init; }
    
    // Lista de funcionários subordinados diretos
    public List<EmployeeDto> Subordinates { get; init; } = new();
    
    // Lista de funcionários do mesmo nível hierárquico
    public List<EmployeeDto> Peers { get; init; } = new();
}