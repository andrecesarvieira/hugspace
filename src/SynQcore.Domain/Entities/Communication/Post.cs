namespace SynQcore.Domain.Entities.Communication;

public class Post : BaseEntity
{
    // Conteúdo básico
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; } // Para knowledge articles
    
    // Mídia e anexos
    public string? ImageUrl { get; set; }
    public string? DocumentUrl { get; set; }
    
    // Tipo e características
    public PostType Type { get; set; } = PostType.Post;
    public bool IsPinned { get; set; }
    public bool IsOfficial { get; set; }
    public bool RequiresApproval { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Published;

    // Visibilidade (corporativa)
    public PostVisibility Visibility { get; set; } = PostVisibility.Company;
    public Guid? DepartmentId { get; set; }
    public Guid? TeamId { get; set; }

    // Knowledge Management
    public Guid? CategoryId { get; set; }
    public KnowledgeCategory? Category { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    
    // Versionamento
    public string Version { get; set; } = "1.0";
    public Guid? ParentPostId { get; set; } // Para versões
    public Post? ParentPost { get; set; }
    public ICollection<Post> Versions { get; set; } = [];

    // Relacionamentos
    public Guid AuthorId { get; set; }
    public Employee Author { get; set; } = null!;
    public Department? Department { get; set; }
    public Team? Team { get; set; }

    // Propriedades de Navegação
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<PostLike> Likes { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}

public enum PostType
{
    Post = 0,           // Post normal
    Article = 1,        // Knowledge article
    Announcement = 2,   // Comunicado oficial
    Policy = 3,         // Política/procedimento
    FAQ = 4,           // Pergunta frequente
    HowTo = 5,         // Tutorial/guia
    News = 6           // Notícia corporativa
}

public enum PostStatus
{
    Draft = 0,         // Rascunho
    PendingApproval = 1, // Aguardando aprovação
    Published = 2,     // Publicado
    Archived = 3,      // Arquivado
    Rejected = 4       // Rejeitado
}

public enum PostVisibility
{
    Public = 0,        // Todos na empresa
    Department = 1,    // Apenas departamento específico  
    Team = 2,         // Apenas team específico
    Company = 3       // Toda a empresa (padrão)
}