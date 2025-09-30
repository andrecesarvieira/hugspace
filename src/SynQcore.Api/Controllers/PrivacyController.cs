using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.Privacy.Queries;
using SynQcore.Application.Features.Privacy.Commands;
using SynQcore.Application.Features.Privacy.DTOs;
using SynQcore.Application.Common.DTOs;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de Privacy e Compliance LGPD
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class PrivacyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PrivacyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Consent Records

    /// <summary>
    /// Obter registros de consentimento com paginação
    /// </summary>
    /// <param name="query">Parâmetros de consulta</param>
    /// <returns>Lista paginada de registros de consentimento</returns>
    [HttpGet("consent-records")]
    [ProducesResponseType(typeof(PagedResult<ConsentRecordDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<ConsentRecordDto>>> GetConsentRecords([FromQuery] GetConsentRecordsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter registro de consentimento por ID
    /// </summary>
    /// <param name="id">ID do registro de consentimento</param>
    /// <returns>Registro de consentimento</returns>
    [HttpGet("consent-records/{id:guid}")]
    [ProducesResponseType(typeof(ConsentRecordDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ConsentRecordDto>> GetConsentRecord(Guid id)
    {
        var query = new GetConsentRecordByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"Registro de consentimento com ID {id} não encontrado.");

        return Ok(result);
    }

    /// <summary>
    /// Criar novo registro de consentimento
    /// </summary>
    /// <param name="command">Dados do registro de consentimento</param>
    /// <returns>Registro de consentimento criado</returns>
    [HttpPost("consent-records")]
    [ProducesResponseType(typeof(ConsentRecordDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ConsentRecordDto>> CreateConsentRecord([FromBody] CreateConsentRecordCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetConsentRecord), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualizar registro de consentimento
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="command">Dados para atualização</param>
    /// <returns>Registro atualizado</returns>
    [HttpPut("consent-records/{id:guid}")]
    [ProducesResponseType(typeof(ConsentRecordDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ConsentRecordDto>> UpdateConsentRecord(Guid id, [FromBody] UpdateConsentRecordCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound($"Registro de consentimento com ID {id} não encontrado.");

        return Ok(result);
    }

    #endregion

    #region Personal Data Categories

    /// <summary>
    /// Obter categorias de dados pessoais
    /// </summary>
    /// <param name="query">Parâmetros de consulta</param>
    /// <returns>Lista paginada de categorias</returns>
    [HttpGet("data-categories")]
    [ProducesResponseType(typeof(PagedResult<PersonalDataCategoryDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<PersonalDataCategoryDto>>> GetPersonalDataCategories([FromQuery] GetPersonalDataCategoriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter categoria de dados pessoais por ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Categoria de dados pessoais</returns>
    [HttpGet("data-categories/{id:guid}")]
    [ProducesResponseType(typeof(PersonalDataCategoryDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PersonalDataCategoryDto>> GetPersonalDataCategory(Guid id)
    {
        var query = new GetPersonalDataCategoryByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"Categoria com ID {id} não encontrada.");

        return Ok(result);
    }

    /// <summary>
    /// Criar nova categoria de dados pessoais
    /// </summary>
    /// <param name="command">Dados da categoria</param>
    /// <returns>Categoria criada</returns>
    [HttpPost("data-categories")]
    [ProducesResponseType(typeof(PersonalDataCategoryDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PersonalDataCategoryDto>> CreatePersonalDataCategory([FromBody] CreatePersonalDataCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPersonalDataCategory), new { id = result.Id }, result);
    }

    #endregion

    #region Data Export Requests

    /// <summary>
    /// Criar solicitação de exportação de dados
    /// </summary>
    /// <param name="command">Dados da solicitação</param>
    /// <returns>Solicitação criada</returns>
    [HttpPost("data-export-requests")]
    [ProducesResponseType(typeof(DataExportRequestDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<DataExportRequestDto>> CreateDataExportRequest([FromBody] CreateDataExportRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction("GetDataExportRequest", new { id = result.Id }, result);
    }

    #endregion

    #region Data Deletion Requests

    /// <summary>
    /// Criar solicitação de exclusão de dados
    /// </summary>
    /// <param name="command">Dados da solicitação</param>
    /// <returns>Solicitação criada</returns>
    [HttpPost("data-deletion-requests")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<DataDeletionRequestDto>> CreateDataDeletionRequest([FromBody] CreateDataDeletionRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction("GetDataDeletionRequest", new { id = result.Id }, result);
    }

    #endregion

    #region Health & Status

    /// <summary>
    /// Verificar status do sistema de privacy
    /// </summary>
    /// <returns>Status do sistema</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), 200)]
    [AllowAnonymous]
    public ActionResult<object> GetHealthStatus()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "Privacy & LGPD Compliance",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }

    #endregion
}
