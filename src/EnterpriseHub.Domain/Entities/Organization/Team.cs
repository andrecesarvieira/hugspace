using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities.Organization;

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

    // COnfigurações
    public bool IsActive { get; set; } = true;
    public int? MaxMembers { get; set; }

    // Propriedades de Navegação
    // public Employee? Leader { get; set; }
    // public ICollection<TeamMembership> Members { get; set; } = [];
}