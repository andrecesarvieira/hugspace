namespace SynQcore.Application.Features.Departments.DTOs;

/// <summary>
/// DTO resumido de funcionário para exibição em listas e hierarquias.
/// Contém informações essenciais sem sobrecarregar a interface.
/// </summary>
public class EmployeeSummaryDto
{
    /// <summary>
    /// Identificador único do funcionário.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Primeiro nome do funcionário.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome do funcionário.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email corporativo do funcionário.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// URL do avatar/foto do perfil (opcional).
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Indica se o funcionário está ativo na organização.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Data de contratação do funcionário.
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Nome completo calculado (FirstName + LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Nome para exibição (FullName ou Email se nome vazio).
    /// </summary>
    public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : Email;
}
