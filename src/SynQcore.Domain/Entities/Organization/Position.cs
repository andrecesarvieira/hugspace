namespace SynQcore.Domain.Entities.Organization;

public class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public string? Requirements { get; set; }

    public Guid? DefaultDepartmentId { get; set; }

    public decimal? MinSalary { get; set; }

    public decimal? MaxSalary { get; set; }

    // Propriedades de navegação comentadas para futura implementação
    // public Department? DefaultDepartment { get; set; }
    // public ICollection<Employee> Employees { get; set; } = [];
}
