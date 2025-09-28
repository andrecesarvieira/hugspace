using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.Departments.DTOs;

/// <summary>
/// Request para atualização de departamento existente.
/// Permite modificar propriedades e reorganizar hierarquia.
/// </summary>
public class UpdateDepartmentRequest
{
    /// <summary>
    /// Novo nome do departamento (obrigatório, máximo 200 caracteres).
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Novo código identificador do departamento (obrigatório, máximo 50 caracteres).
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nova descrição do departamento (opcional, máximo 1000 caracteres).
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Novo ID do departamento pai para reorganização hierárquica (opcional).
    /// </summary>
    public Guid? ParentId { get; set; }
}
