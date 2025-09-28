namespace SynQcore.Domain.Entities.Communication;

public class KnowledgeCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Color { get; set; } = "#007ACC";

    public string Icon { get; set; } = "📄";

    public bool IsActive { get; set; } = true;

    public Guid? ParentCategoryId { get; set; }

    public KnowledgeCategory? ParentCategory { get; set; }

    public ICollection<KnowledgeCategory> SubCategories { get; set; } = [];

    public ICollection<Post> Posts { get; set; } = [];
}
