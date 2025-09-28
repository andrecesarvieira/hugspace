using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

public class CorporateDocument : BaseEntity
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string OriginalFileName { get; set; }

    public required string StorageFileName { get; set; }

    public required string ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentType Type { get; set; }

    public DocumentStatus Status { get; set; }

    public DocumentAccessLevel AccessLevel { get; set; }

    public DocumentCategory Category { get; set; }

    public Guid UploadedByEmployeeId { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    public Guid? ApprovedByEmployeeId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public string? Tags { get; set; }

    public required string Version { get; set; }

    public Guid? ParentDocumentId { get; set; }

    public bool IsCurrentVersion { get; set; }

    public string? FileHash { get; set; }

    public string? ExternalStorageUrl { get; set; }

    public string? Metadata { get; set; }

    public int DownloadCount { get; set; }

    public DateTimeOffset? LastAccessedAt { get; set; }

    // Relacionamentos
    public Employee UploadedByEmployee { get; set; } = null!;

    public Department? OwnerDepartment { get; set; }

    public Employee? ApprovedByEmployee { get; set; }

    public CorporateDocument? ParentDocument { get; set; }

    public ICollection<CorporateDocument> ChildVersions { get; set; } = new List<CorporateDocument>();

    public ICollection<DocumentAccess> DocumentAccesses { get; set; } = new List<DocumentAccess>();

    public ICollection<DocumentAccessLog> AccessLogs { get; set; } = new List<DocumentAccessLog>();
}