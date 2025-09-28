using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

/// <summary>
/// Request para atualização de dados de funcionário existente.
/// Permite modificação de informações pessoais e organizacionais.
/// </summary>
public record UpdateEmployeeRequest
{
    /// <summary>
    /// Primeiro nome atualizado do funcionário.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Sobrenome atualizado do funcionário.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Telefone de contato atualizado (opcional).
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Status ativo do funcionário na organização.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// ID do novo gestor direto (opcional).
    /// </summary>
    public Guid? ManagerId { get; init; }

    /// <summary>
    /// Lista atualizada de IDs dos departamentos.
    /// </summary>
    public List<Guid> DepartmentIds { get; init; } = new();

    /// <summary>
    /// Lista atualizada de IDs das equipes.
    /// </summary>
    public List<Guid> TeamIds { get; init; } = new();
}
