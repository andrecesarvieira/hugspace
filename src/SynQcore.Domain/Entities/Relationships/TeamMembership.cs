namespace SynQcore.Domain.Entities.Relationships;

/// <summary>
/// Representa a participação de um funcionário em uma equipe organizacional.
/// Controla funções, períodos de participação e status ativo.
/// </summary>
public class TeamMembership : BaseEntity
{
    /// <summary>
    /// ID do funcionário membro da equipe.
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário membro da equipe.
    /// </summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// ID da equipe.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Equipe da qual o funcionário é membro.
    /// </summary>
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Data quando o funcionário ingressou na equipe.
    /// </summary>
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora de ingresso na equipe (timestamp completo).
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data quando o funcionário deixou a equipe (null se ainda ativo).
    /// </summary>
    public DateTime? LeftDate { get; set; }

    /// <summary>
    /// Indica se o funcionário ainda é membro ativo da equipe.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Função do funcionário na equipe.
    /// </summary>
    public TeamRole Role { get; set; } = TeamRole.Member;

    /// <summary>
    /// Descrição específica da função na equipe.
    /// </summary>
    public string? SpecificRole { get; set; }

    /// <summary>
    /// Indica se é um membro atual da equipe (ativo e sem data de saída).
    /// </summary>
    public bool IsCurrentMember => IsActive && LeftDate == null;

    /// <summary>
    /// Duração da participação na equipe.
    /// </summary>
    public TimeSpan? MembershipDuration => LeftDate?.Subtract(JoinedDate)
        ?? DateTime.UtcNow.Subtract(JoinedDate);
}

/// <summary>
/// Funções que um funcionário pode desempenhar em uma equipe.
/// </summary>
public enum TeamRole
{
    /// <summary>
    /// Membro regular da equipe.
    /// </summary>
    Member = 0,

    /// <summary>
    /// Líder da equipe.
    /// </summary>
    Leader = 1,

    /// <summary>
    /// Co-líder ou vice-líder da equipe.
    /// </summary>
    CoLeader = 2,

    /// <summary>
    /// Coordenador de atividades específicas.
    /// </summary>
    Coordinator = 3
}
