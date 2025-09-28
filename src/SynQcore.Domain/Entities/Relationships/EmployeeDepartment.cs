namespace SynQcore.Domain.Entities.Relationships;

/// <summary>
/// Representa a relação entre um funcionário e um departamento organizacional.
/// Permite que um funcionário pertença a múltiplos departamentos com diferentes funções.
/// </summary>
public class EmployeeDepartment : BaseEntity
{
    /// <summary>
    /// ID do funcionário na relação.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário na relação.
    /// </summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// ID do departamento na relação.
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Departamento na relação.
    /// </summary>
    public Department Department { get; set; } = null!;

    /// <summary>
    /// Data de início da alocação no departamento.
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de fim da alocação no departamento (null se ainda ativo).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Indica se a alocação está ativa.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indica se este é o departamento primário do funcionário.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Função ou cargo específico do funcionário neste departamento.
    /// </summary>
    public string? RoleInDepartment { get; set; }

    /// <summary>
    /// Indica se esta é uma alocação atual (ativa e sem data de fim).
    /// </summary>
    public bool IsCurrentAssignment => IsActive && EndDate == null;
}
