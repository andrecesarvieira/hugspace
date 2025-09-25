namespace SynQcore.Domain.Entities.Communication;

public class KnowledgeCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "#007ACC"; // Cor para UI
    public string Icon { get; set; } = "📄"; // Emoji ou icon class
    public bool IsActive { get; set; } = true;
    
    // Hierarquia de categorias
    public Guid? ParentCategoryId { get; set; }
    public KnowledgeCategory? ParentCategory { get; set; }
    public ICollection<KnowledgeCategory> SubCategories { get; set; } = [];
    
    // Relacionamentos
    public ICollection<Post> Posts { get; set; } = [];
}