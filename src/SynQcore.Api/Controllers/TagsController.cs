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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid id)
    {
        var query = new GetTagByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createDto)
    {
        var command = new CreateTagCommand { Data = createDto };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTag), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TagDto>> UpdateTag(Guid id, UpdateTagDto updateDto)
    {
        var command = new UpdateTagCommand { Id = id, Data = updateDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTag(Guid id)
    {
        var command = new DeleteTagCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}