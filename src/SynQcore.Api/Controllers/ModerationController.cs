using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Application.Features.Moderation.Queries;
using SynQcore.Application.Features.Moderation.Commands;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para sistema de moderação corporativa
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ModerationController> _logger;

    public ModerationController(IMediator mediator, ILogger<ModerationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obter fila de moderação com filtros e paginação
    /// </summary>
    [HttpGet("queue")]
    [ProducesResponseType(typeof(PagedResult<ModerationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<ModerationDto>>> GetModerationQueue([FromQuery] GetModerationQueueQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter detalhes de moderação específica
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ModerationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModerationDto>> GetModerationById(Guid id)
    {
        var result = await _mediator.Send(new GetModerationByIdQuery { Id = id });

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Obter estatísticas de moderação
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ModerationStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModerationStatsDto>> GetModerationStats()
    {
        var result = await _mediator.Send(new GetModerationStatsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Obter categorias de moderação disponíveis
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<string>>> GetModerationCategories()
    {
        var result = await _mediator.Send(new GetModerationCategoriesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Obter severidades de moderação disponíveis
    /// </summary>
    [HttpGet("severities")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<string>>> GetModerationSeverities()
    {
        var result = await _mediator.Send(new GetModerationSeveritiesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Obter ações de moderação disponíveis
    /// </summary>
    [HttpGet("actions")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<string>>> GetModerationActions()
    {
        var result = await _mediator.Send(new GetModerationActionsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Processar uma moderação (aprovar, rejeitar, remover, escalar)
    /// </summary>
    [HttpPost("process")]
    [ProducesResponseType(typeof(ModerationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModerationDto>> ProcessModeration([FromBody] ProcessModerationCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar status de uma moderação existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ModerationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModerationDto>> UpdateModeration(Guid id, [FromBody] UpdateModerationCommand command)
    {
        if (id != command.ModerationId)
            return BadRequest("ID na URL não corresponde ao ID no comando");

        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Escalar uma moderação para nível superior
    /// </summary>
    [HttpPost("{id}/escalate")]
    [ProducesResponseType(typeof(ModerationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModerationDto>> EscalateModeration(Guid id, [FromBody] EscalateModerationCommand command)
    {
        if (id != command.ModerationId)
            return BadRequest("ID na URL não corresponde ao ID no comando");

        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Criar uma nova solicitação de moderação
    /// </summary>
    [HttpPost("report")]
    [ProducesResponseType(typeof(ModerationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModerationDto>> CreateModerationRequest([FromBody] CreateModerationRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetModerationById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Ações em lote para múltiplas moderações
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<ModerationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ModerationDto>>> BulkModeration([FromBody] BulkModerationCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Arquivar moderações antigas baseado em critérios
    /// </summary>
    [HttpPost("archive")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> ArchiveOldModerations([FromBody] ArchiveOldModerationsCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { archivedCount = result });
    }
}
