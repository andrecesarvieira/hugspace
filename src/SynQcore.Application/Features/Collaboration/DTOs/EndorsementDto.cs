using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.DTOs;

public class EndorsementDto
{
    public Guid Id { get; set; }

    public EndorsementType Type { get; set; }

    public string TypeDisplayName { get; set; } = string.Empty;

    public string TypeIcon { get; set; } = string.Empty;

    public string? Note { get; set; }

    public bool IsPublic { get; set; }

    public DateTime EndorsedAt { get; set; }

    public string? Context { get; set; }

    public Guid EndorserId { get; set; }

    public string EndorserName { get; set; } = string.Empty;

    public string EndorserEmail { get; set; } = string.Empty;

    public string? EndorserDepartment { get; set; }

    public string? EndorserPosition { get; set; }

    public Guid? PostId { get; set; }

    public string? PostTitle { get; set; }

    public Guid? CommentId { get; set; }

    public string? CommentContent { get; set; }
}

public class CreateEndorsementDto
{
    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    public EndorsementType Type { get; set; }

    public string? Note { get; set; }

    public bool IsPublic { get; set; }

    public string? Context { get; set; }
}

public class UpdateEndorsementDto
{
    public EndorsementType? Type { get; set; }

    public string? Note { get; set; }

    public bool? IsPublic { get; set; }

    public string? Context { get; set; }
}

public class EndorsementSearchDto
{
    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    public Guid? EndorserId { get; set; }

    public EndorsementType? Type { get; set; }

    public string? Context { get; set; }

    public bool? IsPublic { get; set; }

    public DateTime? EndorsedAfter { get; set; }

    public DateTime? EndorsedBefore { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}

public class EndorsementStatsDto
{
    public Guid ContentId { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public int TotalEndorsements { get; set; }

    public int HelpfulCount { get; set; }

    public int InsightfulCount { get; set; }

    public int AccurateCount { get; set; }

    public int InnovativeCount { get; set; }

    public int ComprehensiveCount { get; set; }

    public int WellResearchedCount { get; set; }

    public int ActionableCount { get; set; }

    public int StrategicCount { get; set; }

    public EndorsementType? TopEndorsementType { get; set; }

    public string? TopEndorsementTypeIcon { get; set; }
}

public class EmployeeEndorsementRankingDto
{
    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeEmail { get; set; } = string.Empty;

    public string? Department { get; set; }

    public string? Position { get; set; }

    public int TotalEndorsementsReceived { get; set; }

    public int TotalEndorsementsGiven { get; set; }

    public int HelpfulReceived { get; set; }

    public int InsightfulReceived { get; set; }

    public int AccurateReceived { get; set; }

    public int InnovativeReceived { get; set; }

    public double EngagementScore { get; set; }

    public int Ranking { get; set; }
}

public class EndorsementAnalyticsDto
{
    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public string? DepartmentFilter { get; set; }

    public int TotalEndorsements { get; set; }

    public int TotalParticipants { get; set; }

    public int TotalContentEndorsed { get; set; }

    public Dictionary<EndorsementType, int> EndorsementsByType { get; set; } = [];

    public Dictionary<string, int> EndorsementsByDepartment { get; set; } = [];

    public List<EmployeeEndorsementRankingDto> TopEndorsersGiven { get; set; } = [];

    public List<EmployeeEndorsementRankingDto> TopEndorsersReceived { get; set; } = [];

    public double GrowthRate { get; set; }

    public EndorsementType MostPopularType { get; set; }

    public string MostActiveDay { get; set; } = string.Empty;

    public double AverageEndorsementsPerEmployee { get; set; }

    public double EndorsementRate { get; set; }
}
