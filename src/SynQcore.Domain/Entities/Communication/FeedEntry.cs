namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma entrada no feed corporativo de um usuário
/// Controla o que aparece no timeline personalizado de cada funcionário
/// </summary>
public class FeedEntry : BaseEntity
{
    // Identificação da entrada no feed
    public Guid UserId { get; set; } // Funcionário que verá essa entrada
    public Guid PostId { get; set; } // Post associado
    public Guid AuthorId { get; set; } // Autor do post original
    
    // Prioridade e relevância
    public FeedPriority Priority { get; set; } = FeedPriority.Normal;
    public double RelevanceScore { get; set; } // Algoritmo de relevância (0-1)
    public FeedReason Reason { get; set; } // Por que aparece no feed
    
    // Engajamento e métricas
    public DateTime? ViewedAt { get; set; } // Quando foi visualizado
    public bool IsRead { get; set; }
    public bool IsBookmarked { get; set; }
    public bool IsHidden { get; set; } // Usuário ocultou
    
    // Contexto corporativo
    public Guid? DepartmentId { get; set; } // Departamento relevante
    public Guid? TeamId { get; set; } // Team relevante
    
    // Relacionamentos
    public Employee User { get; set; } = null!;
    public Post Post { get; set; } = null!;
    public Employee Author { get; set; } = null!;
    public Department? Department { get; set; }
    public Team? Team { get; set; }
}

/// <summary>
/// Prioridade no feed corporativo
/// Determina ordem de exibição e destaque visual
/// </summary>
public enum FeedPriority
{
    Low = 0,        // Conteúdo menos relevante
    Normal = 1,     // Prioridade padrão
    High = 2,       // Conteúdo importante
    Urgent = 3,     // Comunicados urgentes
    Executive = 4   // Comunicação da diretoria
}

/// <summary>
/// Razão pela qual um item aparece no feed
/// Usado para personalização e explicabilidade do algoritmo
/// </summary>
public enum FeedReason
{
    Following = 0,          // Segue o autor
    SameDepartment = 1,     // Mesmo departamento
    SameTeam = 2,          // Mesma equipe
    SimilarSkills = 3,     // Skills similares
    TagInterest = 4,       // Interesse em tags
    Trending = 5,          // Conteúdo em alta
    Recommended = 6,       // Algoritmo recomenda
    Official = 7,          // Comunicação oficial
    Mentioned = 8,         // Foi mencionado
    CategoryInterest = 9   // Interesse na categoria
}