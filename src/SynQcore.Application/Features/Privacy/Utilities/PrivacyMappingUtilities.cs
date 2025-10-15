using SynQcore.Application.Features.Privacy.Commands;
using SynQcore.Application.Features.Privacy.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Privacy.Utilities;

/// <summary>
/// Utilitários de mapeamento para entidades de privacidade e compliance LGPD
/// </summary>
public static class PrivacyMappingUtilities
{
    /// <summary>
    /// Converte ConsentRecord para ConsentRecordDto
    /// </summary>
    public static ConsentRecordDto ToConsentRecordDto(this ConsentRecord entity)
    {
        return new ConsentRecordDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.Employee != null ? $"{entity.Employee.FirstName} {entity.Employee.LastName}".Trim() : string.Empty,
            EmployeeEmail = entity.Employee?.Email ?? string.Empty,
            ConsentCategory = entity.ConsentCategory,
            ProcessingPurpose = entity.ProcessingPurpose,
            ConsentGranted = entity.ConsentGranted,
            ConsentDate = entity.ConsentDate,
            ExpirationDate = entity.ExpirationDate,
            TermsVersion = entity.TermsVersion,
            CollectionEvidence = entity.CollectionEvidence,
            IpAddress = entity.IpAddress,
            IsActive = entity.IsActive,
            LastModificationDate = entity.LastModificationDate,
            Notes = entity.Notes
        };
    }

    /// <summary>
    /// Converte DataExportRequest para DataExportRequestDto
    /// </summary>
    public static DataExportRequestDto ToDataExportRequestDto(this DataExportRequest entity)
    {
        return new DataExportRequestDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.Employee != null ? $"{entity.Employee.FirstName} {entity.Employee.LastName}".Trim() : string.Empty,
            EmployeeEmail = entity.Employee?.Email ?? string.Empty,
            Status = entity.Status.ToString(),
            RequestDate = entity.RequestDate,
            ProcessingDate = entity.ProcessingDate,
            CompletionDate = entity.CompletionDate,
            DataCategories = entity.DataCategories,
            Format = entity.Format.ToString(),
            FilePath = entity.FilePath,
            FileSize = entity.FileSize,
            DownloadExpirationDate = entity.DownloadExpirationDate,
            Reason = entity.Reason,
            ProcessingNotes = entity.ProcessingNotes,
            ProcessedByName = entity.ProcessedBy != null ? $"{entity.ProcessedBy.FirstName} {entity.ProcessedBy.LastName}".Trim() : null,
            IpAddress = entity.IpAddress,
            DownloadAttempts = entity.DownloadAttempts
        };
    }

    /// <summary>
    /// Converte DataDeletionRequest para DataDeletionRequestDto
    /// </summary>
    public static DataDeletionRequestDto ToDataDeletionRequestDto(this DataDeletionRequest entity)
    {
        return new DataDeletionRequestDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.Employee != null ? $"{entity.Employee.FirstName} {entity.Employee.LastName}".Trim() : string.Empty,
            EmployeeEmail = entity.Employee?.Email ?? string.Empty,
            Status = entity.Status.ToString(),
            RequestDate = entity.RequestDate,
            ProcessingDate = entity.ProcessingDate,
            CompletionDate = entity.CompletionDate,
            DeletionType = entity.DeletionType.ToString(),
            DataCategories = entity.DataCategories,
            Reason = entity.Reason,
            LegalJustification = entity.LegalJustification,
            IncludeBackups = entity.IncludeBackups,
            CompleteDeletion = entity.CompleteDeletion,
            GracePeriodDays = entity.GracePeriodDays,
            EffectiveDeletionDate = entity.EffectiveDeletionDate,
            DeletionReport = entity.DeletionReport,
            ProcessingNotes = entity.ProcessingNotes,
            ProcessedByName = entity.ProcessedBy != null ? $"{entity.ProcessedBy.FirstName} {entity.ProcessedBy.LastName}".Trim() : null,
            IpAddress = entity.IpAddress,
            NotificationSent = entity.NotificationSent,
            NotificationDate = entity.NotificationDate
        };
    }

    /// <summary>
    /// Converte PersonalDataCategory para PersonalDataCategoryDto
    /// </summary>
    public static PersonalDataCategoryDto ToPersonalDataCategoryDto(this PersonalDataCategory entity)
    {
        return new PersonalDataCategoryDto
        {
            Id = entity.Id,
            CategoryName = entity.CategoryName,
            Description = entity.Description,
            SensitivityLevel = entity.SensitivityLevel.ToString(),
            ProcessingPurpose = entity.ProcessingPurpose,
            LegalBasis = entity.LegalBasis.ToString(),
            RetentionPeriodMonths = entity.RetentionPeriodMonths,
            RequiresConsent = entity.RequiresConsent,
            IsMandatoryData = entity.IsMandatoryData,
            AllowsSharing = entity.AllowsSharing,
            InternationalTransfer = entity.InternationalTransfer,
            AuthorizedCountries = entity.AuthorizedCountries,
            SecurityMeasures = entity.SecurityMeasures,
            IsActive = entity.IsActive,
            IncludedFields = entity.IncludedFields,
            Version = entity.Version,
            EffectiveDate = entity.EffectiveDate,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt
        };
    }

    /// <summary>
    /// Converte CreateConsentRecordCommand para ConsentRecord
    /// </summary>
    public static ConsentRecord ToConsentRecord(this CreateConsentRecordDto dto, Guid employeeId, string ipAddress, string userAgent)
    {
        return new ConsentRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ConsentCategory = dto.ConsentCategory,
            ProcessingPurpose = dto.ProcessingPurpose,
            ConsentGranted = dto.ConsentGranted,
            ConsentDate = DateTime.UtcNow,
            ExpirationDate = dto.ExpirationDate,
            TermsVersion = dto.TermsVersion,
            CollectionEvidence = dto.CollectionEvidence,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsActive = true,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Converte CreatePersonalDataCategoryDto para PersonalDataCategory
    /// </summary>
    public static PersonalDataCategory ToPersonalDataCategory(this CreatePersonalDataCategoryCommand command)
    {
        return new PersonalDataCategory
        {
            Id = Guid.NewGuid(),
            CategoryName = command.CategoryName,
            Description = command.Description,
            SensitivityLevel = Enum.Parse<SensitivityLevel>(command.SensitivityLevel, true),
            ProcessingPurpose = command.ProcessingPurpose,
            LegalBasis = Enum.Parse<LegalBasisForProcessing>(command.LegalBasis, true),
            RetentionPeriodMonths = command.RetentionPeriodMonths,
            RequiresConsent = command.RequiresConsent,
            IsMandatoryData = command.IsMandatoryData,
            AllowsSharing = command.AllowsSharing,
            InternationalTransfer = command.InternationalTransfer,
            AuthorizedCountries = command.AuthorizedCountries,
            SecurityMeasures = command.SecurityMeasures,
            IsActive = true,
            IncludedFields = command.IncludedFields,
            Version = command.Version,
            EffectiveDate = command.EffectiveDate,
            Notes = command.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Atualiza ConsentRecord com dados do UpdateConsentRecordCommand
    /// </summary>
    public static void UpdateFromCommand(this ConsentRecord entity, UpdateConsentRecordDto command, string ipAddress, string userAgent)
    {
        entity.ConsentGranted = command.ConsentGranted;
        entity.ExpirationDate = command.ExpirationDate;
        entity.Notes = command.Notes;
        entity.IpAddress = ipAddress;
        entity.UserAgent = userAgent;
        entity.LastModificationDate = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza PersonalDataCategory com dados do UpdatePersonalDataCategoryCommand
    /// </summary>
    public static void UpdateFromCommand(this PersonalDataCategory entity, UpdatePersonalDataCategoryCommand command)
    {
        entity.CategoryName = command.CategoryName;
        entity.Description = command.Description;
        entity.SensitivityLevel = Enum.Parse<SensitivityLevel>(command.SensitivityLevel, true);
        entity.ProcessingPurpose = command.ProcessingPurpose;
        entity.LegalBasis = Enum.Parse<LegalBasisForProcessing>(command.LegalBasis, true);
        entity.RetentionPeriodMonths = command.RetentionPeriodMonths;
        entity.RequiresConsent = command.RequiresConsent;
        entity.IsMandatoryData = command.IsMandatoryData;
        entity.AllowsSharing = command.AllowsSharing;
        entity.InternationalTransfer = command.InternationalTransfer;
        entity.AuthorizedCountries = command.AuthorizedCountries;
        entity.SecurityMeasures = command.SecurityMeasures;
        entity.IncludedFields = command.IncludedFields;
        entity.Version = command.Version;
        entity.EffectiveDate = command.EffectiveDate;
        entity.Notes = command.Notes;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
