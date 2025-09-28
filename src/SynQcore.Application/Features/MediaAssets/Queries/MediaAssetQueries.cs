using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.MediaAssets.Queries;

public class GetMediaAssetsQuery : IRequest<PagedResult<MediaAssetDto>>
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

public class GetMediaAssetByIdQuery : IRequest<MediaAssetDetailDto?>
{
    public Guid AssetId { get; }

    public GetMediaAssetByIdQuery(Guid assetId)
    {
        AssetId = assetId;
    }
}

public class GetMediaAssetFileQuery : IRequest<MediaAssetFileDto?>
{
    public Guid AssetId { get; }

    public GetMediaAssetFileQuery(Guid assetId)
    {
        AssetId = assetId;
    }
}

public class GetMediaAssetThumbnailQuery : IRequest<MediaAssetThumbnailDto?>
{
    public Guid AssetId { get; }

    public string Size { get; }

    public GetMediaAssetThumbnailQuery(Guid assetId, string size = "medium")
    {
        AssetId = assetId;
        Size = size;
    }
}

public class GetMediaAssetStatsQuery : IRequest<MediaAssetStatsDto?>
{
    public Guid AssetId { get; }

    public GetMediaAssetStatsQuery(Guid assetId)
    {
        AssetId = assetId;
    }
}

public class GetMediaAssetsByTypeQuery : IRequest<PagedResult<MediaAssetDto>>
{
    public string AssetType { get; set; } = string.Empty;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public DocumentAccessLevel? AccessLevel { get; set; }

    public Guid? DepartmentId { get; set; }
}

public class GetDepartmentGalleryQuery : IRequest<PagedResult<MediaAssetDto>>
{
    public Guid DepartmentId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? AssetType { get; set; }
}

public class GetPopularAssetsQuery : IRequest<List<MediaAssetDto>>
{
    public int Limit { get; set; } = 10;

    public string Period { get; set; } = "month";

    public Guid? DepartmentId { get; set; }

    public string? AssetType { get; set; }
}

public class GetRecentAssetsQuery : IRequest<List<MediaAssetDto>>
{
    public int Limit { get; set; } = 10;

    public Guid? DepartmentId { get; set; }

    public string? AssetType { get; set; }

    public DocumentAccessLevel? MaxAccessLevel { get; set; }
}

public class GetMyAssetsQuery : IRequest<PagedResult<MediaAssetDto>>
{
    public Guid UserId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? AssetType { get; set; }
}
