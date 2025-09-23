using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities.Organization;

public class Department : BaseEntity
{
    // Identificação
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Informações
    public string? Description { get; set; } // do Departamento
    public bool IsActive { get; set; } = true;

    // Metadados
    public DateTime? EstablishedDate { get; set; }

    // Hierarquia
    public Guid? ParentDepartmentId { get; set; } // para subdepartamentos
    public Guid? ManagerEmployeeId { get; set; } // gestor do departamento

    // Propriedades de Navegação
    // public Department? ParentDepartment { get; set; }
    // public ICollection<Department> SubDepartments { get; set; } = [];
    // public Employee? Manager { get; set; }
    // public ICollection<EmployeeDepartment> Employees = []
}