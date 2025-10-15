using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.CorporateSearch.Commands;

public class RecordSearchEventCommand : IRequest<bool>
{
    public string SearchTerm { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserDepartment { get; set; } = string.Empty;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
    public int ResultCount { get; set; }
    public float AverageRelevanceScore { get; set; }
    public TimeSpan SearchDuration { get; set; }
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;

    public RecordSearchEventCommand(string searchTerm, Guid userId, int resultCount)
    {
        SearchTerm = searchTerm;
        UserId = userId;
        ResultCount = resultCount;
    }
}

public class RecordSearchClickCommand : IRequest<bool>
{
    public string SearchTerm { get; set; } = string.Empty;
    public Guid ResultId { get; set; }
    public string ResultType { get; set; } = string.Empty;
    public string ResultTitle { get; set; } = string.Empty;
    public int Position { get; set; }
    public float RelevanceScore { get; set; }
    public Guid UserId { get; set; }
    public string UserDepartment { get; set; } = string.Empty;
    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

    public RecordSearchClickCommand(string searchTerm, Guid resultId, string resultType, int position, Guid userId)
    {
        SearchTerm = searchTerm;
        ResultId = resultId;
        ResultType = resultType;
        Position = position;
        UserId = userId;
    }
}

public class CreateSearchIndexCommand : IRequest<bool>
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DocumentAccessLevel AccessLevel { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public CreateSearchIndexCommand(Guid contentId, string contentType, string title, string content)
    {
        ContentId = contentId;
        ContentType = contentType;
        Title = title;
        Content = content;
    }
}

public class UpdateSearchIndexCommand : IRequest<bool>
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public List<string>? Tags { get; set; }
    public DocumentAccessLevel? AccessLevel { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UpdateSearchIndexCommand(Guid contentId, string contentType)
    {
        ContentId = contentId;
        ContentType = contentType;
    }
}

public class DeleteFromSearchIndexCommand : IRequest<bool>
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;

    public DeleteFromSearchIndexCommand(Guid contentId, string contentType)
    {
        ContentId = contentId;
        ContentType = contentType;
    }
}

public class RebuildSearchIndexCommand : IRequest<bool>
{
    public List<string>? ContentTypes { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public bool IncludeDeleted { get; set; }
    public int BatchSize { get; set; } = 1000;
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public RebuildSearchIndexCommand(Guid requestedBy)
    {
        RequestedBy = requestedBy;
    }
}

public class CleanupSearchAnalyticsCommand : IRequest<int>
{
    public DateTime OlderThan { get; set; }
    public bool KeepAggregated { get; set; } = true;
    public List<string>? SearchTermsToKeep { get; set; }
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public CleanupSearchAnalyticsCommand(DateTime olderThan, Guid requestedBy)
    {
        OlderThan = olderThan;
        RequestedBy = requestedBy;
    }
}

public class ConfigureTrendingAlertsCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public List<string> TopicsToWatch { get; set; } = new();
    public List<string> CategoriesToWatch { get; set; } = new();
    public List<Guid> DepartmentsToWatch { get; set; } = new();
    public float MinTrendScore { get; set; } = 0.7f;
    public string AlertFrequency { get; set; } = "daily"; // hourly, daily, weekly
    public bool EmailAlerts { get; set; } = true;
    public bool InAppAlerts { get; set; } = true;
    public DateTime ConfiguredAt { get; set; } = DateTime.UtcNow;

    public ConfigureTrendingAlertsCommand(Guid userId)
    {
        UserId = userId;
    }
}

public class SaveFavoriteSearchCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public SearchFiltersDto? Filters { get; set; }
    public SearchConfigDto? Config { get; set; }
    public bool EnableNotifications { get; set; }
    public string NotificationFrequency { get; set; } = "weekly";
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    public SaveFavoriteSearchCommand(Guid userId, string name, string query)
    {
        UserId = userId;
        Name = name;
        Query = query;
    }
}

public class ExecuteSavedSearchCommand : IRequest<PagedResult<SearchResultDto>>
{
    public Guid SavedSearchId { get; set; }
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool UpdateLastExecuted { get; set; } = true;

    public ExecuteSavedSearchCommand(Guid savedSearchId, Guid userId)
    {
        SavedSearchId = savedSearchId;
        UserId = userId;
    }
}

public class OptimizeSearchPerformanceCommand : IRequest<bool>
{
    public List<string>? ContentTypes { get; set; }
    public bool RebuildIndexes { get; set; } = true;
    public bool UpdateStatistics { get; set; } = true;
    public bool CleanupOldData { get; set; } = true;
    public int DaysToKeepDetails { get; set; } = 90;
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public OptimizeSearchPerformanceCommand(Guid requestedBy)
    {
        RequestedBy = requestedBy;
    }
}
