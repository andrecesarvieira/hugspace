using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.DTOs;

/// <summary>
/// DTO para visualização de posts de conhecimento corporativo.
/// Representa artigos, documentos e conteúdo educacional da organização.
/// </summary>
public class KnowledgePostDto
{
    /// <summary>
    /// Identificador único do post de conhecimento.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Título do post de conhecimento.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Conteúdo completo do post.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Resumo opcional do conteúdo.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// URL da imagem de capa (opcional).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URL de documento anexo (opcional).
    /// </summary>
    public string? DocumentUrl { get; set; }

    /// <summary>
    /// Tipo do post (Article, Documentation, Tutorial, etc.).
    /// </summary>
    public PostType Type { get; set; }

    /// <summary>
    /// Status atual do post (Draft, Published, Archived).
    /// </summary>
    public PostStatus Status { get; set; }

    /// <summary>
    /// Nível de visibilidade do post na organização.
    /// </summary>
    public PostVisibility Visibility { get; set; }

    /// <summary>
    /// Versão atual do post para controle de versionamento.
    /// </summary>
    public string Version { get; set; } = null!;

    /// <summary>
    /// Indica se o post requer aprovação antes da publicação.
    /// </summary>
    public bool RequiresApproval { get; set; }

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
    /// ID do autor do post.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Nome completo do autor.
    /// </summary>
    public string AuthorName { get; set; } = null!;

    /// <summary>
    /// Email do autor.
    /// </summary>
    public string AuthorEmail { get; set; } = null!;

    /// <summary>
    /// ID da categoria de conhecimento (opcional).
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Nome da categoria de conhecimento.
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// ID do departamento associado (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Nome do departamento associado.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// ID da equipe associada (opcional).
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Nome da equipe associada.
    /// </summary>
    public string? TeamName { get; set; }

    /// <summary>
    /// ID do post pai para hierarquia de conteúdo.
    /// </summary>
    public Guid? ParentPostId { get; set; }

    /// <summary>
    /// Título do post pai.
    /// </summary>
    public string? ParentPostTitle { get; set; }

    /// <summary>
    /// Lista de tags associadas ao post.
    /// </summary>
    public List<TagDto> Tags { get; set; } = new();

    /// <summary>
    /// Lista de versões anteriores do post.
    /// </summary>
    public List<KnowledgePostDto> Versions { get; set; } = new();

    /// <summary>
    /// Data e hora de criação do post.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para criação de novo post de conhecimento.
/// Define os dados necessários para criar conteúdo educacional.
/// </summary>
public class CreateKnowledgePostDto
{
    /// <summary>
    /// Título do novo post de conhecimento.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Conteúdo completo do post.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Resumo opcional do conteúdo.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// URL da imagem de capa (opcional).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URL de documento anexo (opcional).
    /// </summary>
    public string? DocumentUrl { get; set; }

    /// <summary>
    /// Tipo do post (padrão: Article).
    /// </summary>
    public PostType Type { get; set; } = PostType.Article;

    /// <summary>
    /// Status inicial do post (padrão: Draft).
    /// </summary>
    public PostStatus Status { get; set; } = PostStatus.Draft;

    /// <summary>
    /// Visibilidade do post (padrão: Company).
    /// </summary>
    public PostVisibility Visibility { get; set; } = PostVisibility.Company;

    /// <summary>
    /// Indica se requer aprovação antes da publicação.
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// ID da categoria de conhecimento (opcional).
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// ID do departamento associado (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// ID da equipe associada (opcional).
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// ID do post pai para hierarquia (opcional).
    /// </summary>
    public Guid? ParentPostId { get; set; }

    /// <summary>
    /// Lista de IDs das tags a serem associadas.
    /// </summary>
    public List<Guid> TagIds { get; set; } = new();
}

/// <summary>
/// DTO para atualização de post de conhecimento existente.
/// Permite modificação de propriedades específicas.
/// </summary>
public class UpdateKnowledgePostDto
{
    /// <summary>
    /// Novo título do post (opcional).
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Novo conteúdo do post (opcional).
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Novo resumo do post (opcional).
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Nova URL da imagem (opcional).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Nova URL do documento (opcional).
    /// </summary>
    public string? DocumentUrl { get; set; }

    /// <summary>
    /// Novo tipo do post (opcional).
    /// </summary>
    public PostType? Type { get; set; }

    /// <summary>
    /// Novo status do post (opcional).
    /// </summary>
    public PostStatus? Status { get; set; }

    /// <summary>
    /// Nova visibilidade do post (opcional).
    /// </summary>
    public PostVisibility? Visibility { get; set; }

    /// <summary>
    /// Nova configuração de aprovação (opcional).
    /// </summary>
    public bool? RequiresApproval { get; set; }

    /// <summary>
    /// Novo ID da categoria (opcional).
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Novo ID do departamento (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Novo ID da equipe (opcional).
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Nova lista de IDs das tags (substitui todas as existentes).
    /// </summary>
    public List<Guid>? TagIds { get; set; }
}

/// <summary>
/// DTO para busca avançada de posts de conhecimento.
/// Oferece múltiplos filtros e opções de paginação.
/// </summary>
public class KnowledgePostSearchDto
{
    /// <summary>
    /// Termo de busca textual no título e conteúdo.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filtrar por tipo específico de post.
    /// </summary>
    public PostType? Type { get; set; }

    /// <summary>
    /// Filtrar por status específico.
    /// </summary>
    public PostStatus? Status { get; set; }

    /// <summary>
    /// Filtrar por nível de visibilidade.
    /// </summary>
    public PostVisibility? Visibility { get; set; }

    /// <summary>
    /// Filtrar por categoria específica.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Filtrar por autor específico.
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// Filtrar por departamento específico.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Filtrar por equipe específica.
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Filtrar por tags específicas.
    /// </summary>
    public List<Guid>? TagIds { get; set; }

    /// <summary>
    /// Filtrar posts criados após esta data.
    /// </summary>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filtrar posts criados antes desta data.
    /// </summary>
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// Campo para ordenação (padrão: "CreatedAt").
    /// </summary>
    public string? SortBy { get; set; } = "CreatedAt";

    /// <summary>
    /// Ordenação descendente (padrão: true).
    /// </summary>
    public bool SortDescending { get; set; } = true;

    /// <summary>
    /// Número da página (padrão: 1).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página (padrão: 20).
    /// </summary>
    public int PageSize { get; set; } = 20;
}
