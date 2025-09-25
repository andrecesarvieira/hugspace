using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Features.Employees.Commands;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;

namespace SynQcore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetEmployees([FromQuery] EmployeeSearchRequest request)
    {
        var result = await _mediator.Send(new GetEmployeesQuery(request));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "HR,Admin")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeRequest request)
    {
        var result = await _mediator.Send(new CreateEmployeeCommand(request));
        return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "HR,Admin")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(Guid id, UpdateEmployeeRequest request)
    {
        var result = await _mediator.Send(new UpdateEmployeeCommand(id, request));
        return Ok(result);
    }

    [HttpPost("{id:guid}/avatar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> UploadAvatar(Guid id, IFormFile avatar)
    {
        var avatarUrl = await _mediator.Send(new UploadEmployeeAvatarCommand(id, avatar));
        return Ok(new { avatarUrl });
    }

    [HttpGet("{id:guid}/hierarchy")]
    [ProducesResponseType(typeof(EmployeeHierarchyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeHierarchyDto>> GetEmployeeHierarchy(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeHierarchyQuery(id));
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EmployeeDto>>> SearchEmployees([FromQuery] string q)
    {
        var result = await _mediator.Send(new SearchEmployeesQuery(q));
        return Ok(result);
    }
}