namespace SynQcore.Domain.Entities.Organization;

/// <summary>
/// Representa uma equipe dentro da estrutura organizacional.
/// Pode ser permanente ou temporária, com líder e membros definidos.
/// </summary>
public class Team : BaseEntity
{
    /// <summary>
    /// Nome da equipe.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código identificador da equipe.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da equipe (ex: "Projeto", "Operacional", "Temporária").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Status atual da equipe.
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Descrição dos objetivos e responsabilidades da equipe.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data de formação da equipe.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Data de encerramento da equipe (para equipes temporárias).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// ID do funcionário líder da equipe.
    /// </summary>
    public Guid? LeaderEmployeeId { get; set; }

    /// <summary>
    /// ID do departamento ao qual a equipe pertence.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Indica se a equipe está ativa.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Número máximo de membros permitidos na equipe.
    /// </summary>
    public int? MaxMembers { get; set; }

    /// <summary>
    /// Funcionário líder da equipe.
    /// </summary>
    public Employee? Leader { get; set; }

    /// <summary>
    /// Departamento ao qual a equipe pertence.
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// Membros da equipe com suas funções e períodos.
    /// </summary>
    public ICollection<TeamMembership> Members { get; set; } = [];

    /// <summary>
    /// Posts criados ou associados a esta equipe.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = [];
}
