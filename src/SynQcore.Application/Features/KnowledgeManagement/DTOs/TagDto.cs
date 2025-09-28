using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.DTOs;

/// <summary>
/// DTO para visualização de tags de conhecimento.
/// Representa etiquetas para categorizar e organizar conteúdo.
/// </summary>
public class TagDto
{
    /// <summary>
    /// Identificador único da tag.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da tag.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descrição opcional da tag.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tipo da tag (General, Technical, Business, etc.).
    /// </summary>
    public TagType Type { get; set; }

    /// <summary>
    /// Cor da tag em formato hexadecimal para exibição.
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// Número de vezes que a tag foi utilizada.
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Data e hora de criação da tag.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para criação de nova tag de conhecimento.
/// Define as propriedades básicas para criar uma etiqueta.
/// </summary>
public class CreateTagDto
{
    /// <summary>
    /// Nome da nova tag.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descrição opcional da tag.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tipo da tag (padrão: General).
    /// </summary>
    public TagType Type { get; set; } = TagType.General;

    /// <summary>
    /// Cor da tag em formato hexadecimal (padrão: #6B7280).
    /// </summary>
    public string Color { get; set; } = "#6B7280";
}

/// <summary>
/// DTO para atualização de tag existente.
/// Permite modificar propriedades específicas da tag.
/// </summary>
public class UpdateTagDto
{
    /// <summary>
    /// Novo nome da tag (opcional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Nova descrição da tag (opcional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Novo tipo da tag (opcional).
    /// </summary>
    public TagType? Type { get; set; }

    /// <summary>
    /// Nova cor da tag (opcional).
    /// </summary>
    public string? Color { get; set; }
}
