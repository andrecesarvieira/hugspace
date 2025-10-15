using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.MediaAssets.Commands;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.MediaAssets.Handlers;

/// <summary>
/// Handler para upload de asset de mídia
/// </summary>
public partial class UploadMediaAssetCommandHandler : IRequestHandler<UploadMediaAssetCommand, MediaAssetDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UploadMediaAssetCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Processando upload de asset: {FileName}")]
    private static partial void LogProcessandoUpload(ILogger logger, string fileName, Exception? exception);

    [LoggerMessage(LogLevel.Information, "Asset criado com sucesso: {AssetId}")]
    private static partial void LogAssetCriado(ILogger logger, Guid assetId, Exception? exception);

    public UploadMediaAssetCommandHandler(ISynQcoreDbContext context, ILogger<UploadMediaAssetCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MediaAssetDto> Handle(UploadMediaAssetCommand request, CancellationToken cancellationToken)
    {
        LogProcessandoUpload(_logger, request.FileName, null);

        // Gerar nome de arquivo único para armazenamento
        var storageFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";

        var asset = new MediaAsset
        {
            Id = Guid.NewGuid(),
            Name = request.Title,
            Description = request.Description,
            OriginalFileName = request.FileName,
            StorageFileName = storageFileName,
            FileSizeBytes = request.FileData.Length,
            ContentType = request.FileContentType,
            Type = Enum.Parse<MediaAssetType>(request.AssetType, true),
            Category = MediaAssetCategory.General,
            AccessLevel = request.AccessLevel,
            UploadedByEmployeeId = Guid.NewGuid(), // TODO: Obter do contexto de usuário
            IsApproved = false,
            DownloadCount = 0,
            Width = request.Width,
            Height = request.Height,
            DurationSeconds = request.Duration,
            Tags = string.Join(",", request.TagIds?.Select(t => t.ToString()) ?? []),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.MediaAssets.Add(asset);
        await _context.SaveChangesAsync(cancellationToken);

        LogAssetCriado(_logger, asset.Id, null);
        return asset.ToMediaAssetDto();
    }
}

/// <summary>
/// Handler para atualizar asset de mídia
/// </summary>
public partial class UpdateMediaAssetCommandHandler : IRequestHandler<UpdateMediaAssetCommand, MediaAssetDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateMediaAssetCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Atualizando asset: {AssetId}")]
    private static partial void LogAtualizandoAsset(ILogger logger, Guid assetId, Exception? exception);

    public UpdateMediaAssetCommandHandler(ISynQcoreDbContext context, ILogger<UpdateMediaAssetCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MediaAssetDto?> Handle(UpdateMediaAssetCommand request, CancellationToken cancellationToken)
    {
        LogAtualizandoAsset(_logger, request.Id, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

        if (asset == null) return null;

        if (!string.IsNullOrEmpty(request.Title))
            asset.Name = request.Title;

        if (request.Description != null)
            asset.Description = request.Description;

        if (request.AccessLevel.HasValue)
            asset.AccessLevel = request.AccessLevel.Value;

        if (request.TagIds != null)
            asset.Tags = string.Join(",", request.TagIds.Select(t => t.ToString()));

        asset.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return asset.ToMediaAssetDto();
    }
}

/// <summary>
/// Handler para deletar asset de mídia
/// </summary>
public partial class DeleteMediaAssetCommandHandler : IRequestHandler<DeleteMediaAssetCommand, bool>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<DeleteMediaAssetCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Deletando asset: {AssetId}")]
    private static partial void LogDeletandoAsset(ILogger logger, Guid assetId, Exception? exception);

    public DeleteMediaAssetCommandHandler(ISynQcoreDbContext context, ILogger<DeleteMediaAssetCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMediaAssetCommand request, CancellationToken cancellationToken)
    {
        LogDeletandoAsset(_logger, request.AssetId, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.AssetId, cancellationToken);

        if (asset == null) return false;

        asset.IsDeleted = true;
        asset.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

/// <summary>
/// Handler para upload em lote de assets
/// </summary>
public partial class BulkUploadMediaAssetsCommandHandler : IRequestHandler<BulkUploadMediaAssetsCommand, List<MediaAssetDto>>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<BulkUploadMediaAssetsCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Processando upload em lote: {Count} arquivos")]
    private static partial void LogUploadLote(ILogger logger, int count, Exception? exception);

    public BulkUploadMediaAssetsCommandHandler(ISynQcoreDbContext context, ILogger<BulkUploadMediaAssetsCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MediaAssetDto>> Handle(BulkUploadMediaAssetsCommand request, CancellationToken cancellationToken)
    {
        LogUploadLote(_logger, request.Assets.Count, null);

        var assets = new List<MediaAsset>();
        var defaultUserId = Guid.NewGuid(); // TODO: Obter do contexto de usuário

        foreach (var assetRequest in request.Assets)
        {
            var storageFileName = $"{Guid.NewGuid()}{Path.GetExtension(assetRequest.FileName)}";

            var asset = new MediaAsset
            {
                Id = Guid.NewGuid(),
                Name = assetRequest.Title,
                Description = assetRequest.Description,
                OriginalFileName = assetRequest.FileName,
                StorageFileName = storageFileName,
                FileSizeBytes = assetRequest.FileData.Length,
                ContentType = assetRequest.FileContentType,
                Type = Enum.Parse<MediaAssetType>(assetRequest.AssetType, true),
                Category = MediaAssetCategory.General,
                AccessLevel = request.DefaultAccessLevel,
                UploadedByEmployeeId = defaultUserId,
                IsApproved = false,
                DownloadCount = 0,
                Width = assetRequest.Width,
                Height = assetRequest.Height,
                DurationSeconds = assetRequest.Duration,
                Tags = string.Join(",", assetRequest.TagIds?.Select(t => t.ToString()) ?? []),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            assets.Add(asset);
        }

        _context.MediaAssets.AddRange(assets);
        await _context.SaveChangesAsync(cancellationToken);

        return assets.Select(a => a.ToMediaAssetDto()).ToList();
    }
}

/// <summary>
/// Handler para registrar acesso ao asset
/// </summary>
public partial class RegisterMediaAssetAccessCommandHandler : IRequestHandler<RegisterMediaAssetAccessCommand, bool>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<RegisterMediaAssetAccessCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Registrando acesso ao asset: {AssetId} por usuário: {UserId}")]
    private static partial void LogRegistrandoAcesso(ILogger logger, Guid assetId, Guid userId, Exception? exception);

    public RegisterMediaAssetAccessCommandHandler(ISynQcoreDbContext context, ILogger<RegisterMediaAssetAccessCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(RegisterMediaAssetAccessCommand request, CancellationToken cancellationToken)
    {
        LogRegistrandoAcesso(_logger, request.AssetId, request.UserId, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.AssetId, cancellationToken);

        if (asset == null) return false;

        // Incrementar contador de download
        asset.DownloadCount++;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

/// <summary>
/// Handler para gerar thumbnail
/// </summary>
public partial class GenerateThumbnailCommandHandler : IRequestHandler<GenerateThumbnailCommand, bool>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<GenerateThumbnailCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Gerando thumbnail para asset: {AssetId}")]
    private static partial void LogGerandoThumbnail(ILogger logger, Guid assetId, Exception? exception);

    public GenerateThumbnailCommandHandler(ISynQcoreDbContext context, ILogger<GenerateThumbnailCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(GenerateThumbnailCommand request, CancellationToken cancellationToken)
    {
        LogGerandoThumbnail(_logger, request.AssetId, null);

        var asset = await _context.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == request.AssetId, cancellationToken);

        if (asset == null) return false;

        // Simular geração de thumbnail
        // Em uma implementação real, aqui seria feita a geração do thumbnail
        asset.ThumbnailUrl = $"/thumbnails/{asset.Id}/{request.Width}x{request.Height}.jpg";
        asset.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
