using System.ComponentModel.DataAnnotations;
using MediatR;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.MediaAssets.Commands;

public class UploadMediaAssetCommand : IRequest<MediaAssetDto>
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

public class UpdateMediaAssetCommand : IRequest<MediaAssetDto?>
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DocumentAccessLevel? AccessLevel { get; set; }
    public List<Guid>? TagIds { get; set; }
}

public class DeleteMediaAssetCommand : IRequest<bool>
{
    public Guid AssetId { get; }

    public DeleteMediaAssetCommand(Guid assetId)
    {
        AssetId = assetId;
    }
}

public class BulkUploadMediaAssetsCommand : IRequest<List<MediaAssetDto>>
{
    [Required]
    public List<BulkUploadItemDto> Assets { get; set; } = new();

    public Guid? DepartmentId { get; set; }
    public DocumentAccessLevel DefaultAccessLevel { get; set; } = DocumentAccessLevel.Internal;
    public List<Guid>? DefaultTagIds { get; set; }
}

public class RegisterMediaAssetAccessCommand : IRequest<bool>
{
    [Required]
    public Guid AssetId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty; // View, Download

    public string? IpAddress { get; set; }
}

public class GenerateThumbnailCommand : IRequest<bool>
{
    [Required]
    public Guid AssetId { get; set; }

    public int Width { get; set; } = 300;
    public int Height { get; set; } = 300;
    public bool Force { get; set; }
}
