using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities.Organization;

public class Position : BaseEntity
{
    // Identificação
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    // Classificação
    public string Level { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Descrição
    public string? Description { get; set; }
    public string? Requirements { get; set; }

    // Departamento
    public Guid? DefaultDepartmentId { get; set; }

    // Faixa salarial (opcional)
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    // Propriedades de Navegação
    // public Department? DefaultDepartment { get; set; }
    // public ICollection<Employee> Employees { get; set; } = [];

}