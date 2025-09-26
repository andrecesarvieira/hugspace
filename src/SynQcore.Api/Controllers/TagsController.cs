using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.Commands;
using SynQcore.Application.Features.KnowledgeManagement.Queries;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Buscar tags com filtros avançados e ordenação personalizada
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags(
        [FromQuery] TagType? type = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? minUsageCount = null,
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool sortDescending = false)
    {
        var query = new GetTagsQuery
        {
            Type = type,
            SearchTerm = searchTerm,
            MinUsageCount = minUsageCount,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Obter tags mais populares por número de usos
    [HttpGet("popular")]
    public async Task<ActionResult<List<TagDto>>> GetPopularTags(
        [FromQuery] int count = 20,
        [FromQuery] TagType? type = null)
    {
        var query = new GetPopularTagsQuery
        {
            Count = count,
            Type = type
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Obter tag específica por ID com informações de uso
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid id)
    {
        var query = new GetTagByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Criar nova tag (skill, knowledge, etc.)
    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createDto)
    {
        var command = new CreateTagCommand { Data = createDto };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTag), new { id = result.Id }, result);
    }

    // Atualizar informações da tag existente
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TagDto>> UpdateTag(Guid id, UpdateTagDto updateDto)
    {
        var command = new UpdateTagCommand { Id = id, Data = updateDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Excluir tag (soft delete) com validação de dependências
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTag(Guid id)
    {
        var command = new DeleteTagCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}