namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa um post ou conteúdo corporativo na plataforma.
/// Suporta diversos tipos de conteúdo: posts, artigos, anúncios, políticas e conhecimento organizacional.
/// </summary>
public class Post : BaseEntity
{
    /// <summary>
    /// Título do post ou conteúdo.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Conteúdo textual principal do post.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Resumo ou descrição curta, especialmente para artigos de conhecimento.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// URL de imagem associada ao post.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URL de documento anexo ao post.
    /// </summary>
    public string? DocumentUrl { get; set; }

    /// <summary>
    /// Tipo do post (normal, artigo, anúncio, etc.).
    /// </summary>
    public PostType Type { get; set; } = PostType.Post;

    /// <summary>
    /// Indica se o post está fixado no topo da listagem.
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Indica se o post é oficial da organização.
    /// </summary>
    public bool IsOfficial { get; set; }

    /// <summary>
    /// Indica se o post requer aprovação antes da publicação.
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Status atual do post no workflow de publicação.
    /// </summary>
    public PostStatus Status { get; set; } = PostStatus.Draft;

    /// <summary>
    /// Nível de visibilidade do post na organização.
    /// </summary>
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;

    /// <summary>
    /// ID do departamento se a visibilidade for restrita ao departamento.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// ID da equipe se a visibilidade for restrita à equipe.
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// ID da categoria de conhecimento para organização do conteúdo.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Categoria de conhecimento do post.
    /// </summary>
    public KnowledgeCategory? Category { get; set; }

    /// <summary>
    /// Número total de visualizações do post.
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Número total de curtidas recebidas.
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// Número total de comentários no post.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Data e hora da última atividade relacionada ao post.
    /// </summary>
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Versão do conteúdo para controle de versionamento.
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// ID do post pai se este for uma nova versão.
    /// </summary>
    public Guid? ParentPostId { get; set; }

    /// <summary>
    /// Post pai na hierarquia de versionamento.
    /// </summary>
    public Post? ParentPost { get; set; }

    /// <summary>
    /// Coleção de versões derivadas deste post.
    /// </summary>
    public ICollection<Post> Versions { get; set; } = [];

    /// <summary>
    /// ID do funcionário autor do post.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Funcionário autor do post.
    /// </summary>
    public Employee Author { get; set; } = null!;

    /// <summary>
    /// Departamento associado ao post (se visível apenas ao departamento).
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// Equipe associada ao post (se visível apenas à equipe).
    /// </summary>
    public Team? Team { get; set; }

    /// <summary>
    /// Coleção de comentários no post.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    /// Coleção de curtidas no post.
    /// </summary>
    public ICollection<PostLike> Likes { get; set; } = [];

    /// <summary>
    /// Coleção de tags associadas ao post.
    /// </summary>
    public ICollection<PostTag> PostTags { get; set; } = [];

    /// <summary>
    /// Coleção de endorsements do post.
    /// </summary>
    public ICollection<Endorsement> Endorsements { get; set; } = [];
}

/// <summary>
/// Tipos de posts corporativos na plataforma.
/// </summary>
public enum PostType
{
    /// <summary>
    /// Post normal de discussão corporativa.
    /// </summary>
    Post = 0,

    /// <summary>
    /// Artigo de conhecimento ou documentação.
    /// </summary>
    Article = 1,

    /// <summary>
    /// Comunicado oficial da organização.
    /// </summary>
    Announcement = 2,

    /// <summary>
    /// Política corporativa ou procedimento organizacional.
    /// </summary>
    Policy = 3,

    /// <summary>
    /// Pergunta frequente (FAQ) sobre processos ou políticas.
    /// </summary>
    FAQ = 4,

    /// <summary>
    /// Tutorial ou guia passo-a-passo.
    /// </summary>
    HowTo = 5,

    /// <summary>
    /// Notícia corporativa ou atualização da empresa.
    /// </summary>
    News = 6
}

/// <summary>
/// Status do post no workflow de publicação corporativa.
/// </summary>
public enum PostStatus
{
    /// <summary>
    /// Rascunho em edição, não visível publicamente.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Aguardando aprovação de moderadores ou gestores.
    /// </summary>
    PendingApproval = 1,

    /// <summary>
    /// Publicado e visível conforme regras de visibilidade.
    /// </summary>
    Published = 2,

    /// <summary>
    /// Arquivado, não visível mas preservado no sistema.
    /// </summary>
    Archived = 3,

    /// <summary>
    /// Rejeitado na aprovação, não será publicado.
    /// </summary>
    Rejected = 4
}

/// <summary>
/// Nível de visibilidade do post na estrutura organizacional.
/// </summary>
public enum PostVisibility
{
    /// <summary>
    /// Visível para todos os funcionários da organização.
    /// </summary>
    Public = 0,

    /// <summary>
    /// Visível apenas para funcionários do departamento específico.
    /// </summary>
    Department = 1,

    /// <summary>
    /// Visível apenas para membros da equipe específica.
    /// </summary>
    Team = 2,

    /// <summary>
    /// Visível para toda a empresa (padrão amplo).
    /// </summary>
    Company = 3
}
