using System.Globalization;
using System.Text.Json;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Implementação do serviço de estatísticas da plataforma
/// </summary>
public partial class PlatformStatsService : IPlatformStatsService
{
    private readonly IApiService _apiService;
    private readonly ILogger<PlatformStatsService> _logger;

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao buscar estatísticas da plataforma")]
    private static partial void LogErrorPlatformStats(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao buscar estatísticas de comunicação")]
    private static partial void LogErrorCommunicationStats(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao buscar estatísticas de conteúdo")]
    private static partial void LogErrorContentStats(ILogger logger, Exception ex);

    public PlatformStatsService(IApiService apiService, ILogger<PlatformStatsService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém estatísticas gerais da plataforma
    /// </summary>
    public async Task<PlatformStatsDto> GetPlatformStatsAsync()
    {
        try
        {
            // Buscar diferentes fontes de dados em paralelo
            var contentStatsTask = GetContentStatsAsync();
            var communicationStatsTask = GetCommunicationStatsAsync();

            await Task.WhenAll(contentStatsTask, communicationStatsTask);

            var contentStats = await contentStatsTask;
            var commStats = await communicationStatsTask;

            // Combinar dados para estatísticas da plataforma
            return new PlatformStatsDto
            {
                TotalEmployees = contentStats.TotalEmployees,
                ActiveUsersToday = contentStats.ActiveUsersToday,
                TotalPosts = contentStats.TotalPosts,
                TotalComments = contentStats.TotalComments,
                TotalDocuments = contentStats.TotalDocuments,
                OnlineUsers = commStats.ActiveTeams * 10, // Estimativa
                EngagementRate = CalculateEngagementRate(contentStats),
                LastUpdated = DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture)
            };
        }
        catch (Exception ex)
        {
            LogErrorPlatformStats(_logger, ex);
            return GetFallbackStats();
        }
    }

    /// <summary>
    /// Obtém estatísticas de comunicação
    /// </summary>
    public async Task<CommunicationStatsDto> GetCommunicationStatsAsync()
    {
        try
        {
            var response = await _apiService.GetAsync<dynamic>("corporatecommunication/statistics");

            if (response != null)
            {
                var jsonString = response.ToString();
                var jsonData = JsonSerializer.Deserialize<JsonElement>(jsonString!);

                return new CommunicationStatsDto
                {
                    TotalMessages = GetJsonValue(jsonData, "today.totalMessages", 1247),
                    CompanyAnnouncements = GetJsonValue(jsonData, "today.companyAnnouncements", 3),
                    TeamMessages = GetJsonValue(jsonData, "today.teamMessages", 892),
                    ActiveTeams = GetJsonValue(jsonData, "thisWeek.activeTeams", 23),
                    SatisfactionRate = 95.0 // Valor fixo por enquanto
                };
            }
        }
        catch (Exception ex)
        {
            LogErrorCommunicationStats(_logger, ex);
        }

        return new CommunicationStatsDto
        {
            TotalMessages = 1247,
            CompanyAnnouncements = 3,
            TeamMessages = 892,
            ActiveTeams = 23,
            SatisfactionRate = 95.0
        };
    }

    /// <summary>
    /// Obtém estatísticas de conteúdo
    /// </summary>
    public async Task<ContentStatsDto> GetContentStatsAsync()
    {
        try
        {
            var response = await _apiService.GetAsync<ContentStatsDto>("corporatesearch/stats");
            return response ?? GetFallbackContentStats();
        }
        catch (Exception ex)
        {
            LogErrorContentStats(_logger, ex);
            return GetFallbackContentStats();
        }
    }

    /// <summary>
    /// Calcula taxa de engajamento baseada nas estatísticas
    /// </summary>
    private static double CalculateEngagementRate(ContentStatsDto stats)
    {
        if (stats.TotalEmployees == 0) return 0;

        var engagement = (double)stats.ActiveUsersToday / stats.TotalEmployees * 100;
        return Math.Round(engagement, 1);
    }

    /// <summary>
    /// Extrai valor do JSON com fallback
    /// </summary>
    private static int GetJsonValue(JsonElement json, string path, int fallback)
    {
        try
        {
            var parts = path.Split('.');
            var current = json;

            foreach (var part in parts)
            {
                if (current.TryGetProperty(part, out var next))
                {
                    current = next;
                }
                else
                {
                    return fallback;
                }
            }

            return current.TryGetInt32(out var value) ? value : fallback;
        }
        catch
        {
            return fallback;
        }
    }

    /// <summary>
    /// Estatísticas de fallback para casos de erro
    /// </summary>
    private static PlatformStatsDto GetFallbackStats()
    {
        return new PlatformStatsDto
        {
            TotalEmployees = 500,
            ActiveUsersToday = 87,
            TotalPosts = 1247,
            TotalComments = 3892,
            TotalDocuments = 156,
            OnlineUsers = 42,
            EngagementRate = 85.5,
            LastUpdated = DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture)
        };
    }

    /// <summary>
    /// Estatísticas de conteúdo de fallback
    /// </summary>
    private static ContentStatsDto GetFallbackContentStats()
    {
        return new ContentStatsDto
        {
            TotalPosts = 1247,
            TotalDocuments = 156,
            TotalMediaAssets = 89,
            TotalComments = 3892,
            TotalEmployees = 500,
            ActiveUsersToday = 87,
            ActiveUsersThisWeek = 234,
            ActiveUsersThisMonth = 445
        };
    }
}
