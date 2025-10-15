using System.Globalization;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Application.Features.CorporateSearch.Queries;
using SynQcore.Domain.Entities;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.CorporateSearch.Handlers;

public partial class AdvancedSearchQueryHandler : IRequestHandler<AdvancedSearchQuery, PagedResult<SearchResultDto>>
{
    [GeneratedRegex(@"[^\w\s]")]
    private static partial Regex NonWordCharactersRegex();

    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<AdvancedSearchQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(LogLevel.Information, "Executando busca avançada - Query: {query}, Title: {title}, Author: {author}")]
    private static partial void LogAdvancedSearchStarted(ILogger logger, string? query, string? title, string? author);

    [LoggerMessage(LogLevel.Information, "Busca avançada concluída - Resultados: {count}, Tempo: {elapsed}ms")]
    private static partial void LogAdvancedSearchCompleted(ILogger logger, int count, long elapsed);

    [LoggerMessage(LogLevel.Warning, "Busca avançada com poucos resultados - Critérios: {@criteria}, Resultados: {count}")]
    private static partial void LogLowAdvancedResults(ILogger logger, object criteria, int count);

    public AdvancedSearchQueryHandler(ISynQcoreDbContext context, ILogger<AdvancedSearchQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<SearchResultDto>> Handle(AdvancedSearchQuery request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        LogAdvancedSearchStarted(_logger, request.Query, request.Title, request.Author);

        var allResults = new List<SearchResultDto>();
        var contentTypes = request.Filters?.ContentTypes ?? new List<string> { "Post", "Document", "Employee", "Template" };

        // Processar termos de busca
        var queryTerms = ProcessSearchTerms(request.Query ?? "");
        var titleTerms = ProcessSearchTerms(request.Title ?? "");
        var contentTerms = ProcessSearchTerms(request.Content ?? "");
        var excludeTerms = request.ExcludeWords?.Select(w => w.ToLower(CultureInfo.InvariantCulture)).ToList() ?? new List<string>();

        // Buscar em posts
        if (contentTypes.Contains("Post") || contentTypes.Contains("All"))
        {
            var posts = await SearchPostsAdvancedAsync(request, queryTerms, titleTerms, contentTerms, excludeTerms, cancellationToken);
            allResults.AddRange(posts);
        }

        // Buscar em documentos
        if (contentTypes.Contains("Document") || contentTypes.Contains("All"))
        {
            var documents = await SearchDocumentsAdvancedAsync(request, queryTerms, titleTerms, contentTerms, excludeTerms, cancellationToken);
            allResults.AddRange(documents);
        }

        // Buscar em funcionários
        if (contentTypes.Contains("Employee") || contentTypes.Contains("All"))
        {
            var employees = await SearchEmployeesAdvancedAsync(request, queryTerms, titleTerms, contentTerms, excludeTerms, cancellationToken);
            allResults.AddRange(employees);
        }

        // Aplicar filtros globais
        var filteredResults = ApplyAdvancedFilters(allResults, request);

        // Calcular relevância
        foreach (var result in filteredResults)
        {
            result.RelevanceScore = CalculateAdvancedRelevanceScore(result, queryTerms, titleTerms, contentTerms, request);
            result.HighlightedTerms = FindHighlightedTerms(result, queryTerms.Concat(titleTerms).Concat(contentTerms).ToList());
        }

        // Ordenar por relevância
        var sortedResults = filteredResults
            .OrderByDescending(r => r.RelevanceScore)
            .ThenByDescending(r => r.UpdatedAt)
            .ToList();

        // Aplicar paginação
        var totalCount = sortedResults.Count;
        var skip = (request.Page - 1) * request.PageSize;
        var pagedResults = sortedResults.Skip(skip).Take(request.PageSize).ToList();

        var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
        LogAdvancedSearchCompleted(_logger, totalCount, (long)elapsed);

        if (totalCount < 3 && (!string.IsNullOrEmpty(request.Query) || !string.IsNullOrEmpty(request.Title)))
        {
            LogLowAdvancedResults(_logger, new { request.Query, request.Title, request.Author }, totalCount);
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

    private async Task<List<SearchResultDto>> SearchPostsAdvancedAsync(
        AdvancedSearchQuery request,
        List<string> queryTerms,
        List<string> titleTerms,
        List<string> contentTerms,
        List<string> excludeTerms,
        CancellationToken cancellationToken)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Department)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.Status == PostStatus.Published && !p.IsDeleted);

        // Aplicar critérios de busca avançada
        query = ApplyAdvancedSearchCriteria(query, request, queryTerms, titleTerms, contentTerms, excludeTerms);

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

    private async Task<List<SearchResultDto>> SearchDocumentsAdvancedAsync(
        AdvancedSearchQuery request,
        List<string> queryTerms,
        List<string> titleTerms,
        List<string> contentTerms,
        List<string> excludeTerms,
        CancellationToken cancellationToken)
    {
        var query = _context.CorporateDocuments
            .Where(d => d.Status == DocumentStatus.Approved && !d.IsDeleted);

        // Aplicar busca avançada em documentos
        if (titleTerms.Count > 0)
        {
            if (request.AllWords)
            {
                query = titleTerms.Aggregate(query, (current, term) => current.Where(d => d.Title.Contains(term)));
            }
            else if (request.AnyWords)
            {
                query = query.Where(d => titleTerms.Any(term => d.Title.Contains(term)));
            }
        }

        if (contentTerms.Count > 0 || queryTerms.Count > 0)
        {
            var allTerms = contentTerms.Concat(queryTerms).ToList();
            if (request.AllWords)
            {
                query = allTerms.Aggregate(query, (current, term) => current.Where(d => d.Description != null && d.Description.Contains(term)));
            }
            else
            {
                query = query.Where(d => d.Description != null && allTerms.Any(term => d.Description.Contains(term)));
            }
        }

        // Excluir termos
        if (excludeTerms.Count > 0)
        {
            query = excludeTerms.Aggregate(query, (current, term) =>
                current.Where(d => !d.Title.Contains(term) && (d.Description == null || !d.Description.Contains(term))));
        }

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
                ["ContentType"] = d.ContentType
            }
        }).ToList();
    }

    private async Task<List<SearchResultDto>> SearchEmployeesAdvancedAsync(
        AdvancedSearchQuery request,
        List<string> queryTerms,
        List<string> titleTerms,
        List<string> contentTerms,
        List<string> excludeTerms,
        CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
            .Where(e => e.IsActive && !e.IsDeleted);

        // Busca por autor específico
        if (!string.IsNullOrEmpty(request.Author))
        {
            query = query.Where(e =>
                e.FirstName.Contains(request.Author) ||
                e.LastName.Contains(request.Author) ||
                e.Email.Contains(request.Author));
        }

        // Aplicar termos de busca
        var allTerms = queryTerms.Concat(titleTerms).Concat(contentTerms).ToList();
        if (allTerms.Count > 0)
        {
            if (request.AllWords)
            {
                query = allTerms.Aggregate(query, (current, term) =>
                    current.Where(e => e.FirstName.Contains(term) || e.LastName.Contains(term) ||
                                  (e.Position != null && e.Position.Contains(term))));
            }
            else
            {
                query = query.Where(e => allTerms.Any(term =>
                    e.FirstName.Contains(term) || e.LastName.Contains(term) ||
                    (e.Position != null && e.Position.Contains(term))));
            }
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

    private static IQueryable<Post> ApplyAdvancedSearchCriteria(
        IQueryable<Post> query,
        AdvancedSearchQuery request,
        List<string> queryTerms,
        List<string> titleTerms,
        List<string> contentTerms,
        List<string> excludeTerms)
    {
        // Busca por autor específico
        if (!string.IsNullOrEmpty(request.Author))
        {
            query = query.Where(p =>
                p.Author.FirstName.Contains(request.Author) ||
                p.Author.LastName.Contains(request.Author) ||
                p.Author.Email.Contains(request.Author));
        }

        // Busca no título
        if (titleTerms.Count > 0)
        {
            if (request.ExactPhrase && !string.IsNullOrEmpty(request.Title))
            {
                query = query.Where(p => p.Title.Contains(request.Title));
            }
            else if (request.AllWords)
            {
                query = titleTerms.Aggregate(query, (current, term) => current.Where(p => p.Title.Contains(term)));
            }
            else if (request.AnyWords)
            {
                query = query.Where(p => titleTerms.Any(term => p.Title.Contains(term)));
            }
        }

        // Busca no conteúdo
        if (contentTerms.Count > 0 || queryTerms.Count > 0)
        {
            var allTerms = contentTerms.Concat(queryTerms).ToList();
            if (request.ExactPhrase && !string.IsNullOrEmpty(request.Content))
            {
                query = query.Where(p => p.Content.Contains(request.Content));
            }
            else if (request.AllWords)
            {
                query = allTerms.Aggregate(query, (current, term) => current.Where(p => p.Content.Contains(term)));
            }
            else
            {
                query = query.Where(p => allTerms.Any(term => p.Content.Contains(term)));
            }
        }

        // Excluir termos
        if (excludeTerms.Count > 0)
        {
            query = excludeTerms.Aggregate(query, (current, term) =>
                current.Where(p => !p.Title.Contains(term) && !p.Content.Contains(term)));
        }

        return query;
    }

    private static List<SearchResultDto> ApplyAdvancedFilters(List<SearchResultDto> results, AdvancedSearchQuery request)
    {
        if (request.Filters == null) return results;

        // Aplicar filtros padrão
        if (request.Filters.Tags?.Count > 0)
        {
            results = results.Where(r => r.Tags.Any(tag => request.Filters.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        if (request.Filters.Categories?.Count > 0)
        {
            results = results.Where(r => request.Filters.Categories.Contains(r.Category, StringComparer.OrdinalIgnoreCase)).ToList();
        }

        if (request.Filters.AuthorIds?.Count > 0)
        {
            results = results.Where(r => request.Filters.AuthorIds.Contains(r.AuthorId)).ToList();
        }

        if (request.Filters.MinEngagement.HasValue)
        {
            results = results.Where(r => (r.LikeCount + r.CommentCount + r.ViewCount) >= request.Filters.MinEngagement.Value).ToList();
        }

        return results;
    }

    private static float CalculateAdvancedRelevanceScore(
        SearchResultDto result,
        List<string> queryTerms,
        List<string> titleTerms,
        List<string> contentTerms,
        AdvancedSearchQuery request)
    {
        float score = 0.0f;
        var titleLower = result.Title.ToLower(CultureInfo.InvariantCulture);
        var contentLower = result.Content.ToLower(CultureInfo.InvariantCulture);

        // Score para termos no título (peso alto)
        foreach (var term in titleTerms)
        {
            if (titleLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 5.0f;
                if (titleLower.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                    score += 3.0f; // Boost adicional para match no início
            }
        }

        // Score para termos no conteúdo (peso médio)
        foreach (var term in contentTerms.Concat(queryTerms))
        {
            if (contentLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 2.0f;
            }
        }

        // Boost para busca exata
        if (request.ExactPhrase)
        {
            if (!string.IsNullOrEmpty(request.Title) && titleLower.Contains(request.Title.ToLower(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                score += 8.0f;
            if (!string.IsNullOrEmpty(request.Content) && contentLower.Contains(request.Content.ToLower(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                score += 6.0f;
        }

        // Boost para engajamento
        var engagementBoost = (result.LikeCount + result.CommentCount + result.ViewCount) * 0.01f;
        score += Math.Min(engagementBoost, 5.0f);

        return Math.Max(score, 0.1f);
    }

    private static List<string> ProcessSearchTerms(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<string>();

        var cleanQuery = NonWordCharactersRegex().Replace(query, " ");
        return cleanQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 2)
            .Select(t => t.ToLower(CultureInfo.InvariantCulture))
            .Distinct()
            .ToList();
    }

    private static List<string> FindHighlightedTerms(SearchResultDto result, List<string> searchTerms)
    {
        var highlighted = new List<string>();
        var titleLower = result.Title.ToLower(CultureInfo.InvariantCulture);
        var contentLower = result.Content.ToLower(CultureInfo.InvariantCulture);

        foreach (var term in searchTerms)
        {
            if (titleLower.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                contentLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                highlighted.Add(term);
            }
        }

        return highlighted.Distinct().ToList();
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
