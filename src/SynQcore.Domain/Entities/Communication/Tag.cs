namespace SynQcore.Domain.Entities.Communication;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TagType Type { get; set; } = TagType.General;
    public string Color { get; set; } = "#6B7280"; // Cor para UI
    public int UsageCount { get; set; } // Para estatísticas
    
    // Relacionamentos
    public ICollection<PostTag> PostTags { get; set; } = [];
}

public enum TagType
{
    General = 0,      // Tag geral
    Skill = 1,        // Habilidade/competência
    Technology = 2,   // Tecnologia/ferramenta
    Department = 3,   // Relacionada a departamento
    Project = 4,      // Relacionada a projeto
    Policy = 5        // Política/procedimento
}