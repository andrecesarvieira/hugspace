namespace SynQcore.Domain.Entities.Relationships;

/// <summary>
/// Representa um relacionamento hierárquico entre gestor e subordinado na organização.
/// Suporta diferentes tipos de relacionamentos organizacionais.
/// </summary>
public class ReportingRelationship : BaseEntity
{
    /// <summary>
    /// ID do funcionário gestor na relação hierárquica.
    /// </summary>
    public Guid ManagerId { get; set; }

    /// <summary>
    /// Funcionário gestor na relação hierárquica.
    /// </summary>
    public Employee Manager { get; set; } = null!;

    /// <summary>
    /// ID do funcionário subordinado na relação hierárquica.
    /// </summary>
    public Guid SubordinateId { get; set; }

    /// <summary>
    /// Funcionário subordinado na relação hierárquica.
    /// </summary>
    public Employee Subordinate { get; set; } = null!;

    /// <summary>
    /// Data de início do relacionamento hierárquico.
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de fim do relacionamento hierárquico (null se ainda ativo).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Indica se o relacionamento hierárquico está ativo.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Tipo do relacionamento hierárquico (direto, matricial, etc.).
    /// </summary>
    public ReportingType Type { get; set; } = ReportingType.Direct;

    /// <summary>
    /// ID do departamento onde o relacionamento se aplica (se relevante).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Departamento onde o relacionamento se aplica.
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// ID da equipe onde o relacionamento se aplica (se relevante).
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Equipe onde o relacionamento se aplica.
    /// </summary>
    public Team? Team { get; set; }

    /// <summary>
    /// Indica se é um relacionamento atual (ativo e sem data de fim).
    /// </summary>
    public bool IsCurrentRelationship => IsActive && EndDate == null;

    /// <summary>
    /// Duração do relacionamento hierárquico.
    /// </summary>
    public TimeSpan? RelationshipDuration => EndDate?.Subtract(StartDate)
        ?? DateTime.UtcNow.Subtract(StartDate);
}

/// <summary>
/// Tipos de relacionamentos hierárquicos organizacionais.
/// </summary>
public enum ReportingType
{
    /// <summary>
    /// Subordinado direto na hierarquia tradicional.
    /// </summary>
    Direct = 0,

    /// <summary>
    /// Subordinado indireto (através de outros gestores).
    /// </summary>
    Indirect = 1,

    /// <summary>
    /// Relacionamento matricial para projetos ou funções específicas.
    /// </summary>
    Matrix = 2,

    /// <summary>
    /// Relacionamento funcional temporário para atividades específicas.
    /// </summary>
    Functional = 3,

    /// <summary>
    /// Relacionamento baseado em projeto específico.
    /// </summary>
    ProjectBased = 4
}
