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
    /// <summary>
    /// Número da página para paginação.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Filtro por título do asset (opcional).
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Filtro por tipo de asset (opcional).
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// Filtro por nível de acesso do documento (opcional).
    /// </summary>
    public DocumentAccessLevel? AccessLevel { get; set; }

    /// <summary>
    /// Filtro por data de criação mínima (opcional).
    /// </summary>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filtro por data de criação máxima (opcional).
    /// </summary>
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// ID do departamento para filtrar assets (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// ID do autor para filtrar assets (opcional).
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// Lista de IDs de tags para filtrar assets (opcional).
    /// </summary>
    public List<Guid>? TagIds { get; set; }

    /// <summary>
    /// Campo para ordenação dos resultados.
    /// </summary>
    public string SortBy { get; set; } = "CreatedAt";

    /// <summary>
    /// Direção da ordenação (asc/desc).
    /// </summary>
    public string SortOrder { get; set; } = "desc";
}

/// <summary>
/// Query para buscar asset específico por ID
/// </summary>
public class GetMediaAssetByIdQuery : IRequest<MediaAssetDetailDto?>
{
    /// <summary>
    /// ID do asset de mídia a ser recuperado.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Inicializa query para buscar asset específico.
    /// </summary>
    /// <param name="assetId">ID do asset a ser buscado.</param>
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
    /// <summary>
    /// ID do asset de mídia cujo arquivo será recuperado.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Inicializa query para obter arquivo do asset.
    /// </summary>
    /// <param name="assetId">ID do asset cujo arquivo será obtido.</param>
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
    /// <summary>
    /// ID do asset de mídia para gerar thumbnail.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Tamanho da thumbnail (small, medium, large).
    /// </summary>
    public string Size { get; }

    /// <summary>
    /// Inicializa query para obter thumbnail do asset.
    /// </summary>
    /// <param name="assetId">ID do asset para gerar thumbnail.</param>
    /// <param name="size">Tamanho da thumbnail desejada.</param>
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
    /// <summary>
    /// ID do asset de mídia para obter estatísticas.
    /// </summary>
    public Guid AssetId { get; }

    /// <summary>
    /// Inicializa query para obter estatísticas do asset.
    /// </summary>
    /// <param name="assetId">ID do asset para obter estatísticas.</param>
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
    /// <summary>
    /// Tipo de asset a ser filtrado.
    /// </summary>
    public string AssetType { get; set; } = string.Empty;

    /// <summary>
    /// Número da página para paginação.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Nível de acesso para filtrar assets (opcional).
    /// </summary>
    public DocumentAccessLevel? AccessLevel { get; set; }

    /// <summary>
    /// ID do departamento para filtrar assets (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }
}

/// <summary>
/// Query para buscar galeria do departamento
/// </summary>
public class GetDepartmentGalleryQuery : IRequest<PagedResult<MediaAssetDto>>
{
    /// <summary>
    /// ID do departamento para buscar galeria.
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Número da página para paginação.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Tipo de asset para filtrar (opcional).
    /// </summary>
    public string? AssetType { get; set; }
}

/// <summary>
/// Query para buscar assets populares
/// </summary>
public class GetPopularAssetsQuery : IRequest<List<MediaAssetDto>>
{
    /// <summary>
    /// Limite de assets populares a retornar.
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Período de análise (week, month, year).
    /// </summary>
    public string Period { get; set; } = "month";

    /// <summary>
    /// ID do departamento para filtrar assets (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Tipo de asset para filtrar (opcional).
    /// </summary>
    public string? AssetType { get; set; }
}

/// <summary>
/// Query para buscar assets recentes
/// </summary>
public class GetRecentAssetsQuery : IRequest<List<MediaAssetDto>>
{
    /// <summary>
    /// Limite de assets recentes a retornar.
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// ID do departamento para filtrar assets (opcional).
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Tipo de asset para filtrar (opcional).
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// Nível máximo de acesso permitido (opcional).
    /// </summary>
    public DocumentAccessLevel? MaxAccessLevel { get; set; }
}

/// <summary>
/// Query para buscar meus assets
/// </summary>
public class GetMyAssetsQuery : IRequest<PagedResult<MediaAssetDto>>
{
    /// <summary>
    /// ID do usuário para buscar seus assets.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Número da página para paginação.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página para paginação.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Tipo de asset para filtrar (opcional).
    /// </summary>
    public string? AssetType { get; set; }
}
