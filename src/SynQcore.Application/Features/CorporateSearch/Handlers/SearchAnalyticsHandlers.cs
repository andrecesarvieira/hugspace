using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Application.Features.CorporateSearch.Queries;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.CorporateSearch.Handlers;

public class GetSearchAnalyticsQueryHandler : IRequestHandler<GetSearchAnalyticsQuery, SearchAnalyticsDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetSearchAnalyticsQueryHandler> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, DateTime, DateTime, Exception?> LogBuscandoAnalytics =
        LoggerMessage.Define<DateTime, DateTime>(LogLevel.Information, new EventId(5551, nameof(LogBuscandoAnalytics)),
            "Buscando analytics de busca - Período: {StartDate:yyyy-MM-dd} a {EndDate:yyyy-MM-dd}");

    private static readonly Action<ILogger, int, int, Exception?> LogAnalyticsCalculados =
        LoggerMessage.Define<int, int>(LogLevel.Information, new EventId(5552, nameof(LogAnalyticsCalculados)),
            "Analytics calculados - Total searches: {TotalSearches}, Unique users: {UniqueUsers}");

    private static readonly Action<ILogger, Exception?> LogErroAnalytics =
        LoggerMessage.Define(LogLevel.Error, new EventId(5559, nameof(LogErroAnalytics)),
            "Erro ao calcular analytics de busca");

    public GetSearchAnalyticsQueryHandler(ISynQcoreDbContext context, ILogger<GetSearchAnalyticsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SearchAnalyticsDto> Handle(GetSearchAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        LogBuscandoAnalytics(_logger, startDate, endDate, null);

        try
        {
            // Como não temos uma tabela específica de eventos de busca ainda,
            // vamos simular analytics baseados em atividade de posts e visualizações

            // Total de "buscas" baseado em visualizações de posts
            var totalSearches = await _context.Posts
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .SumAsync(p => p.ViewCount, cancellationToken);

            // Usuários únicos baseado em autores de posts
            var uniqueUsers = await _context.Posts
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .Select(p => p.AuthorId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Buscar posts populares como "resultados mais clicados"
            var popularResults = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.Status == PostStatus.Published)
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderByDescending(p => p.ViewCount + p.LikeCount)
                .Take(10)
                .Select(p => new PopularResultDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Type = "Post",
                    ClickCount = p.ViewCount,
                    Category = p.Category != null ? p.Category.Name : "Geral"
                })
                .ToListAsync(cancellationToken);

            // Simular termos de busca populares baseado em títulos de posts
            var searchTerms = await _context.Posts
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .Select(p => p.Title)
                .ToListAsync(cancellationToken);

            var popularTerms = ExtractPopularTerms(searchTerms)
                .Take(20)
                .ToList();

            // Categorias mais buscadas baseado em posts visualizados
            var popularCategories = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .Where(p => p.Category != null)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new PopularCategoryDto
                {
                    Category = g.Key,
                    SearchCount = g.Sum(p => p.ViewCount),
                    ResultCount = g.Count()
                })
                .OrderByDescending(c => c.SearchCount)
                .Take(10)
                .ToListAsync(cancellationToken);

            // Calcular métricas de tempo
            var avgResponseTime = CalculateAverageResponseTime();
            var successRate = CalculateSuccessRate(totalSearches);

            // Distribuição temporal (simulada)
            var timeDistribution = await GenerateTimeDistribution(startDate, endDate, cancellationToken);

            var analytics = new SearchAnalyticsDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalSearches = totalSearches,
                UniqueUsers = uniqueUsers,
                AverageResponseTime = avgResponseTime,
                SuccessRate = successRate,
                PopularSearchTerms = popularTerms,
                PopularResults = popularResults,
                PopularCategories = popularCategories,
                SearchTimeDistribution = timeDistribution,
                ZeroResultsQueries = GenerateZeroResultsQueries(), // Simulado
                QueryRefinementRate = CalculateQueryRefinementRate()
            };

            LogAnalyticsCalculados(_logger, totalSearches, uniqueUsers, null);

            return analytics;
        }
        catch (Exception ex)
        {
            LogErroAnalytics(_logger, ex);
            throw;
        }
    }

    private static List<PopularSearchTermDto> ExtractPopularTerms(List<string> titles)
    {
        var termCounts = new Dictionary<string, int>();

        foreach (var title in titles)
        {
            var words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length >= 3) // Apenas palavras com 3+ caracteres
                .Select(w => w.ToLowerInvariant().Trim('.', ',', '!', '?', ';', ':'))
                .Where(w => !IsStopWord(w));

            foreach (var word in words)
            {
                termCounts[word] = termCounts.GetValueOrDefault(word, 0) + 1;
            }
        }

        return termCounts
            .Where(kvp => kvp.Value > 1) // Apenas termos que aparecem mais de uma vez
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => new PopularSearchTermDto
            {
                Term = kvp.Key,
                Count = kvp.Value,
                Trend = "stable" // Poderia ser calculado comparando períodos
            })
            .ToList();
    }

    private static bool IsStopWord(string word)
    {
        var stopWords = new HashSet<string>
        {
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "will", "with", "da", "de", "do", "em", "para", "por",
            "com", "uma", "um", "que", "ser", "ter", "este", "esta", "isso"
        };

        return stopWords.Contains(word);
    }

    private static double CalculateAverageResponseTime()
    {
        // Simular tempo de resposta médio (em ms)
        var random = new Random();
        return 50 + random.NextDouble() * 200; // Entre 50-250ms
    }

    private static double CalculateSuccessRate(int totalSearches)
    {
        // Simular taxa de sucesso baseada em alguma lógica
        return totalSearches > 0 ? 0.85 + (new Random().NextDouble() * 0.1) : 0.0;
    }

    private static double CalculateQueryRefinementRate()
    {
        // Simular taxa de refinamento de queries
        return 0.15 + (new Random().NextDouble() * 0.1); // Entre 15-25%
    }

    private async Task<Dictionary<string, int>> GenerateTimeDistribution(
        DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        // Gerar distribuição baseada em atividade de posts por hora
        var hourlyActivity = await _context.Posts
            .Where(p => !p.IsDeleted && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
            .GroupBy(p => p.CreatedAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => $"{x.Hour:D2}:00", x => x.Count, cancellationToken);

        return hourlyActivity;
    }

    private static List<string> GenerateZeroResultsQueries()
    {
        // Simular queries que não retornaram resultados
        return new List<string>
        {
            "termo muito específico",
            "xyz123",
            "produto inexistente",
            "departamento antigo"
        };
    }
}

public class GetSearchConfigQueryHandler : IRequestHandler<GetSearchConfigQuery, SearchConfigDto>
{
    private readonly ILogger<GetSearchConfigQueryHandler> _logger;

    private static readonly Action<ILogger, Exception?> LogBuscandoConfiguracao =
        LoggerMessage.Define(LogLevel.Information, new EventId(5561, nameof(LogBuscandoConfiguracao)),
            "Buscando configuração de busca");

    public GetSearchConfigQueryHandler(ILogger<GetSearchConfigQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<SearchConfigDto> Handle(GetSearchConfigQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoConfiguracao(_logger, null);

        var config = new SearchConfigDto
        {
            EnabledContentTypes = new List<string>
            {
                "Posts",
                "Documentos",
                "Pessoas",
                "Departamentos",
                "MediaAssets"
            },
            EnabledFilters = new List<string>
            {
                "ContentType",
                "Category",
                "Department",
                "Author",
                "DateRange",
                "Tags"
            },
            DefaultSortBy = "Relevance",
            AvailableSortOptions = new List<string>
            {
                "Relevance",
                "Date",
                "Views",
                "Likes",
                "Comments",
                "Title"
            },
            MaxResultsPerType = 50,
            SearchTimeout = 5000,
            EnableAutoComplete = true,
            EnableSuggestions = true,
            EnableAnalytics = true,
            MinQueryLength = 2,
            MaxQueryLength = 500,
            EnableFuzzySearch = true,
            FuzzySearchThreshold = 0.7f,
            EnableHighlighting = true,
            HighlightTags = new List<string> { "<mark>", "</mark>" },
            CacheTimeout = 300 // 5 minutos
        };

        return Task.FromResult(config);
    }
}
