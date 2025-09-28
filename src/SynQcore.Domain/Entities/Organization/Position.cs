namespace SynQcore.Domain.Entities.Organization;

/// <summary>
/// Representa um cargo ou posição na estrutura organizacional.
/// Define título, nível, requisitos e faixa salarial.
/// </summary>
public class Position : BaseEntity
{
    /// <summary>
    /// Título do cargo (ex: "Analista", "Gerente", "Diretor").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Código identificador do cargo.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nível hierárquico do cargo (ex: "Júnior", "Pleno", "Sênior").
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o cargo está ativo para contratações.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Descrição detalhada das responsabilidades do cargo.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Requisitos necessários para o cargo.
    /// </summary>
    public string? Requirements { get; set; }

    /// <summary>
    /// ID do departamento padrão para este cargo.
    /// </summary>
    public Guid? DefaultDepartmentId { get; set; }

    /// <summary>
    /// Salário mínimo da faixa salarial do cargo.
    /// </summary>
    public decimal? MinSalary { get; set; }

    /// <summary>
    /// Salário máximo da faixa salarial do cargo.
    /// </summary>
    public decimal? MaxSalary { get; set; }

    // Propriedades de navegação comentadas para futura implementação
    // public Department? DefaultDepartment { get; set; }
    // public ICollection<Employee> Employees { get; set; } = [];
}
