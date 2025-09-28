using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.CorporateDocuments.Commands;
using SynQcore.Application.Features.CorporateDocuments.Queries;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Application.Common.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de documentos corporativos
/// Fornece endpoints para upload, organização e controle de acesso a documentos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
public class CorporateDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CorporateDocumentsController> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, Exception?> LogProcessingDocumentRequest =
        LoggerMessage.Define(LogLevel.Information, new EventId(5001, nameof(LogProcessingDocumentRequest)),
            "Processando requisição de documentos corporativos");

    private static readonly Action<ILogger, int, Exception?> LogDocumentSearchCompleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5002, nameof(LogDocumentSearchCompleted)),
            "Busca de documentos realizada: {Count} resultados");

    private static readonly Action<ILogger, Guid, Exception?> LogDocumentCreated =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5003, nameof(LogDocumentCreated)),
            "Documento criado com sucesso: {DocumentId}");

    private static readonly Action<ILogger, Guid, Exception?> LogDocumentDeleted =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5004, nameof(LogDocumentDeleted)),
            "Documento excluído com sucesso: {DocumentId}");

    private static readonly Action<ILogger, Exception?> LogDocumentError =
        LoggerMessage.Define(LogLevel.Error, new EventId(5099, nameof(LogDocumentError)),
            "Erro no processamento de documento");

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public CorporateDocumentsController(IMediator mediator, ILogger<CorporateDocumentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Busca documentos corporativos com filtros avançados
    /// </summary>
    /// <param name="request">Critérios de busca e paginação</param>
    /// <returns>Lista paginada de documentos</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<CorporateDocumentDto>>> GetDocuments([FromQuery] GetDocumentsRequest request)
    {
        LogProcessingDocumentRequest(_logger, null);

        try
        {
            var query = new GetDocumentsQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                SearchTerm = request.Title, // Mapear Title para SearchTerm
                Category = request.Category,
                Status = request.Status,
                AccessLevel = request.AccessLevel,
                CreatedAfter = request.CreatedAfter,
                CreatedBefore = request.CreatedBefore,
                DepartmentId = request.DepartmentId,
                AuthorId = request.AuthorId,
                TagIds = request.TagIds,
                SortBy = request.SortBy,
                SortDirection = request.SortOrder // Mapear SortOrder para SortDirection
            };

            var result = await _mediator.Send(query);

            LogDocumentSearchCompleted(_logger, result.Items.Count, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar documentos.");
        }
    }

    /// <summary>
    /// Obtém documento específico por ID
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <returns>Dados detalhados do documento</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CorporateDocumentDetailDto>> GetDocumentById([Required] Guid id)
    {
        try
        {
            var query = new GetDocumentByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar documento.");
        }
    }

    /// <summary>
    /// Cria novo documento corporativo
    /// </summary>
    /// <param name="request">Dados do documento</param>
    /// <returns>Documento criado</returns>
    [HttpPost]
    public async Task<ActionResult<CorporateDocumentDto>> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        try
        {
            var command = new CreateDocumentCommand
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                AccessLevel = request.AccessLevel,
                RequiresApproval = request.RequiresApproval,
                DepartmentId = request.DepartmentId,
                TagIds = request.TagIds,
                FileData = request.FileData,
                FileName = request.FileName,
                FileContentType = request.FileContentType
            };

            var result = await _mediator.Send(command);

            LogDocumentCreated(_logger, result.Id, null);
            return CreatedAtAction(nameof(GetDocumentById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao criar documento.");
        }
    }

    /// <summary>
    /// Atualiza documento existente
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Documento atualizado</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CorporateDocumentDto>> UpdateDocument([Required] Guid id, [FromBody] UpdateDocumentRequest request)
    {
        try
        {
            var command = new UpdateDocumentCommand
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                AccessLevel = request.AccessLevel,
                RequiresApproval = request.RequiresApproval,
                TagIds = request.TagIds
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao atualizar documento.");
        }
    }

    /// <summary>
    /// Exclui documento corporativo
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult> DeleteDocument([Required] Guid id)
    {
        try
        {
            var command = new DeleteDocumentCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            LogDocumentDeleted(_logger, id, null);
            return NoContent();
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao excluir documento.");
        }
    }

    /// <summary>
    /// Faz download do arquivo do documento
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <returns>Arquivo para download</returns>
    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult> DownloadDocument([Required] Guid id)
    {
        try
        {
            var query = new GetDocumentFileQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Arquivo do documento com ID '{id}' não foi encontrado.");
            }

            return File(result.FileData, result.ContentType, result.FileName);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao fazer download do documento.");
        }
    }

    /// <summary>
    /// Upload de nova versão do documento
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <param name="request">Dados da nova versão</param>
    /// <returns>Documento atualizado</returns>
    [HttpPost("{id:guid}/version")]
    public async Task<ActionResult<CorporateDocumentDto>> UploadNewVersion([Required] Guid id, [FromBody] UploadVersionRequest request)
    {
        try
        {
            var command = new UploadDocumentVersionCommand
            {
                DocumentId = id,
                VersionNotes = request.VersionNotes,
                FileData = request.FileData,
                FileName = request.FileName,
                FileContentType = request.FileContentType
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao fazer upload da nova versão.");
        }
    }

    /// <summary>
    /// Aprova documento pendente
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <param name="request">Dados da aprovação</param>
    /// <returns>Documento aprovado</returns>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<CorporateDocumentDto>> ApproveDocument([Required] Guid id, [FromBody] ApproveDocumentRequest request)
    {
        try
        {
            var command = new ApproveDocumentCommand
            {
                DocumentId = id,
                ApprovalNotes = request.ApprovalNotes
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao aprovar documento.");
        }
    }

    /// <summary>
    /// Rejeita documento pendente
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <param name="request">Dados da rejeição</param>
    /// <returns>Documento rejeitado</returns>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<CorporateDocumentDto>> RejectDocument([Required] Guid id, [FromBody] RejectDocumentRequest request)
    {
        try
        {
            var command = new RejectDocumentCommand
            {
                DocumentId = id,
                RejectionReason = request.RejectionReason
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao rejeitar documento.");
        }
    }

    /// <summary>
    /// Obtém histórico de acessos ao documento
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de acessos</returns>
    [HttpGet("{id:guid}/access-history")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<PagedResult<DocumentAccessDto>>> GetDocumentAccessHistory(
        [Required] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetDocumentAccessHistoryQuery
            {
                DocumentId = id,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar histórico de acessos.");
        }
    }

    /// <summary>
    /// Obtém estatísticas do documento
    /// </summary>
    /// <param name="id">ID do documento</param>
    /// <returns>Estatísticas detalhadas</returns>
    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<DocumentStatsDto>> GetDocumentStats([Required] Guid id)
    {
        try
        {
            var query = new GetDocumentStatsQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Documento com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDocumentError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar estatísticas do documento.");
        }
    }
}
