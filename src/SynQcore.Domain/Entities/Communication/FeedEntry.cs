namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa uma entrada no feed corporativo de um usuário
/// Controla o que aparece no timeline personalizado de cada funcionário
/// </summary>
public class FeedEntry : BaseEntity
{
    /// <summary>
    /// ID do funcionário que verá esta entrada no feed.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID do post associado a esta entrada do feed.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// ID do autor do post original.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Prioridade da entrada no feed para ordenação.
    /// </summary>
    public FeedPriority Priority { get; set; } = FeedPriority.Normal;

    /// <summary>
    /// Score de relevância calculado pelo algoritmo (0.0 a 1.0).
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Razão pela qual esta entrada aparece no feed do usuário.
    /// </summary>
    public FeedReason Reason { get; set; }

    /// <summary>
    /// Data e hora quando a entrada foi visualizada pelo usuário.
    /// </summary>
    public DateTime? ViewedAt { get; set; }

    /// <summary>
    /// Indica se a entrada foi lida pelo usuário.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Indica se a entrada foi marcada como favorita pelo usuário.
    /// </summary>
    public bool IsBookmarked { get; set; }

    /// <summary>
    /// Indica se o usuário ocultou esta entrada do feed.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// ID do departamento relevante para esta entrada (se aplicável).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// ID da equipe relevante para esta entrada (se aplicável).
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Funcionário proprietário do feed.
    /// </summary>
    public Employee User { get; set; } = null!;

    /// <summary>
    /// Post associado a esta entrada do feed.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    /// Autor do post original.
    /// </summary>
    public Employee Author { get; set; } = null!;

    /// <summary>
    /// Departamento relevante para contexto (se aplicável).
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// Equipe relevante para contexto (se aplicável).
    /// </summary>
    public Team? Team { get; set; }
}

/// <summary>
/// Prioridade no feed corporativo
/// Determina ordem de exibição e destaque visual
/// </summary>
public enum FeedPriority
{
    /// <summary>
    /// Prioridade baixa - conteúdo menos relevante.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Prioridade normal - padrão para a maioria do conteúdo.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Prioridade alta - conteúdo importante que merece destaque.
    /// </summary>
    High = 2,

    /// <summary>
    /// Prioridade urgente - comunicados que requerem atenção imediata.
    /// </summary>
    Urgent = 3,

    /// <summary>
    /// Prioridade executiva - comunicação da diretoria ou alta gestão.
    /// </summary>
    Executive = 4
}

/// <summary>
/// Razão pela qual um item aparece no feed
/// Usado para personalização e explicabilidade do algoritmo
/// </summary>
public enum FeedReason
{
    /// <summary>
    /// Aparece porque o usuário segue o autor do post.
    /// </summary>
    Following = 0,

    /// <summary>
    /// Aparece porque é do mesmo departamento do usuário.
    /// </summary>
    SameDepartment = 1,

    /// <summary>
    /// Aparece porque é da mesma equipe do usuário.
    /// </summary>
    SameTeam = 2,

    /// <summary>
    /// Aparece porque relacionado a habilidades similares.
    /// </summary>
    SimilarSkills = 3,

    /// <summary>
    /// Aparece porque o usuário tem interesse nas tags do post.
    /// </summary>
    TagInterest = 4,

    /// <summary>
    /// Aparece porque o conteúdo está em alta na empresa.
    /// </summary>
    Trending = 5,

    /// <summary>
    /// Aparece porque o algoritmo recomenda baseado no perfil.
    /// </summary>
    Recommended = 6,

    /// <summary>
    /// Aparece porque é comunicação oficial da empresa.
    /// </summary>
    Official = 7,

    /// <summary>
    /// Aparece porque o usuário foi mencionado no conteúdo.
    /// </summary>
    Mentioned = 8,

    /// <summary>
    /// Aparece porque o usuário tem interesse na categoria.
    /// </summary>
    CategoryInterest = 9
}
