using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.MediaAssets.Queries;

/// <summary>
/// Query para buscar assets de mídia com filtros
/// </summary>
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

/// <summary>
/// Query para buscar asset específico por ID
/// </summary>
public class GetMediaAssetByIdQuery : IRequest<MediaAssetDetailDto?>
{
    public Guid AssetId { get; }

    public GetMediaAssetByIdQuery(Guid assetId)
    {
        AssetId = assetId;
    }
}

/// <summary>
/// Query para obter arquivo do asset
/// </summary>
public class GetMediaAssetFileQuery : IRequest<MediaAssetFileDto?>
{
    public Guid AssetId { get; }

    public GetMediaAssetFileQuery(Guid assetId)
    {
        AssetId = assetId;
    }
}

/// <summary>
/// Query para obter thumbnail do asset
/// </summary>
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

/// <summary>
/// Query para obter estatísticas de asset
/// </summary>
public class GetMediaAssetStatsQuery : IRequest<MediaAssetStatsDto?>
{
    public Guid AssetId { get; }

    public GetMediaAssetStatsQuery(Guid assetId)
    {
        AssetId = assetId;
    }
}

/// <summary>
/// Query para buscar assets por tipo
/// </summary>
public class GetMediaAssetsByTypeQuery : IRequest<PagedResult<MediaAssetDto>>
{
    public string AssetType { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DocumentAccessLevel? AccessLevel { get; set; }
    public Guid? DepartmentId { get; set; }
}

/// <summary>
/// Query para buscar galeria do departamento
/// </summary>
public class GetDepartmentGalleryQuery : IRequest<PagedResult<MediaAssetDto>>
{
    public Guid DepartmentId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? AssetType { get; set; }
}

/// <summary>
/// Query para buscar assets populares
/// </summary>
public class GetPopularAssetsQuery : IRequest<List<MediaAssetDto>>
{
    public int Limit { get; set; } = 10;
    public string Period { get; set; } = "month"; // week, month, year
    public Guid? DepartmentId { get; set; }
    public string? AssetType { get; set; }
}

/// <summary>
/// Query para buscar assets recentes
/// </summary>
public class GetRecentAssetsQuery : IRequest<List<MediaAssetDto>>
{
    public int Limit { get; set; } = 10;
    public Guid? DepartmentId { get; set; }
    public string? AssetType { get; set; }
    public DocumentAccessLevel? MaxAccessLevel { get; set; }
}

/// <summary>
/// Query para buscar meus assets
/// </summary>
public class GetMyAssetsQuery : IRequest<PagedResult<MediaAssetDto>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? AssetType { get; set; }
}
