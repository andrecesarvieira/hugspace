using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.Commands;
using SynQcore.Application.Features.KnowledgeManagement.Queries;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;

namespace SynQcore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class KnowledgeCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public KnowledgeCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<KnowledgeCategoryDto>>> GetCategories(
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool includeHierarchy = true)
    {
        var query = new GetKnowledgeCategoriesQuery
        {
            IncludeInactive = includeInactive,
            IncludeHierarchy = includeHierarchy
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KnowledgeCategoryDto>> GetCategory(Guid id)
    {
        var query = new GetKnowledgeCategoryByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<KnowledgeCategoryDto>> CreateCategory(CreateKnowledgeCategoryDto createDto)
    {
        var command = new CreateKnowledgeCategoryCommand { Data = createDto };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<KnowledgeCategoryDto>> UpdateCategory(Guid id, UpdateKnowledgeCategoryDto updateDto)
    {
        var command = new UpdateKnowledgeCategoryCommand { Id = id, Data = updateDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var command = new DeleteKnowledgeCategoryCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}