/*
 * SynQcore - Corporate Social Network
 *
 * Interface de Serviço para Gestão de Conhecimento
 * Implementa padrão de base de conhecimento corporativo
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.ComponentModel.DataAnnotations;
using SynQcore.Domain.Entities.Communication;
using SynQcore.BlazorApp.Models;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Interface para gerenciamento de conhecimento corporativo
/// Fornece métodos para posts de conhecimento e categorias
/// </summary>
public interface IKnowledgeService
{
    #region Posts de Conhecimento

    /// <summary>
    /// Buscar posts de conhecimento com filtros e paginação
    /// </summary>
    /// <param name="searchRequest">Critérios de busca</param>
    /// <returns>Lista paginada de posts de conhecimento</returns>
    Task<PagedResult<KnowledgePostDto>> SearchKnowledgePostsAsync(KnowledgePostSearchRequest searchRequest);

    /// <summary>
    /// Obter post de conhecimento específico por ID
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <param name="incrementViewCount">Se deve incrementar contador de visualizações</param>
    /// <returns>Detalhes do post de conhecimento ou null se não encontrado</returns>
    Task<KnowledgePostDto?> GetKnowledgePostByIdAsync(Guid id, bool incrementViewCount = true);

    /// <summary>
    /// Obter posts de conhecimento por categoria
    /// </summary>
    /// <param name="categoryId">ID da categoria</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="sortBy">Campo de ordenação</param>
    /// <param name="sortDescending">Se deve ordenar de forma decrescente</param>
    /// <returns>Lista paginada de posts da categoria</returns>
    Task<PagedResult<KnowledgePostDto>> GetKnowledgePostsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20, string? sortBy = "CreatedAt", bool sortDescending = true);

    /// <summary>
    /// Obter histórico de versões de um post
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>Lista de versões do post</returns>
    Task<List<KnowledgePostDto>> GetKnowledgePostVersionsAsync(Guid id);

    /// <summary>
    /// Criar um novo post de conhecimento
    /// </summary>
    /// <param name="request">Dados do post a ser criado</param>
    /// <returns>Post de conhecimento criado</returns>
    Task<KnowledgePostDto> CreateKnowledgePostAsync(CreateKnowledgePostRequest request);

    /// <summary>
    /// Atualizar um post de conhecimento existente
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Post de conhecimento atualizado</returns>
    Task<KnowledgePostDto> UpdateKnowledgePostAsync(Guid id, UpdateKnowledgePostRequest request);

    /// <summary>
    /// Excluir um post de conhecimento
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>True se excluído com sucesso</returns>
    Task<bool> DeleteKnowledgePostAsync(Guid id);

    #endregion

    #region Categorias de Conhecimento

    /// <summary>
    /// Obter todas as categorias de conhecimento
    /// </summary>
    /// <param name="includeInactive">Incluir categorias inativas</param>
    /// <param name="includeHierarchy">Incluir estrutura hierárquica</param>
    /// <returns>Lista de categorias com hierarquia</returns>
    Task<List<KnowledgeCategoryDto>> GetCategoriesAsync(bool includeInactive = false, bool includeHierarchy = true);

    /// <summary>
    /// Obter categoria específica por ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Detalhes da categoria ou null se não encontrada</returns>
    Task<KnowledgeCategoryDto?> GetCategoryByIdAsync(Guid id);

    /// <summary>
    /// Criar uma nova categoria de conhecimento
    /// </summary>
    /// <param name="request">Dados da categoria a ser criada</param>
    /// <returns>Categoria criada</returns>
    Task<KnowledgeCategoryDto> CreateCategoryAsync(CreateKnowledgeCategoryRequest request);

    /// <summary>
    /// Atualizar uma categoria de conhecimento
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Categoria atualizada</returns>
    Task<KnowledgeCategoryDto> UpdateCategoryAsync(Guid id, UpdateKnowledgeCategoryRequest request);

    /// <summary>
    /// Excluir uma categoria de conhecimento
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>True se excluída com sucesso</returns>
    Task<bool> DeleteCategoryAsync(Guid id);

    #endregion
}

#region DTOs para Requests

/// <summary>
/// DTO para criação de post de conhecimento
/// </summary>
public class CreateKnowledgePostRequest
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "O conteúdo é obrigatório")]
    [StringLength(50000, ErrorMessage = "O conteúdo deve ter no máximo 50.000 caracteres")]
    public string Content { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "O resumo deve ter no máximo 500 caracteres")]
    public string? Summary { get; set; }

    [Url(ErrorMessage = "URL da imagem inválida")]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "URL do documento inválida")]
    public string? DocumentUrl { get; set; }

    [Required(ErrorMessage = "O tipo do post é obrigatório")]
    public PostType Type { get; set; } = PostType.Article;

    [Required(ErrorMessage = "O status é obrigatório")]
    public PostStatus Status { get; set; } = PostStatus.Draft;

    [Required(ErrorMessage = "A visibilidade é obrigatória")]
    public PostVisibility Visibility { get; set; } = PostVisibility.Company;

    public bool RequiresApproval { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? TeamId { get; set; }

    public Guid? ParentPostId { get; set; }

    public List<Guid> TagIds { get; set; } = [];
}

/// <summary>
/// DTO para atualização de post de conhecimento
/// </summary>
public class UpdateKnowledgePostRequest
{
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
    public string? Title { get; set; }

    [StringLength(50000, ErrorMessage = "O conteúdo deve ter no máximo 50.000 caracteres")]
    public string? Content { get; set; }

    [StringLength(500, ErrorMessage = "O resumo deve ter no máximo 500 caracteres")]
    public string? Summary { get; set; }

    [Url(ErrorMessage = "URL da imagem inválida")]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "URL do documento inválida")]
    public string? DocumentUrl { get; set; }

    public PostType? Type { get; set; }

    public PostStatus? Status { get; set; }

    public PostVisibility? Visibility { get; set; }

    public bool? RequiresApproval { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? TeamId { get; set; }

    public List<Guid>? TagIds { get; set; }
}

/// <summary>
/// DTO para busca de posts de conhecimento
/// </summary>
public class KnowledgePostSearchRequest
{
    [StringLength(100, ErrorMessage = "O termo de busca deve ter no máximo 100 caracteres")]
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

    [StringLength(50, ErrorMessage = "O campo de ordenação deve ter no máximo 50 caracteres")]
    public string? SortBy { get; set; } = "CreatedAt";

    public bool SortDescending { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que 0")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO para criação de categoria de conhecimento
/// </summary>
public class CreateKnowledgeCategoryRequest
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cor é obrigatória")]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Cor deve estar no formato hexadecimal (#RRGGBB)")]
    public string Color { get; set; } = "#007ACC";

    [Required(ErrorMessage = "O ícone é obrigatório")]
    [StringLength(10, ErrorMessage = "O ícone deve ter no máximo 10 caracteres")]
    public string Icon { get; set; } = "📄";

    public bool IsActive { get; set; } = true;

    public Guid? ParentCategoryId { get; set; }
}

/// <summary>
/// DTO para atualização de categoria de conhecimento
/// </summary>
public class UpdateKnowledgeCategoryRequest
{
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string? Description { get; set; }

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Cor deve estar no formato hexadecimal (#RRGGBB)")]
    public string? Color { get; set; }

    [StringLength(10, ErrorMessage = "O ícone deve ter no máximo 10 caracteres")]
    public string? Icon { get; set; }

    public bool? IsActive { get; set; }

    public Guid? ParentCategoryId { get; set; }
}

#endregion

#region DTOs para Response

/// <summary>
/// DTOs para resposta da API (mapeados dos DTOs do backend)
/// </summary>
public class KnowledgePostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public PostType Type { get; set; }
    public PostStatus Status { get; set; }
    public PostVisibility Visibility { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool RequiresApproval { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
    public Guid? ParentPostId { get; set; }
    public string? ParentPostTitle { get; set; }
    public List<TagDto> Tags { get; set; } = [];
    public List<KnowledgePostDto> Versions { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class KnowledgeCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<KnowledgeCategoryDto> SubCategories { get; set; } = [];
    public int PostsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

#endregion
