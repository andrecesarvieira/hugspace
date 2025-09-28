using SynQcore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.MediaAssets.DTOs;

public class MediaAssetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public DocumentAccessLevel AccessLevel { get; set; }
    public long FileSizeBytes { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Propriedades específicas de mídia
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; } // Em segundos para vídeo/áudio

    // Relacionamentos
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }

    // Tags
    public List<string> Tags { get; set; } = new();

    // Estatísticas básicas
    public int ViewCount { get; set; }
    public int DownloadCount { get; set; }
}

public class MediaAssetDetailDto : MediaAssetDto
{
    public DateTime? LastAccessedAt { get; set; }
    public List<MediaAssetAccessDto> RecentAccesses { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class MediaAssetAccessDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // View, Download
    public DateTime AccessedAt { get; set; }
    public string? IpAddress { get; set; }
}

public class MediaAssetStatsDto
{
    public Guid AssetId { get; set; }
    public int TotalViews { get; set; }
    public int TotalDownloads { get; set; }
    public int UniqueViewers { get; set; }
    public int UniqueDownloaders { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public string? MostActiveUser { get; set; }
    public Dictionary<string, int> ViewsByDepartment { get; set; } = new();
    public Dictionary<DateTime, int> ViewsByDate { get; set; } = new();
}

public class MediaAssetFileDto
{
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public class MediaAssetThumbnailDto
{
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}

public class BulkUploadItemDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string AssetType { get; set; } = string.Empty;

    [Required]
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FileContentType { get; set; } = string.Empty;

    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public List<Guid>? TagIds { get; set; }
}

// Request DTOs

public class GetMediaAssetsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Title { get; set; }
    public string? AssetType { get; set; }
    public DocumentAccessLevel? AccessLevel { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? AuthorId { get; set; }
    public List<Guid>? TagIds { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

public class UploadMediaAssetRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string AssetType { get; set; } = string.Empty;

    public DocumentAccessLevel AccessLevel { get; set; } = DocumentAccessLevel.Internal;
    public Guid? DepartmentId { get; set; }
    public List<Guid>? TagIds { get; set; }

    [Required]
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FileContentType { get; set; } = string.Empty;

    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
}

public class UpdateMediaAssetRequest
{
    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DocumentAccessLevel? AccessLevel { get; set; }
    public List<Guid>? TagIds { get; set; }
}

public class BulkUploadMediaAssetsRequest
{
    [Required]
    public List<BulkUploadItemDto> Assets { get; set; } = new();

    public Guid? DepartmentId { get; set; }
    public DocumentAccessLevel DefaultAccessLevel { get; set; } = DocumentAccessLevel.Internal;
    public List<Guid>? DefaultTagIds { get; set; }
}
