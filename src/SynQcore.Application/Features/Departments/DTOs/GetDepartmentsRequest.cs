using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.Departments.DTOs;

/// <summary>
/// Request para busca e listagem de departamentos com filtros avançados.
/// Suporta paginação e inclusão de dados relacionados.
/// </summary>
public class GetDepartmentsRequest
{
    /// <summary>
    /// Número da página para paginação (padrão: 1).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página (entre 1 e 100, padrão: 10).
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Filtrar por nome do departamento (máximo 100 caracteres).
    /// </summary>
    [StringLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Filtrar por código do departamento (máximo 100 caracteres).
    /// </summary>
    [StringLength(100)]
    public string? Code { get; set; }

    /// <summary>
    /// Filtrar por departamento pai específico.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Filtrar apenas departamentos ativos ou inativos.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Incluir dados dos funcionários associados aos departamentos.
    /// </summary>
    public bool IncludeEmployees { get; set; }
}
