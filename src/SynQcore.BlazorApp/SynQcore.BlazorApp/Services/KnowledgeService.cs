/*
 * SynQcore - Corporate Social Network
 *
 * Implementação do Serviço de Gestão de Conhecimento
 * Fornece integração completa com API de base de conhecimento corporativo
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.Globalization;
using System.Text;
using System.Text.Json;
using SynQcore.BlazorApp.Models;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Implementação do serviço de gestão de conhecimento corporativo
/// Integra com a API backend para gerenciamento de posts e categorias de conhecimento
/// </summary>
public partial class KnowledgeService : IKnowledgeService
{
    private readonly IApiService _apiService;
    private readonly ILogger<KnowledgeService> _logger;

    // Event IDs para logging estruturado (5001-5032)
    [LoggerMessage(EventId = 5001, Level = LogLevel.Information,
        Message = "Buscando posts de conhecimento - Termo: {SearchTerm}, Categoria: {CategoryId}, Tipo: {Type}, Page: {Page}, PageSize: {PageSize}")]
    private static partial void LogSearchKnowledgePosts(ILogger logger, string? searchTerm, Guid? categoryId, PostType? type, int page, int pageSize);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Information,
        Message = "Busca de posts concluída - Total encontrado: {TotalCount}, Página {Page} de {TotalPages}")]
    private static partial void LogSearchKnowledgePostsSuccess(ILogger logger, int totalCount, int page, int totalPages);

    [LoggerMessage(EventId = 5003, Level = LogLevel.Information,
        Message = "Obtendo post de conhecimento por ID: {PostId}, IncrementView: {IncrementView}")]
    private static partial void LogGetKnowledgePostById(ILogger logger, Guid postId, bool incrementView);

    [LoggerMessage(EventId = 5004, Level = LogLevel.Information,
        Message = "Post de conhecimento encontrado - ID: {PostId}, Título: {Title}, Autor: {AuthorName}")]
    private static partial void LogGetKnowledgePostByIdSuccess(ILogger logger, Guid postId, string title, string authorName);

    [LoggerMessage(EventId = 5005, Level = LogLevel.Warning,
        Message = "Post de conhecimento não encontrado - ID: {PostId}")]
    private static partial void LogKnowledgePostNotFound(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 5006, Level = LogLevel.Information,
        Message = "Obtendo posts por categoria: {CategoryId}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}")]
    private static partial void LogGetKnowledgePostsByCategory(ILogger logger, Guid categoryId, int page, int pageSize, string? sortBy);

    [LoggerMessage(EventId = 5007, Level = LogLevel.Information,
        Message = "Posts por categoria obtidos - CategoryId: {CategoryId}, Total: {Count}")]
    private static partial void LogGetKnowledgePostsByCategorySuccess(ILogger logger, Guid categoryId, int count);

    [LoggerMessage(EventId = 5008, Level = LogLevel.Information,
        Message = "Obtendo versões do post: {PostId}")]
    private static partial void LogGetKnowledgePostVersions(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 5009, Level = LogLevel.Information,
        Message = "Versões obtidas - PostId: {PostId}, Total: {Count}")]
    private static partial void LogGetKnowledgePostVersionsSuccess(ILogger logger, Guid postId, int count);

    [LoggerMessage(EventId = 5010, Level = LogLevel.Information,
        Message = "Criando post de conhecimento - Título: {Title}, Tipo: {Type}, Categoria: {CategoryId}")]
    private static partial void LogCreateKnowledgePost(ILogger logger, string title, PostType type, Guid? categoryId);

    [LoggerMessage(EventId = 5011, Level = LogLevel.Information,
        Message = "Post de conhecimento criado com sucesso - ID: {PostId}, Título: {Title}")]
    private static partial void LogCreateKnowledgePostSuccess(ILogger logger, Guid postId, string title);

    [LoggerMessage(EventId = 5012, Level = LogLevel.Information,
        Message = "Atualizando post de conhecimento - ID: {PostId}, Título: {Title}")]
    private static partial void LogUpdateKnowledgePost(ILogger logger, Guid postId, string? title);

    [LoggerMessage(EventId = 5013, Level = LogLevel.Information,
        Message = "Post de conhecimento atualizado com sucesso - ID: {PostId}")]
    private static partial void LogUpdateKnowledgePostSuccess(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 5014, Level = LogLevel.Information,
        Message = "Excluindo post de conhecimento - ID: {PostId}")]
    private static partial void LogDeleteKnowledgePost(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 5015, Level = LogLevel.Information,
        Message = "Post de conhecimento excluído com sucesso - ID: {PostId}")]
    private static partial void LogDeleteKnowledgePostSuccess(ILogger logger, Guid postId);

    [LoggerMessage(EventId = 5016, Level = LogLevel.Information,
        Message = "Obtendo categorias - IncludeInactive: {IncludeInactive}, IncludeHierarchy: {IncludeHierarchy}")]
    private static partial void LogGetCategories(ILogger logger, bool includeInactive, bool includeHierarchy);

    [LoggerMessage(EventId = 5017, Level = LogLevel.Information,
        Message = "Categorias obtidas - Total: {Count}")]
    private static partial void LogGetCategoriesSuccess(ILogger logger, int count);

    [LoggerMessage(EventId = 5018, Level = LogLevel.Information,
        Message = "Obtendo categoria por ID: {CategoryId}")]
    private static partial void LogGetCategoryById(ILogger logger, Guid categoryId);

    [LoggerMessage(EventId = 5019, Level = LogLevel.Information,
        Message = "Categoria encontrada - ID: {CategoryId}, Nome: {Name}")]
    private static partial void LogGetCategoryByIdSuccess(ILogger logger, Guid categoryId, string name);

    [LoggerMessage(EventId = 5020, Level = LogLevel.Warning,
        Message = "Categoria não encontrada - ID: {CategoryId}")]
    private static partial void LogCategoryNotFound(ILogger logger, Guid categoryId);

    [LoggerMessage(EventId = 5021, Level = LogLevel.Information,
        Message = "Criando categoria - Nome: {Name}, Cor: {Color}, Parent: {ParentCategoryId}")]
    private static partial void LogCreateCategory(ILogger logger, string name, string color, Guid? parentCategoryId);

    [LoggerMessage(EventId = 5022, Level = LogLevel.Information,
        Message = "Categoria criada com sucesso - ID: {CategoryId}, Nome: {Name}")]
    private static partial void LogCreateCategorySuccess(ILogger logger, Guid categoryId, string name);

    [LoggerMessage(EventId = 5023, Level = LogLevel.Information,
        Message = "Atualizando categoria - ID: {CategoryId}, Nome: {Name}")]
    private static partial void LogUpdateCategory(ILogger logger, Guid categoryId, string? name);

    [LoggerMessage(EventId = 5024, Level = LogLevel.Information,
        Message = "Categoria atualizada com sucesso - ID: {CategoryId}")]
    private static partial void LogUpdateCategorySuccess(ILogger logger, Guid categoryId);

    [LoggerMessage(EventId = 5025, Level = LogLevel.Information,
        Message = "Excluindo categoria - ID: {CategoryId}")]
    private static partial void LogDeleteCategory(ILogger logger, Guid categoryId);

    [LoggerMessage(EventId = 5026, Level = LogLevel.Information,
        Message = "Categoria excluída com sucesso - ID: {CategoryId}")]
    private static partial void LogDeleteCategorySuccess(ILogger logger, Guid categoryId);

    [LoggerMessage(EventId = 5027, Level = LogLevel.Error,
        Message = "Erro ao buscar posts de conhecimento - Erro: {Error}")]
    private static partial void LogSearchKnowledgePostsError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 5028, Level = LogLevel.Error,
        Message = "Erro ao obter post por ID - PostId: {PostId}, Erro: {Error}")]
    private static partial void LogGetKnowledgePostByIdError(ILogger logger, Guid postId, string error, Exception? exception);

    [LoggerMessage(EventId = 5029, Level = LogLevel.Error,
        Message = "Erro ao obter posts por categoria - CategoryId: {CategoryId}, Erro: {Error}")]
    private static partial void LogGetKnowledgePostsByCategoryError(ILogger logger, Guid categoryId, string error, Exception? exception);

    [LoggerMessage(EventId = 5030, Level = LogLevel.Error,
        Message = "Erro ao obter versões - PostId: {PostId}, Erro: {Error}")]
    private static partial void LogGetKnowledgePostVersionsError(ILogger logger, Guid postId, string error, Exception? exception);

    [LoggerMessage(EventId = 5031, Level = LogLevel.Error,
        Message = "Erro ao criar/atualizar/excluir post - Erro: {Error}")]
    private static partial void LogKnowledgePostCrudError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 5032, Level = LogLevel.Error,
        Message = "Erro ao gerenciar categoria - Erro: {Error}")]
    private static partial void LogCategoryCrudError(ILogger logger, string error, Exception? exception);

    [LoggerMessage(EventId = 5033, Level = LogLevel.Warning,
        Message = "Usando dados de fallback para conhecimento - Motivo: {Reason}")]
    private static partial void LogUsingFallbackData(ILogger logger, string reason);

    public KnowledgeService(IApiService apiService, ILogger<KnowledgeService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    #region Posts de Conhecimento

    public async Task<PagedResult<KnowledgePostDto>> SearchKnowledgePostsAsync(KnowledgePostSearchRequest searchRequest)
    {
        LogSearchKnowledgePosts(_logger, searchRequest.SearchTerm, searchRequest.CategoryId, searchRequest.Type, searchRequest.Page, searchRequest.PageSize);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["searchTerm"] = searchRequest.SearchTerm,
                ["type"] = searchRequest.Type?.ToString(),
                ["status"] = searchRequest.Status?.ToString(),
                ["visibility"] = searchRequest.Visibility?.ToString(),
                ["categoryId"] = searchRequest.CategoryId,
                ["authorId"] = searchRequest.AuthorId,
                ["departmentId"] = searchRequest.DepartmentId,
                ["teamId"] = searchRequest.TeamId,
                ["createdAfter"] = searchRequest.CreatedAfter?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["createdBefore"] = searchRequest.CreatedBefore?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["sortBy"] = searchRequest.SortBy,
                ["sortDescending"] = searchRequest.SortDescending,
                ["page"] = searchRequest.Page,
                ["pageSize"] = searchRequest.PageSize
            });

            var response = await _apiService.GetAsync<PagedResult<KnowledgePostDto>>($"api/knowledgeposts{queryParams}");

            if (response != null)
            {
                LogSearchKnowledgePostsSuccess(_logger, response.TotalCount, response.Page, response.TotalPages);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackKnowledgePosts(searchRequest);
        }
        catch (Exception ex)
        {
            LogSearchKnowledgePostsError(_logger, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackKnowledgePosts(searchRequest);
        }
    }

    public async Task<KnowledgePostDto?> GetKnowledgePostByIdAsync(Guid id, bool incrementViewCount = true)
    {
        LogGetKnowledgePostById(_logger, id, incrementViewCount);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["incrementViewCount"] = incrementViewCount
            });

            var response = await _apiService.GetAsync<KnowledgePostDto>($"api/knowledgeposts/{id}{queryParams}");

            if (response != null)
            {
                LogGetKnowledgePostByIdSuccess(_logger, response.Id, response.Title, response.AuthorName);
                return response;
            }

            LogKnowledgePostNotFound(_logger, id);
            return null;
        }
        catch (Exception ex)
        {
            LogGetKnowledgePostByIdError(_logger, id, ex.Message, ex);
            return null;
        }
    }

    public async Task<PagedResult<KnowledgePostDto>> GetKnowledgePostsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20, string? sortBy = "CreatedAt", bool sortDescending = true)
    {
        LogGetKnowledgePostsByCategory(_logger, categoryId, page, pageSize, sortBy);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["page"] = page,
                ["pageSize"] = pageSize,
                ["sortBy"] = sortBy,
                ["sortDescending"] = sortDescending
            });

            var response = await _apiService.GetAsync<PagedResult<KnowledgePostDto>>($"api/knowledgeposts/category/{categoryId}{queryParams}");

            if (response != null)
            {
                LogGetKnowledgePostsByCategorySuccess(_logger, categoryId, response.TotalCount);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackKnowledgePostsByCategory(categoryId, page, pageSize);
        }
        catch (Exception ex)
        {
            LogGetKnowledgePostsByCategoryError(_logger, categoryId, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackKnowledgePostsByCategory(categoryId, page, pageSize);
        }
    }

    public async Task<List<KnowledgePostDto>> GetKnowledgePostVersionsAsync(Guid id)
    {
        LogGetKnowledgePostVersions(_logger, id);

        try
        {
            var response = await _apiService.GetAsync<List<KnowledgePostDto>>($"api/knowledgeposts/{id}/versions");

            if (response != null)
            {
                LogGetKnowledgePostVersionsSuccess(_logger, id, response.Count);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackVersions(id);
        }
        catch (Exception ex)
        {
            LogGetKnowledgePostVersionsError(_logger, id, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackVersions(id);
        }
    }

    public async Task<KnowledgePostDto> CreateKnowledgePostAsync(CreateKnowledgePostRequest request)
    {
        LogCreateKnowledgePost(_logger, request.Title, request.Type, request.CategoryId);

        try
        {
            var response = await _apiService.PostAsync<KnowledgePostDto>("api/knowledgeposts", request);

            if (response != null)
            {
                LogCreateKnowledgePostSuccess(_logger, response.Id, response.Title);
                return response;
            }

            throw new InvalidOperationException("Erro ao criar post de conhecimento: resposta da API foi null");
        }
        catch (Exception ex)
        {
            LogKnowledgePostCrudError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<KnowledgePostDto> UpdateKnowledgePostAsync(Guid id, UpdateKnowledgePostRequest request)
    {
        LogUpdateKnowledgePost(_logger, id, request.Title);

        try
        {
            var response = await _apiService.PutAsync<KnowledgePostDto>($"api/knowledgeposts/{id}", request);

            if (response != null)
            {
                LogUpdateKnowledgePostSuccess(_logger, response.Id);
                return response;
            }

            throw new InvalidOperationException("Erro ao atualizar post de conhecimento: resposta da API foi null");
        }
        catch (Exception ex)
        {
            LogKnowledgePostCrudError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<bool> DeleteKnowledgePostAsync(Guid id)
    {
        LogDeleteKnowledgePost(_logger, id);

        try
        {
            await _apiService.DeleteAsync($"api/knowledgeposts/{id}");

            LogDeleteKnowledgePostSuccess(_logger, id);
            return true;
        }
        catch (Exception ex)
        {
            LogKnowledgePostCrudError(_logger, ex.Message, ex);
            return false;
        }
    }

    #endregion

    #region Categorias de Conhecimento

    public async Task<List<KnowledgeCategoryDto>> GetCategoriesAsync(bool includeInactive = false, bool includeHierarchy = true)
    {
        LogGetCategories(_logger, includeInactive, includeHierarchy);

        try
        {
            var queryParams = BuildQueryParams(new Dictionary<string, object?>
            {
                ["includeInactive"] = includeInactive,
                ["includeHierarchy"] = includeHierarchy
            });

            var response = await _apiService.GetAsync<List<KnowledgeCategoryDto>>($"api/knowledgecategories{queryParams}");

            if (response != null)
            {
                LogGetCategoriesSuccess(_logger, response.Count);
                return response;
            }

            LogUsingFallbackData(_logger, "API retornou null");
            return GetFallbackCategories();
        }
        catch (Exception ex)
        {
            LogCategoryCrudError(_logger, ex.Message, ex);
            LogUsingFallbackData(_logger, $"Erro na API: {ex.Message}");
            return GetFallbackCategories();
        }
    }

    public async Task<KnowledgeCategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        LogGetCategoryById(_logger, id);

        try
        {
            var response = await _apiService.GetAsync<KnowledgeCategoryDto>($"api/knowledgecategories/{id}");

            if (response != null)
            {
                LogGetCategoryByIdSuccess(_logger, response.Id, response.Name);
                return response;
            }

            LogCategoryNotFound(_logger, id);
            return null;
        }
        catch (Exception ex)
        {
            LogCategoryCrudError(_logger, ex.Message, ex);
            return null;
        }
    }

    public async Task<KnowledgeCategoryDto> CreateCategoryAsync(CreateKnowledgeCategoryRequest request)
    {
        LogCreateCategory(_logger, request.Name, request.Color, request.ParentCategoryId);

        try
        {
            var response = await _apiService.PostAsync<KnowledgeCategoryDto>("api/knowledgecategories", request);

            if (response != null)
            {
                LogCreateCategorySuccess(_logger, response.Id, response.Name);
                return response;
            }

            throw new InvalidOperationException("Erro ao criar categoria: resposta da API foi null");
        }
        catch (Exception ex)
        {
            LogCategoryCrudError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<KnowledgeCategoryDto> UpdateCategoryAsync(Guid id, UpdateKnowledgeCategoryRequest request)
    {
        LogUpdateCategory(_logger, id, request.Name);

        try
        {
            var response = await _apiService.PutAsync<KnowledgeCategoryDto>($"api/knowledgecategories/{id}", request);

            if (response != null)
            {
                LogUpdateCategorySuccess(_logger, response.Id);
                return response;
            }

            throw new InvalidOperationException("Erro ao atualizar categoria: resposta da API foi null");
        }
        catch (Exception ex)
        {
            LogCategoryCrudError(_logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        LogDeleteCategory(_logger, id);

        try
        {
            await _apiService.DeleteAsync($"api/knowledgecategories/{id}");

            LogDeleteCategorySuccess(_logger, id);
            return true;
        }
        catch (Exception ex)
        {
            LogCategoryCrudError(_logger, ex.Message, ex);
            return false;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper para construir query parameters
    /// </summary>
    private static string BuildQueryParams(Dictionary<string, object?> parameters)
    {
        var queryParams = parameters
            .Where(p => p.Value != null)
            .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!.ToString()!)}")
            .ToList();

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

    #endregion

    #region Fallback Data

    /// <summary>
    /// Dados de fallback para teste e desenvolvimento
    /// </summary>
    private static PagedResult<KnowledgePostDto> GetFallbackKnowledgePosts(KnowledgePostSearchRequest request)
    {
        var mockPosts = new List<KnowledgePostDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Title = "Guia Completo de Onboarding 2024",
                Content = "Este guia abrangente apresenta todos os procedimentos necessários para um onboarding eficaz...",
                Summary = "Procedimentos completos para integração de novos funcionários",
                Type = PostType.Article,
                Status = PostStatus.Published,
                Visibility = PostVisibility.Company,
                Version = "2.1",
                ViewCount = 342,
                LikeCount = 28,
                CommentCount = 12,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Ana Silva",
                AuthorEmail = "ana.silva@synqcore.com",
                CategoryId = Guid.NewGuid(),
                CategoryName = "Recursos Humanos",
                DepartmentName = "RH",
                Tags = [new TagDto { Name = "onboarding", Color = "#007ACC" }, new TagDto { Name = "rh", Color = "#28a745" }],
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "Políticas de Segurança da Informação",
                Content = "Diretrizes essenciais para manter a segurança dos dados corporativos...",
                Summary = "Políticas e procedimentos de segurança digital",
                Type = PostType.Policy,
                Status = PostStatus.Published,
                Visibility = PostVisibility.Company,
                Version = "1.3",
                ViewCount = 567,
                LikeCount = 45,
                CommentCount = 8,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Carlos Santos",
                AuthorEmail = "carlos.santos@synqcore.com",
                CategoryId = Guid.NewGuid(),
                CategoryName = "Tecnologia",
                DepartmentName = "TI",
                Tags = [new TagDto { Name = "seguranca", Color = "#dc3545" }, new TagDto { Name = "politicas", Color = "#6f42c1" }],
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "Manual de Vendas - Técnicas Avançadas",
                Content = "Estratégias e técnicas comprovadas para otimização do processo de vendas...",
                Summary = "Métodos avançados para melhoria de performance em vendas",
                Type = PostType.HowTo,
                Status = PostStatus.Published,
                Visibility = PostVisibility.Department,
                Version = "1.0",
                ViewCount = 234,
                LikeCount = 19,
                CommentCount = 15,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Maria Oliveira",
                AuthorEmail = "maria.oliveira@synqcore.com",
                CategoryId = Guid.NewGuid(),
                CategoryName = "Vendas",
                DepartmentName = "Comercial",
                Tags = [new TagDto { Name = "vendas", Color = "#28a745" }, new TagDto { Name = "tecnicas", Color = "#17a2b8" }],
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        return new PagedResult<KnowledgePostDto>
        {
            Items = mockPosts.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
            TotalCount = mockPosts.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static PagedResult<KnowledgePostDto> GetFallbackKnowledgePostsByCategory(Guid categoryId, int page, int pageSize)
    {
        var mockPosts = new List<KnowledgePostDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Title = "Artigo da Categoria Específica",
                Content = "Conteúdo específico da categoria...",
                Summary = "Resumo do artigo específico",
                Type = PostType.Article,
                Status = PostStatus.Published,
                Visibility = PostVisibility.Company,
                CategoryId = categoryId,
                CategoryName = "Categoria Teste",
                AuthorName = "Autor Teste",
                AuthorEmail = "autor@synqcore.com",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        return new PagedResult<KnowledgePostDto>
        {
            Items = mockPosts.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            TotalCount = mockPosts.Count,
            Page = page,
            PageSize = pageSize
        };
    }

    private static List<KnowledgePostDto> GetFallbackVersions(Guid postId)
    {
        return new List<KnowledgePostDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Title = "Versão 1.0",
                Content = "Primeira versão do documento...",
                Version = "1.0",
                ParentPostId = postId,
                AuthorName = "Autor Original",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "Versão 1.1",
                Content = "Versão revisada com melhorias...",
                Version = "1.1",
                ParentPostId = postId,
                AuthorName = "Autor Revisor",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };
    }

    private static List<KnowledgeCategoryDto> GetFallbackCategories()
    {
        return new List<KnowledgeCategoryDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "Recursos Humanos",
                Description = "Políticas e procedimentos de RH",
                Color = "#28a745",
                Icon = "👥",
                IsActive = true,
                PostsCount = 15,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow.AddDays(-10),
                SubCategories = [
                    new KnowledgeCategoryDto {
                        Id = Guid.NewGuid(),
                        Name = "Onboarding",
                        Description = "Processos de integração",
                        Color = "#007ACC",
                        Icon = "🎯",
                        IsActive = true,
                        PostsCount = 8,
                        CreatedAt = DateTime.UtcNow.AddDays(-50)
                    }
                ]
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Tecnologia",
                Description = "Documentação técnica e políticas de TI",
                Color = "#6f42c1",
                Icon = "💻",
                IsActive = true,
                PostsCount = 23,
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                SubCategories = [
                    new KnowledgeCategoryDto {
                        Id = Guid.NewGuid(),
                        Name = "Segurança",
                        Description = "Políticas de segurança da informação",
                        Color = "#dc3545",
                        Icon = "🔒",
                        IsActive = true,
                        PostsCount = 12,
                        CreatedAt = DateTime.UtcNow.AddDays(-80)
                    }
                ]
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Vendas",
                Description = "Materiais e técnicas de vendas",
                Color = "#ffc107",
                Icon = "📈",
                IsActive = true,
                PostsCount = 18,
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };
    }

    #endregion
}
