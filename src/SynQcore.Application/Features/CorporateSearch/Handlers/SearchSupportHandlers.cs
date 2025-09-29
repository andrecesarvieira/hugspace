using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Application.Features.CorporateSearch.Queries;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;
using System.Globalization;

namespace SynQcore.Application.Features.CorporateSearch.Handlers;

public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQuery, List<SearchSuggestionDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetSearchSuggestionsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, string, int, Exception?> LogBuscandoSugestoes =
        LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(5511, nameof(LogBuscandoSugestoes)),
            "Buscando sugestões para termo: {Partial}, Máximo: {MaxSuggestions}");

    private static readonly Action<ILogger, int, Exception?> LogSugestoesEncontradas =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5512, nameof(LogSugestoesEncontradas)),
            "Sugestões encontradas: {Count} resultados");

    private static readonly Action<ILogger, Exception?> LogErroSugestoes =
        LoggerMessage.Define(LogLevel.Error, new EventId(5519, nameof(LogErroSugestoes)),
            "Erro ao buscar sugestões de busca");

    public GetSearchSuggestionsQueryHandler(ISynQcoreDbContext context, ILogger<GetSearchSuggestionsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<SearchSuggestionDto>> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoSugestoes(_logger, request.Partial, request.MaxSuggestions, null);

        try
        {
            var suggestions = new List<SearchSuggestionDto>();
            var partial = request.Partial.ToLower(CultureInfo.InvariantCulture);

            // Sugestões de títulos de Posts
            var postSuggestions = await _context.Posts
                .Where(p => !p.IsDeleted && p.Status == PostStatus.Published)
                .Where(p => p.Title.Contains(partial))
                .Select(p => new { Term = p.Title, Category = "Posts" })
                .Distinct()
                .Take(request.MaxSuggestions / 4)
                .ToListAsync(cancellationToken);

            suggestions.AddRange(postSuggestions.Select(s => new SearchSuggestionDto
            {
                Term = s.Term,
                Category = s.Category,
                Frequency = 1,
                Score = CalculateSuggestionScore(s.Term, partial)
            }));

            // Sugestões de documentos corporativos
            var docSuggestions = await _context.CorporateDocuments
                .Where(d => !d.IsDeleted)
                .Where(d => d.Title.Contains(partial))
                .Select(d => new { Term = d.Title, Category = "Documentos" })
                .Distinct()
                .Take(request.MaxSuggestions / 4)
                .ToListAsync(cancellationToken);

            suggestions.AddRange(docSuggestions.Select(s => new SearchSuggestionDto
            {
                Term = s.Term,
                Category = s.Category,
                Frequency = 1,
                Score = CalculateSuggestionScore(s.Term, partial)
            }));

            // Sugestões de funcionários
            var employeeSuggestions = await _context.Employees
                .Where(e => e.IsActive && !e.IsDeleted)
                .Where(e => e.FirstName.Contains(partial) || e.LastName.Contains(partial))
                .Select(e => new { Term = e.FullName, Category = "Pessoas" })
                .Distinct()
                .Take(request.MaxSuggestions / 4)
                .ToListAsync(cancellationToken);

            suggestions.AddRange(employeeSuggestions.Select(s => new SearchSuggestionDto
            {
                Term = s.Term,
                Category = s.Category,
                Frequency = 1,
                Score = CalculateSuggestionScore(s.Term, partial)
            }));

            // Sugestões de departamentos
            var deptSuggestions = await _context.Departments
                .Where(d => d.IsActive)
                .Where(d => d.Name.Contains(partial))
                .Select(d => new { Term = d.Name, Category = "Departamentos" })
                .Distinct()
                .Take(request.MaxSuggestions / 4)
                .ToListAsync(cancellationToken);

            suggestions.AddRange(deptSuggestions.Select(s => new SearchSuggestionDto
            {
                Term = s.Term,
                Category = s.Category,
                Frequency = 1,
                Score = CalculateSuggestionScore(s.Term, partial)
            }));

            // Ordenar por score e limitar resultado
            var finalSuggestions = suggestions
                .OrderByDescending(s => s.Score)
                .Take(request.MaxSuggestions)
                .ToList();

            LogSugestoesEncontradas(_logger, finalSuggestions.Count, null);

            return finalSuggestions;
        }
        catch (Exception ex)
        {
            LogErroSugestoes(_logger, ex);
            throw;
        }
    }

    private static float CalculateSuggestionScore(string term, string partial)
    {
        var termLower = term.ToLower(CultureInfo.InvariantCulture);
        var score = 0f;

        // Score base por match
        if (termLower.Contains(partial))
            score += 1f;

        // Boost se começa com o termo
        if (termLower.StartsWith(partial, StringComparison.InvariantCultureIgnoreCase))
            score += 2f;

        // Boost por tamanho (termos menores são melhores)
        if (term.Length <= 50)
            score += 1f;

        return score;
    }
}

public class GetTrendingTopicsQueryHandler : IRequestHandler<GetTrendingTopicsQuery, List<TrendingTopicDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetTrendingTopicsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, string, int, Exception?> LogBuscandoTrending =
        LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(5521, nameof(LogBuscandoTrending)),
            "Buscando trending topics - Período: {Period}, Máximo: {MaxTopics}");

    private static readonly Action<ILogger, int, Exception?> LogTrendingEncontrados =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5522, nameof(LogTrendingEncontrados)),
            "Trending topics encontrados: {Count} resultados");

    private static readonly Action<ILogger, Exception?> LogErroTrending =
        LoggerMessage.Define(LogLevel.Error, new EventId(5529, nameof(LogErroTrending)),
            "Erro ao buscar trending topics");

    public GetTrendingTopicsQueryHandler(ISynQcoreDbContext context, ILogger<GetTrendingTopicsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TrendingTopicDto>> Handle(GetTrendingTopicsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoTrending(_logger, request.Period, request.MaxTopics, null);

        try
        {
            // Calcular período de análise
            var endDate = DateTime.UtcNow;
            var startDate = request.Period.ToLower(CultureInfo.InvariantCulture) switch
            {
                "day" => endDate.AddDays(-1),
                "week" => endDate.AddDays(-7),
                "month" => endDate.AddMonths(-1),
                "quarter" => endDate.AddMonths(-3),
                _ => endDate.AddDays(-7)
            };

            var trendingTopics = new List<TrendingTopicDto>();

            // Analisar posts mais comentados/curtidos
            var popularPosts = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.Status == PostStatus.Published)
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderByDescending(p => p.LikeCount + p.CommentCount + p.ViewCount)
                .Take(request.MaxTopics)
                .ToListAsync(cancellationToken);

            foreach (var post in popularPosts)
            {
                var trendScore = CalculateTrendScore(post.LikeCount, post.CommentCount, post.ViewCount, post.CreatedAt, startDate, endDate);

                if (trendScore >= request.MinTrendScore)
                {
                    trendingTopics.Add(new TrendingTopicDto
                    {
                        Topic = post.Title,
                        Category = post.Category?.Name ?? "Geral",
                        MentionCount = 1,
                        EngagementCount = post.LikeCount + post.CommentCount,
                        TrendScore = trendScore,
                        PeriodStart = startDate,
                        PeriodEnd = endDate,
                        TopContributors = new List<Guid> { post.AuthorId }
                    });
                }
            }

            // Analisar categorias mais ativas
            var categoryStats = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.Status == PostStatus.Published)
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    PostCount = g.Count(),
                    TotalEngagement = g.Sum(p => p.LikeCount + p.CommentCount + p.ViewCount)
                })
                .OrderByDescending(x => x.TotalEngagement)
                .Take(5)
                .ToListAsync(cancellationToken);

            foreach (var category in categoryStats)
            {
                var trendScore = CalculateCategoryTrendScore(category.PostCount, category.TotalEngagement, startDate, endDate);

                if (trendScore >= request.MinTrendScore)
                {
                    trendingTopics.Add(new TrendingTopicDto
                    {
                        Topic = $"Categoria: {category.CategoryName}",
                        Category = "Categorias",
                        MentionCount = category.PostCount,
                        EngagementCount = category.TotalEngagement,
                        TrendScore = trendScore,
                        PeriodStart = startDate,
                        PeriodEnd = endDate
                    });
                }
            }

            // Ordenar por trend score e limitar resultado
            var finalTopics = trendingTopics
                .OrderByDescending(t => t.TrendScore)
                .Take(request.MaxTopics)
                .ToList();

            LogTrendingEncontrados(_logger, finalTopics.Count, null);

            return finalTopics;
        }
        catch (Exception ex)
        {
            LogErroTrending(_logger, ex);
            throw;
        }
    }

    private static float CalculateTrendScore(int likes, int comments, int views, DateTime createdAt, DateTime startDate, DateTime endDate)
    {
        var engagementScore = likes * 3 + comments * 2 + views * 0.1f;
        var timeSpan = (endDate - startDate).TotalDays;
        var ageScore = (float)(1.0 - (DateTime.UtcNow - createdAt).TotalDays / timeSpan);

        return (engagementScore / 100f) * Math.Max(ageScore, 0.1f);
    }

    private static float CalculateCategoryTrendScore(int postCount, int totalEngagement, DateTime startDate, DateTime endDate)
    {
        var timeSpan = (endDate - startDate).TotalDays;
        var averageEngagement = totalEngagement / Math.Max(postCount, 1);
        var frequencyScore = postCount / (float)timeSpan;

        return (averageEngagement / 10f) + (frequencyScore * 2f);
    }
}

public class GetContentStatsQueryHandler : IRequestHandler<GetContentStatsQuery, ContentStatsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetContentStatsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, Exception?> LogBuscandoEstatisticas =
        LoggerMessage.Define(LogLevel.Information, new EventId(5531, nameof(LogBuscandoEstatisticas)),
            "Buscando estatísticas de conteúdo");

    private static readonly Action<ILogger, int, int, int, Exception?> LogEstatisticasCalculadas =
        LoggerMessage.Define<int, int, int>(LogLevel.Information, new EventId(5532, nameof(LogEstatisticasCalculadas)),
            "Estatísticas calculadas - Posts: {PostCount}, Documentos: {DocumentCount}, Funcionários: {EmployeeCount}");

    private static readonly Action<ILogger, Exception?> LogErroEstatisticas =
        LoggerMessage.Define(LogLevel.Error, new EventId(5539, nameof(LogErroEstatisticas)),
            "Erro ao calcular estatísticas de conteúdo");

    public GetContentStatsQueryHandler(ISynQcoreDbContext context, ILogger<GetContentStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ContentStatsDto> Handle(GetContentStatsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoEstatisticas(_logger, null);

        try
        {
            var now = DateTime.UtcNow;
            var startDate = request.StartDate ?? now.AddMonths(-1);
            var endDate = request.EndDate ?? now;

            // Contar posts
            var totalPosts = await _context.Posts
                .CountAsync(p => !p.IsDeleted &&
                    p.CreatedAt >= startDate && p.CreatedAt <= endDate, cancellationToken);

            // Contar documentos
            var totalDocuments = await _context.CorporateDocuments
                .CountAsync(d => !d.IsDeleted &&
                    d.CreatedAt >= startDate && d.CreatedAt <= endDate, cancellationToken);

            // Contar assets de mídia
            var totalMediaAssets = await _context.MediaAssets
                .CountAsync(m => !m.IsDeleted &&
                    m.CreatedAt >= startDate && m.CreatedAt <= endDate, cancellationToken);

            // Contar comentários
            var totalComments = await _context.Comments
                .CountAsync(c => !c.IsDeleted &&
                    c.CreatedAt >= startDate && c.CreatedAt <= endDate, cancellationToken);

            // Contar funcionários
            var totalEmployees = await _context.Employees
                .CountAsync(e => e.IsActive && !e.IsDeleted, cancellationToken);

            // Usuários ativos
            var activeUsersToday = await _context.Posts
                .Where(p => p.CreatedAt >= now.AddDays(-1))
                .Select(p => p.AuthorId)
                .Distinct()
                .CountAsync(cancellationToken);

            var activeUsersThisWeek = await _context.Posts
                .Where(p => p.CreatedAt >= now.AddDays(-7))
                .Select(p => p.AuthorId)
                .Distinct()
                .CountAsync(cancellationToken);

            var activeUsersThisMonth = await _context.Posts
                .Where(p => p.CreatedAt >= now.AddDays(-30))
                .Select(p => p.AuthorId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Distribuição por categoria
            var categoryDistribution = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count, cancellationToken);

            // Atividade por departamento
            var departmentActivity = await _context.Posts
                .Include(p => p.Department)
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .Where(p => p.Department != null)
                .GroupBy(p => p.Department!.Name)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Department, x => x.Count, cancellationToken);

            var stats = new ContentStatsDto
            {
                TotalPosts = totalPosts,
                TotalDocuments = totalDocuments,
                TotalMediaAssets = totalMediaAssets,
                TotalComments = totalComments,
                TotalEmployees = totalEmployees,
                ActiveUsersToday = activeUsersToday,
                ActiveUsersThisWeek = activeUsersThisWeek,
                ActiveUsersThisMonth = activeUsersThisMonth,
                ContentTypeDistribution = new Dictionary<string, int>
                {
                    ["Posts"] = totalPosts,
                    ["Documentos"] = totalDocuments,
                    ["Assets de Mídia"] = totalMediaAssets,
                    ["Comentários"] = totalComments
                },
                CategoryDistribution = categoryDistribution,
                DepartmentActivity = departmentActivity,
                TopSearchTerms = new List<string>(), // Implementar com dados reais se necessário
                TrendingTopics = new List<TrendingTopicDto>() // Implementar com dados reais se necessário
            };

            LogEstatisticasCalculadas(_logger, totalPosts, totalDocuments, totalEmployees, null);

            return stats;
        }
        catch (Exception ex)
        {
            LogErroEstatisticas(_logger, ex);
            throw;
        }
    }
}
