namespace SynQcore.Domain.Entities.Communication;

public class Post : BaseEntity
{
    // Conteúdo
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? DocumentUrl { get; set; }

    // Metadados
    public bool IsPinned { get; set; }
    public bool IsOfficial { get; set; }

    // Visibilidade (corporativa)
    public PostVisibility Visibility { get; set; } = PostVisibility.Company;
    public Guid? DepartmentId { get; set; }
    public Guid? TeamId { get; set; }

    // Relacionamentos
    public Guid AuthorId { get; set; }
    public Employee Author { get; set; } = null!;
    public Department? Department { get; set; }
    public Team? Team { get; set; }

    // Propriedades de Navegação
    // public ICollection<Comment> Comments { get; set; } = [];
    // public ICollection<PostLike> Likes { get; set; } = [];
}
public enum PostVisibility
{
    Public = 0,        // Todos na empresa
    Department = 1,    // Apenas departamento específico  
    Team = 2,         // Apenas team específico
    Company = 3       // Toda a empresa (padrão)
}