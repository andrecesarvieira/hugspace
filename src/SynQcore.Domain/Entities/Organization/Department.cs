namespace SynQcore.Domain.Entities.Organization;

public class Department : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? EstablishedDate { get; set; }

    public Guid? ParentDepartmentId { get; set; }

    public Guid? ManagerEmployeeId { get; set; }

    public Department? ParentDepartment { get; set; }

    public ICollection<Department> SubDepartments { get; set; } = [];

    public Employee? Manager { get; set; }

    public ICollection<EmployeeDepartment> Employees { get; set; } = [];

    public ICollection<Team> Teams { get; set; } = [];

    public ICollection<Post> Posts { get; set; } = [];
}
