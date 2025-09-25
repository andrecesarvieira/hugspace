

namespace SynQcore.Application.Features.KnowledgeManagement.DTOs;

public class KnowledgeCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<KnowledgeCategoryDto> SubCategories { get; set; } = new();
    public int PostsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateKnowledgeCategoryDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Color { get; set; } = "#007ACC";
    public string Icon { get; set; } = "📄";
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; }
}

public class UpdateKnowledgeCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool? IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
}