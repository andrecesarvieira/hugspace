using System;
using System.Collections.Generic;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Departments.DTOs;

/// <summary>
/// DTO para representação completa de departamento organizacional.
/// Inclui hierarquia, estatísticas e lista de funcionários.
/// </summary>
public class DepartmentDto
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
    /// Código identificador do departamento.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição das responsabilidades do departamento.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// ID do departamento pai na hierarquia (se aplicável).
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Nome do departamento pai para exibição.
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// Status ativo do departamento.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Número de subdepartamentos filhos.
    /// </summary>
    public int ChildrenCount { get; set; }

    /// <summary>
    /// Número total de funcionários no departamento.
    /// </summary>
    public int EmployeesCount { get; set; }

    /// <summary>
    /// Lista resumida dos funcionários do departamento.
    /// </summary>
    public List<EmployeeSummaryDto> Employees { get; set; } = new();

    /// <summary>
    /// Data de criação do departamento.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
