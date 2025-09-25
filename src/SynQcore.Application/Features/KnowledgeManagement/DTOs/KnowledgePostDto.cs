using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.DTOs;

public class KnowledgePostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public PostType Type { get; set; }
    public PostStatus Status { get; set; }
    public PostVisibility Visibility { get; set; }
    public string Version { get; set; } = null!;
    public bool RequiresApproval { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    
    // Relacionamentos
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string AuthorEmail { get; set; } = null!;
    
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
    
    public Guid? ParentPostId { get; set; }
    public string? ParentPostTitle { get; set; }
    
    // Tags
    public List<TagDto> Tags { get; set; } = new();
    
    // Versionamento
    public List<KnowledgePostDto> Versions { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateKnowledgePostDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public PostType Type { get; set; } = PostType.Article;
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public PostVisibility Visibility { get; set; } = PostVisibility.Company;
    public bool RequiresApproval { get; set; }
    
    public Guid? CategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? ParentPostId { get; set; }
    
    // Tags para associar
    public List<Guid> TagIds { get; set; } = new();
}

public class UpdateKnowledgePostDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public PostType? Type { get; set; }
    public PostStatus? Status { get; set; }
    public PostVisibility? Visibility { get; set; }
    public bool? RequiresApproval { get; set; }
    
    public Guid? CategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? TeamId { get; set; }
    
    // Tags para associar (substitui todas as existentes)
    public List<Guid>? TagIds { get; set; }
}

public class KnowledgePostSearchDto
{
    public string? SearchTerm { get; set; }
    public PostType? Type { get; set; }
    public PostStatus? Status { get; set; }
    public PostVisibility? Visibility { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? TeamId { get; set; }
    public List<Guid>? TagIds { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}