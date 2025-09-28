using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

/// <summary>
/// Request para criação de novo funcionário no sistema.
/// Contém todas as informações obrigatórias e opcionais para o cadastro.
/// </summary>
public record CreateEmployeeRequest
{
    /// <summary>
    /// Primeiro nome do funcionário.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Sobrenome do funcionário.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Email corporativo único do funcionário.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Telefone de contato (opcional).
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Data de contratação do funcionário.
    /// </summary>
    public DateTime HireDate { get; init; }

    /// <summary>
    /// ID do gestor direto (opcional).
    /// </summary>
    public Guid? ManagerId { get; init; }

    /// <summary>
    /// Lista de IDs dos departamentos aos quais o funcionário pertencerá.
    /// </summary>
    public List<Guid> DepartmentIds { get; init; } = new();

    /// <summary>
    /// Lista de IDs das equipes das quais o funcionário participará.
    /// </summary>
    public List<Guid> TeamIds { get; init; } = new();
}
