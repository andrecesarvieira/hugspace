using MediatR;
namespace SynQcore.Application.Features.Employees.DTOs;

/// <summary>
/// DTO para representação de funcionário nas operações da aplicação.
/// Contém informações completas incluindo relacionamentos organizacionais.
/// </summary>
public record EmployeeDto
{
    /// <summary>
    /// Identificador único do funcionário.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Primeiro nome do funcionário.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Sobrenome do funcionário.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Email corporativo do funcionário.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Telefone de contato (opcional).
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// URL do avatar ou foto do perfil.
    /// </summary>
    public string? Avatar { get; init; }

    /// <summary>
    /// Data de contratação.
    /// </summary>
    public DateTime HireDate { get; init; }

    /// <summary>
    /// Status ativo do funcionário na organização.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// ID do gestor direto (opcional).
    /// </summary>
    public Guid? ManagerId { get; init; }

    /// <summary>
    /// Nome do gestor direto para exibição.
    /// </summary>
    public string? ManagerName { get; init; }

    /// <summary>
    /// Lista de departamentos aos quais o funcionário pertence.
    /// </summary>
    public List<EmployeeDepartmentDto> Departments { get; init; } = new();

    /// <summary>
    /// Lista de equipes das quais o funcionário participa.
    /// </summary>
    public List<TeamDto> Teams { get; init; } = new();

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Data da última atualização (opcional).
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO simplificado para departamento no contexto de funcionário.
/// </summary>
public record EmployeeDepartmentDto
{
    /// <summary>
    /// Identificador único do departamento.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nome do departamento.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Descrição do departamento (opcional).
    /// </summary>
    public string? Description { get; init; }
}

/// <summary>
/// DTO simplificado para equipe no contexto de funcionário.
/// </summary>
public record TeamDto
{
    /// <summary>
    /// Identificador único da equipe.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nome da equipe.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Descrição da equipe (opcional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Função do funcionário na equipe.
    /// </summary>
    public string Role { get; init; } = "Member";
}
