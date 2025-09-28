using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.Departments.DTOs;

/// <summary>
/// Request para criação de novo departamento na organização.
/// Define a estrutura hierárquica organizacional.
/// </summary>
public class CreateDepartmentRequest
{
    /// <summary>
    /// Nome do departamento (obrigatório, máximo 200 caracteres).
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código único identificador do departamento (obrigatório, máximo 50 caracteres).
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do departamento (opcional, máximo 1000 caracteres).
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// ID do departamento pai para hierarquia organizacional (opcional).
    /// </summary>
    public Guid? ParentId { get; set; }
}
