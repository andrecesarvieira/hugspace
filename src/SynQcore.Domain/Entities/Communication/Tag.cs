namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma tag para categorização e organização de conteúdo corporativo.
/// Suporta diferentes tipos e cores para melhor organização visual.
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>
    /// Nome da tag (ex: "JavaScript", "RH", "Treinamento").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional da tag.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tipo da tag para categorização.
    /// </summary>
    public TagType Type { get; set; } = TagType.General;

    /// <summary>
    /// Cor hexadecimal para exibição na interface.
    /// </summary>
    public string Color { get; set; } = "#6B7280";

    /// <summary>
    /// Número de vezes que a tag foi usada (para estatísticas).
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Posts que possuem esta tag.
    /// </summary>
    public ICollection<PostTag> PostTags { get; set; } = [];
}

/// <summary>
/// Tipos de tags para categorização de conteúdo corporativo.
/// </summary>
public enum TagType
{
    /// <summary>
    /// Tag geral sem categoria específica.
    /// </summary>
    General = 0,

    /// <summary>
    /// Tag relacionada a habilidades ou competências.
    /// </summary>
    Skill = 1,

    /// <summary>
    /// Tag relacionada a tecnologias ou ferramentas.
    /// </summary>
    Technology = 2,

    /// <summary>
    /// Tag relacionada a departamento específico.
    /// </summary>
    Department = 3,

    /// <summary>
    /// Tag relacionada a projeto corporativo.
    /// </summary>
    Project = 4,

    /// <summary>
    /// Tag relacionada a políticas ou procedimentos.
    /// </summary>
    Policy = 5
}
