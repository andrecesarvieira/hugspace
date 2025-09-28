using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

/// <summary>
/// Request para busca avançada de funcionários com filtros e paginação.
/// Permite filtrar por diversos critérios organizacionais.
/// </summary>
public record EmployeeSearchRequest
{
    /// <summary>
    /// Termo de busca textual no nome, email ou cargo.
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filtrar por departamento específico.
    /// </summary>
    public Guid? DepartmentId { get; init; }

    /// <summary>
    /// Filtrar por equipe específica.
    /// </summary>
    public Guid? TeamId { get; init; }

    /// <summary>
    /// Filtrar por gestor específico.
    /// </summary>
    public Guid? ManagerId { get; init; }

    /// <summary>
    /// Filtrar apenas funcionários ativos ou inativos.
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Número da página para paginação (padrão: 1).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Tamanho da página para paginação (padrão: 20).
    /// </summary>
    public int PageSize { get; init; } = 20;
}
