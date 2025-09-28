using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

public class DocumentTemplate : BaseEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public DocumentType DocumentType { get; set; }

    public DocumentCategory DefaultCategory { get; set; }

    public DocumentAccessLevel DefaultAccessLevel { get; set; }

    public required string TemplateFileName { get; set; }

    public required string ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public string? StorageUrl { get; set; }

    public required string Version { get; set; }

    public bool IsActive { get; set; }

    public bool IsDefault { get; set; }

    public string? Placeholders { get; set; }

    public string? UsageInstructions { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    public Guid CreatedByEmployeeId { get; set; }

    public string? Tags { get; set; }

    public int UsageCount { get; set; }

    public DateTimeOffset? LastUsedAt { get; set; }

    // Relacionamentos
    public Department? OwnerDepartment { get; set; }

    public Employee CreatedByEmployee { get; set; } = null!;
}

public class MediaAsset : BaseEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public MediaAssetType Type { get; set; }

    public MediaAssetCategory Category { get; set; }

    public required string OriginalFileName { get; set; }

    public required string StorageFileName { get; set; }

    public required string ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? DurationSeconds { get; set; }

    public string? StorageUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public DocumentAccessLevel AccessLevel { get; set; }

    public bool IsApproved { get; set; }

    public Guid UploadedByEmployeeId { get; set; }

    public Guid? ApprovedByEmployeeId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public string? Tags { get; set; }

    public string? Metadata { get; set; }

    public int DownloadCount { get; set; }

    // Relacionamentos
    public Employee UploadedByEmployee { get; set; } = null!;

    public Employee? ApprovedByEmployee { get; set; }
}