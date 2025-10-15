using System.Globalization;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Application.Features.CorporateSearch.Queries;
using SynQcore.Domain.Entities;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.CorporateSearch.Handlers;

public partial class CorporateSearchQueryHandler : IRequestHandler<CorporateSearchQuery, PagedResult<SearchResultDto>>
{
    [GeneratedRegex(@"[^\w\s]")]
    private static partial Regex NonWordCharactersRegex();

    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CorporateSearchQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Executando busca corporativa - Query: {query}, Page: {page}, Filtros: {@filters}")]
    private static partial void LogSearchStarted(ILogger logger, string query, int page, SearchFiltersDto? filters);

    [LoggerMessage(LogLevel.Information, "Busca corporativa concluída - Query: {query}, Resultados: {count}, Tempo: {elapsed}ms")]
    private static partial void LogSearchCompleted(ILogger logger, string query, int count, long elapsed);

    [LoggerMessage(LogLevel.Warning, "Busca corporativa com poucos resultados - Query: {query}, Resultados: {count}")]
    private static partial void LogLowResults(ILogger logger, string query, int count);

    public CorporateSearchQueryHandler(ISynQcoreDbContext context, ILogger<CorporateSearchQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<SearchResultDto>> Handle(CorporateSearchQuery request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        LogSearchStarted(_logger, request.Query ?? "", request.Page, request.Filters);

        var searchTerms = ProcessSearchTerms(request.Query ?? "");
        var allResults = new List<SearchResultDto>();

        // Buscar em diferentes tipos de conteúdo baseado no filtro
        var contentTypes = request.Filters?.ContentTypes ?? new List<string> { "Post", "Document", "Employee", "Template" };

        if (contentTypes.Contains("Post") || contentTypes.Contains("All"))
        {
            var posts = await SearchPostsAsync(searchTerms, request, cancellationToken);
            allResults.AddRange(posts);
        }

        if (contentTypes.Contains("Document") || contentTypes.Contains("All"))
        {
            var documents = await SearchCorporateDocumentsAsync(searchTerms, request, cancellationToken);
            allResults.AddRange(documents);
        }

        if (contentTypes.Contains("Employee") || contentTypes.Contains("All"))
        {
            var employees = await SearchEmployeesAsync(searchTerms, request, cancellationToken);
            allResults.AddRange(employees);
        }

        if (contentTypes.Contains("Template") || contentTypes.Contains("All"))
        {
            var templates = await SearchDocumentTemplatesAsync(searchTerms, request, cancellationToken);
            allResults.AddRange(templates);
        }

        if (contentTypes.Contains("MediaAsset") || contentTypes.Contains("All"))
        {
            var assets = await SearchMediaAssetsAsync(searchTerms, request, cancellationToken);
            allResults.AddRange(assets);
        }

        // Aplicar filtros globais
        var filteredResults = ApplyGlobalFilters(allResults, request.Filters);

        // Calcular relevância e ordenar
        foreach (var result in filteredResults)
        {
            result.RelevanceScore = CalculateRelevanceScore(result, searchTerms);
            result.HighlightedTerms = FindHighlightedTerms(result, searchTerms);
        }

        // Ordenar por relevância e data
        var sortedResults = filteredResults
            .OrderByDescending(r => r.RelevanceScore)
            .ThenByDescending(r => r.UpdatedAt)
            .ToList();

        // Aplicar paginação
        var totalCount = sortedResults.Count;
        var skip = (request.Page - 1) * request.PageSize;
        var pagedResults = sortedResults.Skip(skip).Take(request.PageSize).ToList();

        var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
        LogSearchCompleted(_logger, request.Query ?? "", totalCount, (long)elapsed);

        if (totalCount < 3 && !string.IsNullOrEmpty(request.Query))
        {
            LogLowResults(_logger, request.Query, totalCount);
        }

        return new PagedResult<SearchResultDto>
        {
            Items = pagedResults,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

    private static List<string> ProcessSearchTerms(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<string>();

        // Remover caracteres especiais e dividir por espaços
        var cleanQuery = NonWordCharactersRegex().Replace(query, " ");
        var terms = cleanQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 2) // Termos com mais de 2 caracteres
            .Select(t => t.ToLower(CultureInfo.InvariantCulture))
            .Distinct()
            .ToList();

        return terms;
    }

    private async Task<List<SearchResultDto>> SearchPostsAsync(List<string> searchTerms, CorporateSearchQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.Status == PostStatus.Published);

        // Aplicar busca nos termos
        if (searchTerms.Count > 0)
        {
            query = query.Where(p =>
                searchTerms.Any(t => p.Title.Contains(t)) ||
                searchTerms.Any(t => p.Content.Contains(t)) ||
                searchTerms.Any(t => p.Summary != null && p.Summary.Contains(t)));
        }

        // Aplicar filtros específicos
        query = ApplyPostFilters(query, request.Filters);

        var posts = await query.Take(100).ToListAsync(cancellationToken);

        return posts.Select(p => new SearchResultDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = TruncateContent(p.Content),
            Excerpt = p.Summary ?? TruncateContent(p.Content, 150),
            Type = "Post",
            Category = p.Category?.Name ?? "Discussão",
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            AuthorId = p.AuthorId,
            AuthorName = p.Author?.FullName ?? "Usuário",
            AuthorEmail = p.Author?.Email ?? "",
            AuthorDepartment = p.Department?.Name,
            Tags = ExtractTags(p.PostTags?.Select(pt => pt.Tag.Name).ToList()),
            LikeCount = p.LikeCount,
            CommentCount = p.CommentCount,
            ViewCount = p.ViewCount,
            Metadata = new Dictionary<string, object>
            {
                ["PostType"] = p.Type.ToString(),
                ["Status"] = p.Status.ToString(),
                ["Version"] = p.Version,
                ["RequiresApproval"] = p.RequiresApproval
            }
        }).ToList();
    }

    private async Task<List<SearchResultDto>> SearchCorporateDocumentsAsync(List<string> searchTerms, CorporateSearchQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CorporateDocuments
            .Where(d => d.Status == DocumentStatus.Approved);

        // Aplicar busca nos termos
        if (searchTerms.Count > 0)
        {
            query = query.Where(d =>
                searchTerms.Any(t => d.Title.Contains(t)) ||
                searchTerms.Any(t => d.Description != null && d.Description.Contains(t)));
        }

        // Aplicar filtros específicos
        query = ApplyDocumentFilters(query, request.Filters);

        var documents = await query.Take(100).ToListAsync(cancellationToken);

        return documents.Select(d => new SearchResultDto
        {
            Id = d.Id,
            Title = d.Title,
            Content = d.Description ?? "Documento corporativo",
            Excerpt = TruncateContent(d.Description, 150),
            Type = "Document",
            Category = d.Category.ToString(),
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            AuthorId = d.UploadedByEmployeeId,
            AuthorName = "Funcionário",
            AuthorEmail = "",
            AuthorDepartment = null,
            Tags = ExtractTags(d.Tags),
            Metadata = new Dictionary<string, object>
            {
                ["DocumentType"] = d.Type.ToString(),
                ["AccessLevel"] = d.AccessLevel.ToString(),
                ["Version"] = d.Version,
                ["FileSizeBytes"] = d.FileSizeBytes,
                ["ContentType"] = d.ContentType,
                ["OriginalFileName"] = d.OriginalFileName
            }
        }).ToList();
    }

    private async Task<List<SearchResultDto>> SearchMediaAssetsAsync(List<string> searchTerms, CorporateSearchQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MediaAssets
            .Where(m => m.IsApproved);

        // Aplicar busca nos termos
        if (searchTerms.Count > 0)
        {
            query = query.Where(m =>
                searchTerms.Any(t => m.Name.Contains(t)) ||
                searchTerms.Any(t => m.Description != null && m.Description.Contains(t)));
        }

        // Aplicar filtros específicos
        query = ApplyMediaAssetFilters(query, request.Filters);

        var assets = await query.Take(100).ToListAsync(cancellationToken);

        return assets.Select(m => new SearchResultDto
        {
            Id = m.Id,
            Title = m.Name,
            Content = m.Description ?? "Asset de mídia corporativa",
            Excerpt = TruncateContent(m.Description, 150),
            Type = "MediaAsset",
            Category = m.Category.ToString(),
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            AuthorId = m.UploadedByEmployeeId,
            AuthorName = "Funcionário",
            AuthorEmail = "",
            AuthorDepartment = null,
            Tags = new List<string>(),
            Metadata = new Dictionary<string, object>
            {
                ["AssetType"] = m.Type.ToString(),
                ["AccessLevel"] = m.AccessLevel.ToString(),
                ["FileSizeBytes"] = m.FileSizeBytes,
                ["ContentType"] = m.ContentType,
                ["Width"] = m.Width ?? 0,
                ["Height"] = m.Height ?? 0,
                ["DurationSeconds"] = m.DurationSeconds ?? 0
            }
        }).ToList();
    }

    private async Task<List<SearchResultDto>> SearchDocumentTemplatesAsync(List<string> searchTerms, CorporateSearchQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DocumentTemplates
            .Where(t => t.IsActive);

        // Aplicar busca nos termos
        if (searchTerms.Count > 0)
        {
            query = query.Where(t =>
                searchTerms.Any(term => t.Name.Contains(term)) ||
                searchTerms.Any(term => t.Description != null && t.Description.Contains(term)));
        }

        // Aplicar filtros específicos
        query = ApplyTemplateFilters(query, request.Filters);

        var templates = await query.Take(100).ToListAsync(cancellationToken);

        return templates.Select(t => new SearchResultDto
        {
            Id = t.Id,
            Title = t.Name,
            Content = t.Description ?? "Template de documento corporativo",
            Excerpt = TruncateContent(t.Description, 150),
            Type = "Template",
            Category = t.DefaultCategory.ToString(),
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            AuthorId = t.CreatedByEmployeeId,
            AuthorName = "Funcionário",
            AuthorEmail = "",
            AuthorDepartment = null,
            Tags = ExtractTags(t.Tags),
            Metadata = new Dictionary<string, object>
            {
                ["DocumentType"] = t.DocumentType.ToString(),
                ["DefaultAccessLevel"] = t.DefaultAccessLevel.ToString(),
                ["Version"] = t.Version,
                ["IsDefault"] = t.IsDefault,
                ["UsageCount"] = t.UsageCount
            }
        }).ToList();
    }

    private async Task<List<SearchResultDto>> SearchEmployeesAsync(List<string> searchTerms, CorporateSearchQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .Where(e => e.IsActive && !e.IsDeleted);

        // Aplicar busca nos termos
        if (searchTerms.Count > 0)
        {
            query = query.Where(e =>
                searchTerms.Any(t => e.FirstName.Contains(t)) ||
                searchTerms.Any(t => e.LastName.Contains(t)) ||
                searchTerms.Any(t => e.Email.Contains(t)) ||
                searchTerms.Any(t => e.Position != null && e.Position.Contains(t)));
        }

        var employees = await query.Take(100).ToListAsync(cancellationToken);

        return employees.Select(e => new SearchResultDto
        {
            Id = e.Id,
            Title = e.FullName,
            Content = $"{e.Position} - {e.Email}",
            Excerpt = $"{e.Position} na {string.Join(", ", e.EmployeeDepartments.Select(ed => ed.Department.Name))}",
            Type = "Employee",
            Category = "Pessoas",
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            AuthorId = e.Id,
            AuthorName = e.FullName,
            AuthorEmail = e.Email,
            AuthorDepartment = e.EmployeeDepartments.FirstOrDefault()?.Department.Name,
            Tags = new List<string>(),
            Metadata = new Dictionary<string, object>
            {
                ["Position"] = e.Position ?? "",
                ["HireDate"] = e.HireDate,
                ["IsManager"] = e.ManagerId == null,
                ["Departments"] = e.EmployeeDepartments.Select(ed => ed.Department.Name).ToList()
            }
        }).ToList();
    }

    // Métodos auxiliares para aplicar filtros específicos
    private static IQueryable<Post> ApplyPostFilters(IQueryable<Post> query, SearchFiltersDto? filters)
    {
        if (filters == null) return query;

        if (filters.Categories?.Count > 0)
        {
            query = query.Where(p => p.Category != null && filters.Categories.Contains(p.Category.Name));
        }

        if (filters.AuthorIds?.Count > 0)
        {
            query = query.Where(p => filters.AuthorIds.Contains(p.AuthorId));
        }

        if (filters.DepartmentIds?.Count > 0)
        {
            query = query.Where(p => p.DepartmentId.HasValue && filters.DepartmentIds.Contains(p.DepartmentId.Value));
        }

        if (filters.CreatedAfter.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= filters.CreatedAfter.Value);
        }

        if (filters.CreatedBefore.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= filters.CreatedBefore.Value);
        }

        return query;
    }

    private static IQueryable<CorporateDocument> ApplyDocumentFilters(IQueryable<CorporateDocument> query, SearchFiltersDto? filters)
    {
        if (filters == null) return query;

        if (filters.Categories?.Count > 0)
        {
            var categories = filters.Categories.Select(c => Enum.Parse<DocumentCategory>(c, true)).ToList();
            query = query.Where(d => categories.Contains(d.Category));
        }

        if (filters.AuthorIds?.Count > 0)
        {
            query = query.Where(d => filters.AuthorIds.Contains(d.UploadedByEmployeeId));
        }

        if (filters.DepartmentIds?.Count > 0)
        {
            query = query.Where(d => d.OwnerDepartmentId.HasValue && filters.DepartmentIds.Contains(d.OwnerDepartmentId.Value));
        }

        if (filters.MinAccessLevel.HasValue)
        {
            query = query.Where(d => (int)d.AccessLevel >= (int)filters.MinAccessLevel.Value);
        }

        if (filters.CreatedAfter.HasValue)
        {
            query = query.Where(d => d.CreatedAt >= filters.CreatedAfter.Value);
        }

        if (filters.CreatedBefore.HasValue)
        {
            query = query.Where(d => d.CreatedAt <= filters.CreatedBefore.Value);
        }

        return query;
    }

    private static IQueryable<MediaAsset> ApplyMediaAssetFilters(IQueryable<MediaAsset> query, SearchFiltersDto? filters)
    {
        if (filters == null) return query;

        if (filters.Categories?.Count > 0)
        {
            // Converter categorias para MediaAssetCategory enum
            var mediaCategories = filters.Categories
                .Where(c => Enum.TryParse<MediaAssetCategory>(c, out _))
                .Select(c => Enum.Parse<MediaAssetCategory>(c))
                .ToList();

            if (mediaCategories.Count > 0)
            {
                query = query.Where(m => mediaCategories.Contains(m.Category));
            }
        }

        if (filters.AuthorIds?.Count > 0)
        {
            query = query.Where(m => filters.AuthorIds.Contains(m.UploadedByEmployeeId));
        }

        if (filters.CreatedAfter.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= filters.CreatedAfter.Value);
        }

        if (filters.CreatedBefore.HasValue)
        {
            query = query.Where(m => m.CreatedAt <= filters.CreatedBefore.Value);
        }

        return query;
    }

    private static IQueryable<DocumentTemplate> ApplyTemplateFilters(IQueryable<DocumentTemplate> query, SearchFiltersDto? filters)
    {
        if (filters == null) return query;

        if (filters.Categories?.Count > 0)
        {
            // Converter categorias para DocumentCategory enum
            var docCategories = filters.Categories
                .Where(c => Enum.TryParse<DocumentCategory>(c, out _))
                .Select(c => Enum.Parse<DocumentCategory>(c))
                .ToList();

            if (docCategories.Count > 0)
            {
                query = query.Where(t => docCategories.Contains(t.DefaultCategory));
            }
        }

        if (filters.AuthorIds?.Count > 0)
        {
            query = query.Where(t => filters.AuthorIds.Contains(t.CreatedByEmployeeId));
        }

        if (filters.DepartmentIds?.Count > 0)
        {
            query = query.Where(t => t.OwnerDepartmentId.HasValue && filters.DepartmentIds.Contains(t.OwnerDepartmentId.Value));
        }

        if (filters.CreatedAfter.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= filters.CreatedAfter.Value);
        }

        if (filters.CreatedBefore.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= filters.CreatedBefore.Value);
        }

        return query;
    }

    private static List<SearchResultDto> ApplyGlobalFilters(List<SearchResultDto> results, SearchFiltersDto? filters)
    {
        if (filters == null) return results;

        if (filters.Tags?.Count > 0)
        {
            results = results.Where(r => r.Tags.Any(tag => filters.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        if (filters.MinEngagement.HasValue)
        {
            results = results.Where(r => (r.LikeCount + r.CommentCount + r.ViewCount) >= filters.MinEngagement.Value).ToList();
        }

        return results;
    }

    private static float CalculateRelevanceScore(SearchResultDto result, List<string> searchTerms)
    {
        if (searchTerms.Count == 0) return 1.0f;

        float score = 0.0f;
        var titleLower = result.Title.ToLower(CultureInfo.InvariantCulture);
        var contentLower = result.Content.ToLower(CultureInfo.InvariantCulture);

        foreach (var term in searchTerms)
        {
            // Título tem peso maior
            if (titleLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 3.0f;
                if (titleLower.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                    score += 2.0f; // Boost para match no início
            }

            // Conteúdo tem peso menor
            if (contentLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 1.0f;
            }

            // Tags têm peso médio
            if (result.Tags.Any(tag => tag.ToLower(CultureInfo.InvariantCulture).Contains(term, StringComparison.OrdinalIgnoreCase)))
            {
                score += 2.0f;
            }
        }

        return MathF.Max(score / searchTerms.Count, 0.1f);
    }

    private static List<string> FindHighlightedTerms(SearchResultDto result, List<string> searchTerms)
    {
        var highlighted = new List<string>();
        var titleLower = result.Title.ToLower(CultureInfo.InvariantCulture);
        var contentLower = result.Content.ToLower(CultureInfo.InvariantCulture);

        foreach (var term in searchTerms)
        {
            if (titleLower.Contains(term, StringComparison.OrdinalIgnoreCase) || contentLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                highlighted.Add(term);
            }
        }

        return highlighted;
    }

    private static List<string> ExtractTags(string? tagsString)
    {
        if (string.IsNullOrWhiteSpace(tagsString))
            return new List<string>();

        return tagsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();
    }

    private static List<string> ExtractTags(List<string>? tagsList)
    {
        return tagsList ?? new List<string>();
    }

    private static string TruncateContent(string? content, int maxLength = 200)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length <= maxLength)
            return content ?? string.Empty;

        var truncated = content.AsSpan(0, maxLength);
        var lastSpace = truncated.LastIndexOf(' ');

        return lastSpace > 0
            ? string.Concat(truncated[..lastSpace], "...")
            : string.Concat(truncated, "...");
    }
}
