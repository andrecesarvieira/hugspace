namespace SynQcore.Application.Features.KnowledgeManagement.DTOs;

/// <summary>
/// DTO para visualização de categorias de conhecimento.
/// Representa a organização hierárquica do conhecimento corporativo.
/// </summary>
public class KnowledgeCategoryDto
{
    /// <summary>
    /// Identificador único da categoria.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da categoria de conhecimento.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descrição detalhada da categoria.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Cor da categoria em formato hexadecimal para exibição.
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// Ícone representativo da categoria.
    /// </summary>
    public string Icon { get; set; } = null!;

    /// <summary>
    /// Indica se a categoria está ativa no sistema.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// ID da categoria pai para hierarquia.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>
    /// Nome da categoria pai.
    /// </summary>
    public string? ParentCategoryName { get; set; }

    /// <summary>
    /// Lista de subcategorias filhas.
    /// </summary>
    public List<KnowledgeCategoryDto> SubCategories { get; set; } = new();

    /// <summary>
    /// Número total de posts associados à categoria.
    /// </summary>
    public int PostsCount { get; set; }

    /// <summary>
    /// Data e hora de criação da categoria.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para criação de nova categoria de conhecimento.
/// Define a estrutura organizacional do conhecimento.
/// </summary>
public class CreateKnowledgeCategoryDto
{
    /// <summary>
    /// Nome da nova categoria.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descrição da nova categoria.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Cor da categoria (padrão: #007ACC).
    /// </summary>
    public string Color { get; set; } = "#007ACC";

    /// <summary>
    /// Ícone da categoria (padrão: 📄).
    /// </summary>
    public string Icon { get; set; } = "📄";

    /// <summary>
    /// Status ativo da categoria (padrão: true).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ID da categoria pai para criar hierarquia.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }
}

/// <summary>
/// DTO para atualização de categoria de conhecimento existente.
/// Permite modificar propriedades e reorganizar hierarquia.
/// </summary>
public class UpdateKnowledgeCategoryDto
{
    /// <summary>
    /// Novo nome da categoria (opcional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Nova descrição da categoria (opcional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Nova cor da categoria (opcional).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Novo ícone da categoria (opcional).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Novo status ativo da categoria (opcional).
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Novo ID da categoria pai para reorganizar hierarquia (opcional).
    /// </summary>
    public Guid? ParentCategoryId { get; set; }
}
