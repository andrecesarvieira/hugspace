namespace SynQcore.Domain.Entities.Organization;

/// <summary>
/// Representa um departamento na estrutura organizacional da empresa.
/// Suporta hierarquia de departamentos e subdepartamentos.
/// </summary>
public class Department : BaseEntity
{
    /// <summary>
    /// Código único identificador do departamento (ex: "TI", "RH", "FIN").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do departamento.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada das responsabilidades e função do departamento.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indica se o departamento está operacionalmente ativo.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data de estabelecimento ou criação do departamento.
    /// </summary>
    public DateTime? EstablishedDate { get; set; }

    /// <summary>
    /// ID do departamento pai na hierarquia organizacional.
    /// </summary>
    public Guid? ParentDepartmentId { get; set; }

    /// <summary>
    /// ID do funcionário gestor responsável pelo departamento.
    /// </summary>
    public Guid? ManagerEmployeeId { get; set; }

    /// <summary>
    /// Departamento pai na hierarquia organizacional.
    /// </summary>
    public Department? ParentDepartment { get; set; }

    /// <summary>
    /// Subdepartamentos subordinados a este departamento.
    /// </summary>
    public ICollection<Department> SubDepartments { get; set; } = [];

    /// <summary>
    /// Funcionário gestor responsável pelo departamento.
    /// </summary>
    public Employee? Manager { get; set; }

    /// <summary>
    /// Funcionários pertencentes a este departamento.
    /// </summary>
    public ICollection<EmployeeDepartment> Employees { get; set; } = [];

    /// <summary>
    /// Equipes organizadas dentro deste departamento.
    /// </summary>
    public ICollection<Team> Teams { get; set; } = [];

    /// <summary>
    /// Posts criados ou associados a este departamento.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = [];
}
