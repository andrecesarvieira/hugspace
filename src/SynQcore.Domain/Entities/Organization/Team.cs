namespace SynQcore.Domain.Entities.Organization;

public class Team : BaseEntity
{
    // Identificação
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    // Classificação
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    // Informações
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Gestão
    public Guid? LeaderEmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }

    // Configurações
    public bool IsActive { get; set; } = true;
    public int? MaxMembers { get; set; }

    // Propriedades de Navegação - Organization
    public Employee? Leader { get; set; }
    public Department? Department { get; set; }
    public ICollection<TeamMembership> Members { get; set; } = [];
    
    // Propriedades de Navegação - Communication
    public ICollection<Post> Posts { get; set; } = [];
}