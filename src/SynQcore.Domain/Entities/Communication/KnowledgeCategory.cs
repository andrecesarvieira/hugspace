namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma categoria para organização do conhecimento corporativo.
/// Suporta hierarquia de categorias e subcategorias.
/// </summary>
public class KnowledgeCategory : BaseEntity
{
    /// <summary>
    /// Nome da categoria de conhecimento.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da categoria.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Cor hexadecimal para exibição na interface.
    /// </summary>
    public string Color { get; set; } = "#007ACC";

    /// <summary>
    /// Ícone ou emoji para representação visual da categoria.
    /// </summary>
    public string Icon { get; set; } = "📄";

    /// <summary>
    /// Indica se a categoria está ativa para uso.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ID da categoria pai na hierarquia (se é subcategoria).
    /// </summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>
    /// Categoria pai na hierarquia.
    /// </summary>
    public KnowledgeCategory? ParentCategory { get; set; }

    /// <summary>
    /// Subcategorias filhas desta categoria.
    /// </summary>
    public ICollection<KnowledgeCategory> SubCategories { get; set; } = [];

    /// <summary>
    /// Posts associados a esta categoria de conhecimento.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = [];
}
