using MediatR;
using SynQcore.Application.Features.Privacy.DTOs;

namespace SynQcore.Application.Features.Privacy.Commands;

/// <summary>
/// Command para criar registro de consentimento
/// </summary>
public class CreateConsentRecordCommand : IRequest<ConsentRecordDto>
{
    public Guid EmployeeId { get; set; }
    public string ConsentCategory { get; set; } = string.Empty;
    public string ProcessingPurpose { get; set; } = string.Empty;
    public bool ConsentGranted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string TermsVersion { get; set; } = string.Empty;
    public string CollectionEvidence { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// Command para atualizar registro de consentimento
/// </summary>
public class UpdateConsentRecordCommand : IRequest<ConsentRecordDto?>
{
    public Guid Id { get; set; }
    public bool ConsentGranted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Notes { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

/// <summary>
/// Command para revogar consentimento
/// </summary>
public class RevokeConsentCommand : IRequest<ConsentRecordDto?>
{
    public Guid Id { get; set; }
    public string RevocationReason { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

/// <summary>
/// Command para criar solicitação de exportação de dados
/// </summary>
public class CreateDataExportRequestCommand : IRequest<DataExportRequestDto>
{
    public Guid EmployeeId { get; set; }
    public string[] DataCategories { get; set; } = Array.Empty<string>();
    public string Format { get; set; } = "JSON";
    public string? Reason { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

/// <summary>
/// Command para processar solicitação de exportação
/// </summary>
public class ProcessDataExportRequestCommand : IRequest<DataExportRequestDto?>
{
    public Guid Id { get; set; }
    public Guid ProcessedById { get; set; }
    public bool Approved { get; set; }
    public string? ProcessingNotes { get; set; }
}

/// <summary>
/// Command para gerar arquivo de exportação
/// </summary>
public class GenerateDataExportFileCommand : IRequest<DataExportRequestDto?>
{
    public Guid RequestId { get; set; }
    public Guid ProcessedById { get; set; }
}

/// <summary>
/// Command para download de arquivo de exportação
/// </summary>
public class DownloadDataExportCommand : IRequest<byte[]?>
{
    public Guid RequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}

/// <summary>
/// Command para criar solicitação de exclusão de dados
/// </summary>
public class CreateDataDeletionRequestCommand : IRequest<DataDeletionRequestDto>
{
    public Guid EmployeeId { get; set; }
    public string DeletionType { get; set; } = "PersonalData";
    public string[] DataCategories { get; set; } = Array.Empty<string>();
    public string Reason { get; set; } = string.Empty;
    public string? LegalJustification { get; set; }
    public bool IncludeBackups { get; set; } = true;
    public bool CompleteDeletion { get; set; } = true;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

/// <summary>
/// Command para processar solicitação de exclusão
/// </summary>
public class ProcessDataDeletionRequestCommand : IRequest<DataDeletionRequestDto?>
{
    public Guid Id { get; set; }
    public Guid ProcessedById { get; set; }
    public bool Approved { get; set; }
    public string? ProcessingNotes { get; set; }
    public DateTime? EffectiveDeletionDate { get; set; }
}

/// <summary>
/// Command para executar exclusão de dados
/// </summary>
public class ExecuteDataDeletionCommand : IRequest<DataDeletionRequestDto?>
{
    public Guid RequestId { get; set; }
    public Guid ProcessedById { get; set; }
    public bool SendNotification { get; set; } = true;
}

/// <summary>
/// Command para cancelar solicitação
/// </summary>
public class CancelPrivacyRequestCommand : IRequest<bool>
{
    public Guid RequestId { get; set; }
    public string RequestType { get; set; } = string.Empty; // "Export" ou "Deletion"
    public Guid EmployeeId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}

/// <summary>
/// Command para criar categoria de dados pessoais
/// </summary>
public class CreatePersonalDataCategoryCommand : IRequest<PersonalDataCategoryDto>
{
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SensitivityLevel { get; set; } = "Normal";
    public string ProcessingPurpose { get; set; } = string.Empty;
    public string LegalBasis { get; set; } = "Consent";
    public int RetentionPeriodMonths { get; set; }
    public bool RequiresConsent { get; set; } = true;
    public bool IsMandatoryData { get; set; }
    public bool AllowsSharing { get; set; }
    public bool InternationalTransfer { get; set; }
    public string? AuthorizedCountries { get; set; }
    public string? SecurityMeasures { get; set; }
    public string IncludedFields { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public DateTime EffectiveDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Command para atualizar categoria de dados pessoais
/// </summary>
public class UpdatePersonalDataCategoryCommand : IRequest<PersonalDataCategoryDto?>
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
    public string IncludedFields { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Command para desativar categoria de dados pessoais
/// </summary>
public class DeactivatePersonalDataCategoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string DeactivationReason { get; set; } = string.Empty;
}

/// <summary>
/// Command para processar expiração automática de consentimentos
/// </summary>
public class ProcessExpiredConsentsCommand : IRequest<int>
{
    public DateTime? CheckDate { get; set; }
    public bool SendNotifications { get; set; } = true;
}

/// <summary>
/// Command para anonymizar dados antigos
/// </summary>
public class AnonymizeOldDataCommand : IRequest<int>
{
    public DateTime CutoffDate { get; set; }
    public string[] DataCategories { get; set; } = Array.Empty<string>();
    public bool DryRun { get; set; }
}
