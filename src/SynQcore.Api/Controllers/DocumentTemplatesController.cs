using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.DocumentTemplates.Commands;
using SynQcore.Application.Features.DocumentTemplates.Queries;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Application.Common.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de templates de documentos corporativos
/// Fornece endpoints para criação e manutenção de modelos padronizados
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
public class DocumentTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DocumentTemplatesController> _logger;

    // LoggerMessage delegates para performance otimizada
    private static readonly Action<ILogger, Exception?> LogProcessingTemplateRequest =
        LoggerMessage.Define(LogLevel.Information, new EventId(5101, nameof(LogProcessingTemplateRequest)),
            "Processando requisição de templates de documento");

    private static readonly Action<ILogger, int, Exception?> LogTemplateSearchCompleted =
        LoggerMessage.Define<int>(LogLevel.Information, new EventId(5102, nameof(LogTemplateSearchCompleted)),
            "Busca de templates realizada: {Count} resultados");

    private static readonly Action<ILogger, Guid, Exception?> LogTemplateCreated =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5103, nameof(LogTemplateCreated)),
            "Template criado com sucesso: {TemplateId}");

    private static readonly Action<ILogger, Guid, Exception?> LogTemplateDeleted =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(5104, nameof(LogTemplateDeleted)),
            "Template excluído com sucesso: {TemplateId}");

    private static readonly Action<ILogger, Exception?> LogTemplateError =
        LoggerMessage.Define(LogLevel.Error, new EventId(5199, nameof(LogTemplateError)),
            "Erro no processamento de template");

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public DocumentTemplatesController(IMediator mediator, ILogger<DocumentTemplatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Busca templates de documentos com filtros
    /// </summary>
    /// <param name="request">Critérios de busca e paginação</param>
    /// <returns>Lista paginada de templates</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<DocumentTemplateDto>>> GetTemplates([FromQuery] GetTemplatesRequest request)
    {
        LogProcessingTemplateRequest(_logger, null);

        try
        {
            var query = new GetTemplatesQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Name = request.Name,
                Category = request.Category,
                IsActive = request.IsActive,
                DepartmentId = request.DepartmentId,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder
            };

            var result = await _mediator.Send(query);

            LogTemplateSearchCompleted(_logger, result.Items.Count, null);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar templates.");
        }
    }

    /// <summary>
    /// Obtém template específico por ID
    /// </summary>
    /// <param name="id">ID do template</param>
    /// <returns>Dados detalhados do template</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DocumentTemplateDetailDto>> GetTemplateById([Required] Guid id)
    {
        try
        {
            var query = new GetTemplateByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar template.");
        }
    }

    /// <summary>
    /// Cria novo template de documento
    /// </summary>
    /// <param name="request">Dados do template</param>
    /// <returns>Template criado</returns>
    [HttpPost]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<DocumentTemplateDto>> CreateTemplate([FromBody] CreateTemplateRequest request)
    {
        try
        {
            var command = new CreateTemplateCommand
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Content = request.Content,
                Fields = request.Fields,
                DefaultAccessLevel = request.DefaultAccessLevel,
                RequiresApproval = request.RequiresApproval,
                AllowedDepartmentIds = request.AllowedDepartmentIds,
                IsActive = request.IsActive
            };

            var result = await _mediator.Send(command);

            LogTemplateCreated(_logger, result.Id, null);
            return CreatedAtAction(nameof(GetTemplateById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao criar template.");
        }
    }

    /// <summary>
    /// Atualiza template existente
    /// </summary>
    /// <param name="id">ID do template</param>
    /// <param name="request">Dados atualizados</param>
    /// <returns>Template atualizado</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<DocumentTemplateDto>> UpdateTemplate([Required] Guid id, [FromBody] UpdateTemplateRequest request)
    {
        try
        {
            var command = new UpdateTemplateCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Content = request.Content,
                Fields = request.Fields,
                DefaultAccessLevel = request.DefaultAccessLevel,
                RequiresApproval = request.RequiresApproval,
                AllowedDepartmentIds = request.AllowedDepartmentIds,
                IsActive = request.IsActive
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao atualizar template.");
        }
    }

    /// <summary>
    /// Exclui template de documento
    /// </summary>
    /// <param name="id">ID do template</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult> DeleteTemplate([Required] Guid id)
    {
        try
        {
            var command = new DeleteTemplateCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            LogTemplateDeleted(_logger, id, null);
            return NoContent();
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao excluir template.");
        }
    }

    /// <summary>
    /// Cria documento a partir de template
    /// </summary>
    /// <param name="id">ID do template</param>
    /// <param name="request">Dados específicos do documento</param>
    /// <returns>Documento criado</returns>
    [HttpPost("{id:guid}/create-document")]
    public async Task<ActionResult<CreateDocumentFromTemplateDto>> CreateDocumentFromTemplate([Required] Guid id, [FromBody] CreateDocumentFromTemplateRequest request)
    {
        try
        {
            var command = new CreateDocumentFromTemplateCommand
            {
                TemplateId = id,
                Title = request.Title,
                FieldValues = request.FieldValues,
                AccessLevel = request.AccessLevel,
                DepartmentId = request.DepartmentId,
                TagIds = request.TagIds
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao criar documento a partir do template.");
        }
    }

    /// <summary>
    /// Duplica template existente
    /// </summary>
    /// <param name="id">ID do template original</param>
    /// <param name="request">Dados do novo template</param>
    /// <returns>Template duplicado</returns>
    [HttpPost("{id:guid}/duplicate")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<DocumentTemplateDto>> DuplicateTemplate([Required] Guid id, [FromBody] DuplicateTemplateRequest request)
    {
        try
        {
            var command = new DuplicateTemplateCommand
            {
                SourceTemplateId = id,
                NewName = request.NewName,
                NewDescription = request.NewDescription
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            return CreatedAtAction(nameof(GetTemplateById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao duplicar template.");
        }
    }

    /// <summary>
    /// Obtém estatísticas de uso do template
    /// </summary>
    /// <param name="id">ID do template</param>
    /// <returns>Estatísticas de uso</returns>
    [HttpGet("{id:guid}/usage-stats")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<TemplateUsageStatsDto>> GetTemplateUsageStats([Required] Guid id)
    {
        try
        {
            var query = new GetTemplateUsageStatsQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao buscar estatísticas do template.");
        }
    }

    /// <summary>
    /// Ativa ou desativa template
    /// </summary>
    /// <param name="id">ID do template</param>
    /// <param name="request">Estado do template</param>
    /// <returns>Template atualizado</returns>
    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<DocumentTemplateDto>> ToggleTemplateStatus([Required] Guid id, [FromBody] ToggleTemplateStatusRequest request)
    {
        try
        {
            var command = new ToggleTemplateStatusCommand
            {
                TemplateId = id,
                IsActive = request.IsActive
            };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"Template com ID '{id}' não foi encontrado.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            LogTemplateError(_logger, ex);
            return StatusCode(500, "Erro interno do servidor ao alterar status do template.");
        }
    }
}
