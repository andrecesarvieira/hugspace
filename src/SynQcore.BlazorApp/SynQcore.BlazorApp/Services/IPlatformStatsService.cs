using SynQcore.BlazorApp.Services;
using System.Text.Json;

namespace SynQcore.BlazorApp.Services;

/// <summary>
/// Serviço para buscar estatísticas da plataforma
/// </summary>
public interface IPlatformStatsService
{
    /// <summary>
    /// Obtém estatísticas gerais da plataforma
    /// </summary>
    Task<PlatformStatsDto> GetPlatformStatsAsync();

    /// <summary>
    /// Obtém estatísticas de comunicação
    /// </summary>
    Task<CommunicationStatsDto> GetCommunicationStatsAsync();

    /// <summary>
    /// Obtém estatísticas de conteúdo
    /// </summary>
    Task<ContentStatsDto> GetContentStatsAsync();
}

/// <summary>
/// DTO para estatísticas gerais da plataforma
/// </summary>
public class PlatformStatsDto
{
    public int TotalEmployees { get; set; }
    public int ActiveUsersToday { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalDocuments { get; set; }
    public int OnlineUsers { get; set; }
    public double EngagementRate { get; set; }
    public string LastUpdated { get; set; } = string.Empty;
}

/// <summary>
/// DTO para estatísticas de comunicação
/// </summary>
public class CommunicationStatsDto
{
    public int TotalMessages { get; set; }
    public int CompanyAnnouncements { get; set; }
    public int TeamMessages { get; set; }
    public int ActiveTeams { get; set; }
    public double SatisfactionRate { get; set; }
}

/// <summary>
/// DTO para estatísticas de conteúdo
/// </summary>
public class ContentStatsDto
{
    public int TotalPosts { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalMediaAssets { get; set; }
    public int TotalComments { get; set; }
    public int TotalEmployees { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public Dictionary<string, int> ContentTypeDistribution { get; set; } = new();
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    public Dictionary<string, int> DepartmentActivity { get; set; } = new();
}
