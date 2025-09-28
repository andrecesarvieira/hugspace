using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Application.Features.MediaAssets.Queries;
using SynQcore.Application.Common.Extensions;
using System.Globalization;

namespace SynQcore.Application.Features.MediaAssets.Handlers;

/// <summary>
/// Handler para obter asset específico por ID
/// </summary>
public class GetMediaAssetByIdQueryHandler : IRequestHandler<GetMediaAssetByIdQuery, MediaAssetDetailDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMediaAssetByIdQueryHandler> _logger;

    private static readonly Action<ILogger, Guid, Exception?> LogBuscandoAssetPorId =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5101, nameof(LogBuscandoAssetPorId)),
            "Buscando asset de mídia por ID: {AssetId}");

    private static readonly Action<ILogger, Guid, Exception?> LogAssetEncontrado =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5102, nameof(LogAssetEncontrado)),
            "Asset de mídia encontrado: {AssetId}");

    private static readonly Action<ILogger, Guid, Exception?> LogAssetNaoEncontrado =
        LoggerMessage.Define<Guid>(LogLevel.Warning, new EventId(5103, nameof(LogAssetNaoEncontrado)),
            "Asset de mídia não encontrado: {AssetId}");

    public GetMediaAssetByIdQueryHandler(ISynQcoreDbContext context, ILogger<GetMediaAssetByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MediaAssetDetailDto?> Handle(GetMediaAssetByIdQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoAssetPorId(_logger, request.AssetId, null);

        var asset = await _context.MediaAssets
            .Include(m => m.UploadedByEmployee)
            .Include(m => m.ApprovedByEmployee)
            .FirstOrDefaultAsync(m => m.Id == request.AssetId && !m.IsDeleted, cancellationToken);

        if (asset == null)
        {
            LogAssetNaoEncontrado(_logger, request.AssetId, null);
            return null;
        }

        LogAssetEncontrado(_logger, request.AssetId, null);
        return asset.ToMediaAssetDetailDto();
    }
}

/// <summary>
/// Handler para obter assets por tipo
/// </summary>
public class GetMediaAssetsByTypeQueryHandler : IRequestHandler<GetMediaAssetsByTypeQuery, PagedResult<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMediaAssetsByTypeQueryHandler> _logger;

    private static readonly Action<ILogger, string, int, Exception?> LogBuscandoAssetsPorTipo =
        LoggerMessage.Define<string, int>(LogLevel.Information, new EventId(5104, nameof(LogBuscandoAssetsPorTipo)),
            "Buscando assets por tipo: {AssetType} - Página: {Page}");

    public GetMediaAssetsByTypeQueryHandler(ISynQcoreDbContext context, ILogger<GetMediaAssetsByTypeQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<MediaAssetDto>> Handle(GetMediaAssetsByTypeQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoAssetsPorTipo(_logger, request.AssetType, request.Page, null);

        var query = _context.MediaAssets
            .Where(m => !m.IsDeleted && m.Type.ToString().Contains(request.AssetType));

        if (request.AccessLevel.HasValue)
        {
            query = query.Where(m => m.AccessLevel == request.AccessLevel);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(m => m.UploadedByEmployee != null &&
                m.UploadedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId));
        }

        return await query.OrderByDescending(m => m.CreatedAt)
            .ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}

/// <summary>
/// Handler para obter galeria de departamento
/// </summary>
public class GetDepartmentGalleryQueryHandler : IRequestHandler<GetDepartmentGalleryQuery, PagedResult<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetDepartmentGalleryQueryHandler> _logger;

    private static readonly Action<ILogger, Guid, Exception?> LogBuscandoGaleriaDepartamento =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5105, nameof(LogBuscandoGaleriaDepartamento)),
            "Buscando galeria do departamento: {DepartmentId}");

    public GetDepartmentGalleryQueryHandler(ISynQcoreDbContext context, ILogger<GetDepartmentGalleryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<MediaAssetDto>> Handle(GetDepartmentGalleryQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoGaleriaDepartamento(_logger, request.DepartmentId, null);

        var query = _context.MediaAssets
            .Where(m => !m.IsDeleted && m.UploadedByEmployee != null &&
                m.UploadedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId));

        if (!string.IsNullOrEmpty(request.AssetType))
        {
            query = query.Where(m => m.Type.ToString().Contains(request.AssetType));
        }

        return await query.OrderByDescending(m => m.CreatedAt)
            .ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}

/// <summary>
/// Handler para obter assets populares
/// </summary>
public class GetPopularAssetsQueryHandler : IRequestHandler<GetPopularAssetsQuery, List<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetPopularAssetsQueryHandler> _logger;

    private static readonly Action<ILogger, int, string, Exception?> LogBuscandoAssetsPopulares =
        LoggerMessage.Define<int, string>(LogLevel.Information, new EventId(5106, nameof(LogBuscandoAssetsPopulares)),
            "Buscando assets populares - Limite: {Limit}, Período: {Period}");

    public GetPopularAssetsQueryHandler(ISynQcoreDbContext context, ILogger<GetPopularAssetsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MediaAssetDto>> Handle(GetPopularAssetsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoAssetsPopulares(_logger, request.Limit, request.Period, null);

        var dateFilter = request.Period.ToLower(CultureInfo.InvariantCulture) switch
        {
            "week" => DateTime.UtcNow.AddDays(-7),
            "month" => DateTime.UtcNow.AddMonths(-1),
            "year" => DateTime.UtcNow.AddYears(-1),
            _ => DateTime.UtcNow.AddMonths(-1)
        };

        var query = _context.MediaAssets
            .Where(m => !m.IsDeleted && m.CreatedAt >= dateFilter);

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(m => m.UploadedByEmployee != null &&
                m.UploadedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId));
        }

        if (!string.IsNullOrEmpty(request.AssetType))
        {
            query = query.Where(m => m.Type.ToString().Contains(request.AssetType));
        }

        var assets = await query
            .OrderByDescending(m => m.DownloadCount)
            .ThenByDescending(m => m.CreatedAt)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return assets.Select(m => m.ToMediaAssetDto()).ToList();
    }
}

/// <summary>
/// Handler para obter assets recentes
/// </summary>
public class GetRecentAssetsQueryHandler : IRequestHandler<GetRecentAssetsQuery, List<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetRecentAssetsQueryHandler> _logger;

    private static readonly Action<ILogger, int, Exception?> LogBuscandoAssetsRecentes =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5107, nameof(LogBuscandoAssetsRecentes)),
            "Buscando assets recentes - Limite: {Limit}");

    public GetRecentAssetsQueryHandler(ISynQcoreDbContext context, ILogger<GetRecentAssetsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MediaAssetDto>> Handle(GetRecentAssetsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoAssetsRecentes(_logger, request.Limit, null);

        var query = _context.MediaAssets
            .Where(m => !m.IsDeleted);

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(m => m.UploadedByEmployee != null &&
                m.UploadedByEmployee.EmployeeDepartments.Any(ed => ed.DepartmentId == request.DepartmentId));
        }

        if (!string.IsNullOrEmpty(request.AssetType))
        {
            query = query.Where(m => m.Type.ToString().Contains(request.AssetType));
        }

        if (request.MaxAccessLevel.HasValue)
        {
            query = query.Where(m => m.AccessLevel <= request.MaxAccessLevel);
        }

        var assets = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return assets.Select(m => m.ToMediaAssetDto()).ToList();
    }
}

/// <summary>
/// Handler para obter meus assets
/// </summary>
public class GetMyAssetsQueryHandler : IRequestHandler<GetMyAssetsQuery, PagedResult<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMyAssetsQueryHandler> _logger;

    private static readonly Action<ILogger, Guid, Exception?> LogBuscandoMeusAssets =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5108, nameof(LogBuscandoMeusAssets)),
            "Buscando assets do usuário: {UserId}");

    public GetMyAssetsQueryHandler(ISynQcoreDbContext context, ILogger<GetMyAssetsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<MediaAssetDto>> Handle(GetMyAssetsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoMeusAssets(_logger, request.UserId, null);

        var query = _context.MediaAssets
            .Where(m => !m.IsDeleted && m.UploadedByEmployeeId == request.UserId);

        if (!string.IsNullOrEmpty(request.AssetType))
        {
            query = query.Where(m => m.Type.ToString().Contains(request.AssetType));
        }

        return await query.OrderByDescending(m => m.CreatedAt)
            .ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}

/// <summary>
/// Handler para obter arquivo do asset
/// </summary>
public class GetMediaAssetFileQueryHandler : IRequestHandler<GetMediaAssetFileQuery, MediaAssetFileDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMediaAssetFileQueryHandler> _logger;

    private static readonly Action<ILogger, Guid, Exception?> LogBuscandoArquivoAsset =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5109, nameof(LogBuscandoArquivoAsset)),
            "Buscando arquivo do asset: {AssetId}");

    public GetMediaAssetFileQueryHandler(ISynQcoreDbContext context, ILogger<GetMediaAssetFileQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MediaAssetFileDto?> Handle(GetMediaAssetFileQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoArquivoAsset(_logger, request.AssetId, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.AssetId && !m.IsDeleted, cancellationToken);

        return asset?.ToMediaAssetFileDto();
    }
}

/// <summary>
/// Handler para obter thumbnail do asset
/// </summary>
public class GetMediaAssetThumbnailQueryHandler : IRequestHandler<GetMediaAssetThumbnailQuery, MediaAssetThumbnailDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMediaAssetThumbnailQueryHandler> _logger;

    private static readonly Action<ILogger, Guid, string, Exception?> LogBuscandoThumbnail =
        LoggerMessage.Define<Guid, string>(LogLevel.Information, new EventId(5110, nameof(LogBuscandoThumbnail)),
            "Buscando thumbnail do asset: {AssetId}, tamanho: {Size}");

    public GetMediaAssetThumbnailQueryHandler(ISynQcoreDbContext context, ILogger<GetMediaAssetThumbnailQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MediaAssetThumbnailDto?> Handle(GetMediaAssetThumbnailQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoThumbnail(_logger, request.AssetId, request.Size, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.AssetId && !m.IsDeleted, cancellationToken);

        return asset?.ToMediaAssetThumbnailDto(request.Size);
    }
}

/// <summary>
/// Handler para obter estatísticas do asset
/// </summary>
public class GetMediaAssetStatsQueryHandler : IRequestHandler<GetMediaAssetStatsQuery, MediaAssetStatsDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GetMediaAssetStatsQueryHandler> _logger;

    private static readonly Action<ILogger, Guid, Exception?> LogBuscandoEstatisticasAsset =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5111, nameof(LogBuscandoEstatisticasAsset)),
            "Buscando estatísticas do asset: {AssetId}");

    public GetMediaAssetStatsQueryHandler(ISynQcoreDbContext context, ILogger<GetMediaAssetStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MediaAssetStatsDto?> Handle(GetMediaAssetStatsQuery request, CancellationToken cancellationToken)
    {
        LogBuscandoEstatisticasAsset(_logger, request.AssetId, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.AssetId && !m.IsDeleted, cancellationToken);

        return asset?.ToMediaAssetStatsDto();
    }
}
