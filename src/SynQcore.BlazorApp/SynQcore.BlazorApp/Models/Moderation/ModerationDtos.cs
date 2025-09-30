namespace SynQcore.BlazorApp.Models.Moderation;

/// <summary>
/// DTO para fila de moderação
/// </summary>
public class ModerationQueueDto
{
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<ModerationItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO para item individual de moderação
/// </summary>
public class ModerationItemDto
{
    public int Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public int ContentId { get; set; }
    public string ContentText { get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public string? ReviewReason { get; set; }
    public List<string> Flags { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO para estatísticas de moderação
/// </summary>
public class ModerationStatsDto
{
    public int PendingItems { get; set; }
    public int ProcessedToday { get; set; }
    public int ApprovedToday { get; set; }
    public int RejectedToday { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
    public int TotalModeratorsActive { get; set; }
    public Dictionary<string, int> ContentTypeBreakdown { get; set; } = new();
    public Dictionary<string, int> PriorityBreakdown { get; set; } = new();
}

/// <summary>
/// DTO para analytics detalhados
/// </summary>
public class ModerationAnalyticsDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalProcessed { get; set; }
    public int TotalApproved { get; set; }
    public int TotalRejected { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal RejectionRate { get; set; }
    public List<DailyModerationStat> DailyStats { get; set; } = new();
    public List<ModeratorPerformance> ModeratorPerformance { get; set; } = new();
    public Dictionary<string, int> RejectionReasons { get; set; } = new();
    public Dictionary<string, decimal> ContentTypeApprovalRates { get; set; } = new();
}

/// <summary>
/// Estatística diária de moderação
/// </summary>
public class DailyModerationStat
{
    public DateTime Date { get; set; }
    public int Processed { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public decimal ApprovalRate { get; set; }
}

/// <summary>
/// Performance de moderador
/// </summary>
public class ModeratorPerformance
{
    public string ModeratorName { get; set; } = string.Empty;
    public string ModeratorEmail { get; set; } = string.Empty;
    public int ItemsProcessed { get; set; }
    public int ItemsApproved { get; set; }
    public int ItemsRejected { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
}

/// <summary>
/// Configurações de moderação
/// </summary>
public class ModerationConfigDto
{
    public bool AutoApproveEnabled { get; set; }
    public List<string> AutoApproveKeywords { get; set; } = new();
    public List<string> AutoRejectKeywords { get; set; } = new();
    public int MaxProcessingTimeHours { get; set; }
    public bool RequireDoubleApproval { get; set; }
    public List<string> SensitiveContentTypes { get; set; } = new();
    public Dictionary<string, object> NotificationSettings { get; set; } = new();
}

/// <summary>
/// Relatório de moderação
/// </summary>
public class ModerationReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ReportId { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;

    // Resumo executivo
    public ModerationReportSummary Summary { get; set; } = new();

    // Dados detalhados
    public List<DailyModerationStat> DailyBreakdown { get; set; } = new();
    public List<ModeratorPerformance> ModeratorPerformance { get; set; } = new();
    public Dictionary<string, int> ContentTypeStats { get; set; } = new();
    public Dictionary<string, int> RejectionReasonStats { get; set; } = new();

    // Recomendações
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Resumo do relatório de moderação
/// </summary>
public class ModerationReportSummary
{
    public int TotalItemsProcessed { get; set; }
    public int TotalApproved { get; set; }
    public int TotalRejected { get; set; }
    public decimal OverallApprovalRate { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
    public int UniqueModerators { get; set; }
    public string MostActiveDay { get; set; } = string.Empty;
    public string TopRejectionReason { get; set; } = string.Empty;
}
