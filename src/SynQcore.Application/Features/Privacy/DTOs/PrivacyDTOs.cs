namespace SynQcore.Application.Features.Privacy.DTOs;

/// <summary>
/// DTO para visualização de registro de consentimento
/// </summary>
public class ConsentRecordDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public string ConsentCategory { get; set; } = string.Empty;
    public string ProcessingPurpose { get; set; } = string.Empty;
    public bool ConsentGranted { get; set; }
    public DateTime ConsentDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string TermsVersion { get; set; } = string.Empty;
    public string CollectionEvidence { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastModificationDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO para criação de registro de consentimento
/// </summary>
public class CreateConsentRecordDto
{
    public string ConsentCategory { get; set; } = string.Empty;
    public string ProcessingPurpose { get; set; } = string.Empty;
    public bool ConsentGranted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string TermsVersion { get; set; } = string.Empty;
    public string CollectionEvidence { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// DTO para atualização de consentimento
/// </summary>
public class UpdateConsentRecordDto
{
    public Guid Id { get; set; }
    public bool ConsentGranted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO para solicitação de exportação de dados
/// </summary>
public class DataExportRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime? ProcessingDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string DataCategories { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public DateTime? DownloadExpirationDate { get; set; }
    public string? Reason { get; set; }
    public string? ProcessingNotes { get; set; }
    public string? ProcessedByName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public int DownloadAttempts { get; set; }
}

/// <summary>
/// DTO para criar solicitação de exportação
/// </summary>
public class CreateDataExportRequestDto
{
    public string[] DataCategories { get; set; } = Array.Empty<string>();
    public string Format { get; set; } = "JSON";
    public string? Reason { get; set; }
}

/// <summary>
/// DTO para solicitação de exclusão de dados
/// </summary>
public class DataDeletionRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime? ProcessingDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string DeletionType { get; set; } = string.Empty;
    public string DataCategories { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? LegalJustification { get; set; }
    public bool IncludeBackups { get; set; }
    public bool CompleteDeletion { get; set; }
    public int GracePeriodDays { get; set; }
    public DateTime? EffectiveDeletionDate { get; set; }
    public string? DeletionReport { get; set; }
    public string? ProcessingNotes { get; set; }
    public string? ProcessedByName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public bool NotificationSent { get; set; }
    public DateTime? NotificationDate { get; set; }
}

/// <summary>
/// DTO para criar solicitação de exclusão
/// </summary>
public class CreateDataDeletionRequestDto
{
    public string DeletionType { get; set; } = "PersonalData";
    public string[] DataCategories { get; set; } = Array.Empty<string>();
    public string Reason { get; set; } = string.Empty;
    public string? LegalJustification { get; set; }
    public bool IncludeBackups { get; set; } = true;
    public bool CompleteDeletion { get; set; } = true;
}

/// <summary>
/// DTO para categoria de dados pessoais
/// </summary>
public class PersonalDataCategoryDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SensitivityLevel { get; set; } = string.Empty;
    public string ProcessingPurpose { get; set; } = string.Empty;
    public string LegalBasis { get; set; } = string.Empty;
    public int RetentionPeriodMonths { get; set; }
    public bool RequiresConsent { get; set; }
    public bool IsMandatoryData { get; set; }
    public bool AllowsSharing { get; set; }
    public bool InternationalTransfer { get; set; }
    public string? AuthorizedCountries { get; set; }
    public string? SecurityMeasures { get; set; }
    public bool IsActive { get; set; }
    public string IncludedFields { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para relatório de compliance
/// </summary>
public class ComplianceReportDto
{
    public int TotalConsents { get; set; }
    public int ActiveConsents { get; set; }
    public int RevokedConsents { get; set; }
    public int ExportRequests { get; set; }
    public int DeletionRequests { get; set; }
    public int PendingRequests { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Dictionary<string, int> ConsentsByCategory { get; set; } = new();
    public Dictionary<string, int> RequestsByStatus { get; set; } = new();
    public Dictionary<string, int> ExportsByFormat { get; set; } = new();
    public List<ConsentRecordDto> RecentConsents { get; set; } = new();
    public List<DataExportRequestDto> RecentExports { get; set; } = new();
    public List<DataDeletionRequestDto> RecentDeletions { get; set; } = new();
}

/// <summary>
/// DTO para dashboard de privacidade
/// </summary>
public class PrivacyDashboardDto
{
    public ComplianceReportDto ComplianceReport { get; set; } = new();
    public List<PersonalDataCategoryDto> DataCategories { get; set; } = new();
    public Dictionary<string, int> ComplianceAlerts { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}
