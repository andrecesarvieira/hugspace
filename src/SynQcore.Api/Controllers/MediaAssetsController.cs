using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.MediaAssets.Commands;
using SynQcore.Application.Features.MediaAssets.Queries;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Application.Common.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de assets de mídia corporativa
/// Fornece endpoints para upload, organização e distribuição de mídias
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class MediaAssetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MediaAssetsController> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, Exception?> LogProcessingMediaRequest =
        LoggerMessage.Define(LogLevel.Information, new EventId(5201, nameof(LogProcessingMediaRequest)),
            "Processando requisição de assets de mídia");

    private static readonly Action<ILogger, int, Exception?> LogMediaSearchCompleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5202, nameof(LogMediaSearchCompleted)),
            "Busca de assets realizada: {Count} resultados");

    private static readonly Action<ILogger, Guid, Exception?> LogMediaCreated =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5203, nameof(LogMediaCreated)),
            "Asset de mídia criado com sucesso: {AssetId}");

    private static readonly Action<ILogger, Guid, Exception?> LogMediaDeleted =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5204, nameof(LogMediaDeleted)),
            "Asset de mídia excluído com sucesso: {AssetId}");

    private static readonly Action<ILogger, Exception?> LogMediaError =
        LoggerMessage.Define(LogLevel.Error, new EventId(5299, nameof(LogMediaError)),
            "Erro no processamento de asset de mídia");

    public MediaAssetsController(IMediator mediator, ILogger<MediaAssetsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Busca assets de mídia com filtros avançados
    /// </summary>
    /// <param name="request">Critérios de busca e paginação</param>
    /// <returns>Lista paginada de assets</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<MediaAssetDto>>> GetMediaAssets([FromQuery] GetMediaAssetsRequest request)
    {
        LogProcessingMediaRequest(_logger, null);

        try
        {
            var query = new GetMediaAssetsQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Title = request.Title,
                AssetType = request.AssetType,
                AccessLevel = request.AccessLevel,
                CreatedAfter = request.CreatedAfter,
                CreatedBefore = request.CreatedBefore,
                DepartmentId = request.DepartmentId,
                AuthorId = request.AuthorId,
                TagIds = request.TagIds,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder
            };

            var result = await _mediator.Send(query);

            LogMediaSearchCompleted(_logger, result.Items.Count, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar assets de mídia.");
        }
    }

    /// <summary>
    /// Obtém asset específico por ID
    /// </summary>
    /// <param name="id">ID do asset</param>
    /// <returns>Dados detalhados do asset</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MediaAssetDetailDto>> GetMediaAssetById([Required] Guid id)
    {
        try
        {
            var query = new GetMediaAssetByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Asset de mídia com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar asset de mídia.");
        }
    }

    /// <summary>
    /// Faz upload de novo asset de mídia
    /// </summary>
    /// <param name="request">Dados do asset</param>
    /// <returns>Asset criado</returns>
    [HttpPost]
    public async Task<ActionResult<MediaAssetDto>> UploadMediaAsset([FromBody] UploadMediaAssetRequest request)
    {
        try
        {
            var command = new UploadMediaAssetCommand
            {
                Title = request.Title,
                Description = request.Description,
                AssetType = request.AssetType,
                AccessLevel = request.AccessLevel,
                DepartmentId = request.DepartmentId,
                TagIds = request.TagIds,
                FileData = request.FileData,
                FileName = request.FileName,
                FileContentType = request.FileContentType,
                Width = request.Width,
                Height = request.Height,
                Duration = request.Duration
            };

            var result = await _mediator.Send(command);

            LogMediaCreated(_logger, result.Id, null);
            return CreatedAtAction(nameof(GetMediaAssetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao fazer upload do asset.");
        }
    }

    /// <summary>
    /// Atualiza asset existente
    /// </summary>
    /// <param name="id">ID do asset</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Asset atualizado</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MediaAssetDto>> UpdateMediaAsset([Required] Guid id, [FromBody] UpdateMediaAssetRequest request)
    {
        try
        {
            var command = new UpdateMediaAssetCommand
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                AccessLevel = request.AccessLevel,
                TagIds = request.TagIds
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Asset de mídia com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao atualizar asset.");
        }
    }

    /// <summary>
    /// Exclui asset de mídia
    /// </summary>
    /// <param name="id">ID do asset</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult> DeleteMediaAsset([Required] Guid id)
    {
        try
        {
            var command = new DeleteMediaAssetCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound($"Asset de mídia com ID '{id}' não foi encontrado.");
            }

            LogMediaDeleted(_logger, id, null);
            return NoContent();
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao excluir asset.");
        }
    }

    /// <summary>
    /// Faz download do arquivo do asset
    /// </summary>
    /// <param name="id">ID do asset</param>
    /// <returns>Arquivo para download</returns>
    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult> DownloadMediaAsset([Required] Guid id)
    {
        try
        {
            var query = new GetMediaAssetFileQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Arquivo do asset com ID '{id}' não foi encontrado.");
            }

            return File(result.FileData, result.ContentType, result.FileName);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao fazer download do asset.");
        }
    }

    /// <summary>
    /// Obtém thumbnail do asset (quando aplicável)
    /// </summary>
    /// <param name="id">ID do asset</param>
    /// <param name="size">Tamanho do thumbnail (small, medium, large)</param>
    /// <returns>Thumbnail</returns>
    [HttpGet("{id:guid}/thumbnail")]
    public async Task<ActionResult> GetThumbnail([Required] Guid id, [FromQuery] string size = "medium")
    {
        try
        {
            var query = new GetMediaAssetThumbnailQuery(id, size);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Thumbnail do asset com ID '{id}' não foi encontrado.");
            }

            return File(result.FileData, result.ContentType);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar thumbnail.");
        }
    }

    /// <summary>
    /// Obtém assets de mídia por categoria
    /// </summary>
    /// <param name="assetType">Tipo de asset</param>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de assets</returns>
    [HttpGet("by-type/{assetType}")]
    public async Task<ActionResult<PagedResult<MediaAssetDto>>> GetMediaAssetsByType(
        [Required] string assetType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetMediaAssetsByTypeQuery
            {
                AssetType = assetType,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar assets por tipo.");
        }
    }

    /// <summary>
    /// Obtém galeria de imagens do departamento
    /// </summary>
    /// <param name="departmentId">ID do departamento</param>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de imagens</returns>
    [HttpGet("gallery/department/{departmentId:guid}")]
    public async Task<ActionResult<PagedResult<MediaAssetDto>>> GetDepartmentGallery(
        [Required] Guid departmentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetDepartmentGalleryQuery
            {
                DepartmentId = departmentId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar galeria do departamento.");
        }
    }

    /// <summary>
    /// Obtém estatísticas de uso do asset
    /// </summary>
    /// <param name="id">ID do asset</param>
    /// <returns>Estatísticas detalhadas</returns>
    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<MediaAssetStatsDto>> GetMediaAssetStats([Required] Guid id)
    {
        try
        {
            var query = new GetMediaAssetStatsQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Asset de mídia com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar estatísticas do asset.");
        }
    }

    /// <summary>
    /// Upload múltiplo de assets
    /// </summary>
    /// <param name="request">Lista de assets para upload</param>
    /// <returns>Lista de assets criados</returns>
    [HttpPost("bulk-upload")]
    public async Task<ActionResult<List<MediaAssetDto>>> BulkUploadMediaAssets([FromBody] BulkUploadMediaAssetsRequest request)
    {
        try
        {
            var command = new BulkUploadMediaAssetsCommand
            {
                Assets = request.Assets,
                DepartmentId = request.DepartmentId,
                DefaultAccessLevel = request.DefaultAccessLevel,
                DefaultTagIds = request.DefaultTagIds
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao fazer upload em lote.");
        }
    }

    /// <summary>
    /// Obtém assets mais populares
    /// </summary>
    /// <param name="period">Período (week, month, year)</param>
    /// <param name="limit">Número máximo de resultados</param>
    /// <returns>Lista de assets populares</returns>
    [HttpGet("popular")]
    public async Task<ActionResult<List<MediaAssetDto>>> GetPopularAssets(
        [FromQuery] string period = "month",
        [FromQuery] int limit = 10)
    {
        try
        {
            var query = new GetPopularAssetsQuery
            {
                Period = period,
                Limit = limit
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogMediaError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar assets populares.");
        }
    }
}
