using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Departments.Commands;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Application.Features.Departments.Queries;
using DepartmentDto = SynQcore.Application.Features.Departments.DTOs.DepartmentDto;

namespace SynQcore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DepartmentsController> _logger;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public DepartmentsController(IMediator mediator, ILogger<DepartmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Busca departamentos com filtros e paginação
    /// </summary>
    /// <param name="request">Critérios de busca e paginação</param>
    /// <returns>Lista paginada de departamentos</returns>
    [HttpGet]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    public async Task<ActionResult<PagedResult<DepartmentDto>>> GetDepartments([FromQuery] GetDepartmentsRequest request)
    {
        LogDepartmentRequest();

        try
        {
            var query = new GetDepartmentsQuery(request);
            var result = await _mediator.Send(query);

            LogDepartmentSuccess(result.TotalCount);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDepartmentError(ex);
            return StatusCode(500, "An error occurred while retrieving departments.");
        }
    }

    /// <summary>
    /// Obtém departamento específico por ID
    /// </summary>
    /// <param name="id">ID do departamento</param>
    /// <returns>Dados do departamento</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Employee,Manager,HR,Admin")]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentById([Required] Guid id)
    {
        LogDepartmentByIdRequest(id);

        try
        {
            var query = new GetDepartmentByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                LogDepartmentNotFound(id);
                return NotFound($"Department with ID '{id}' not found.");
            }

            LogDepartmentByIdSuccess(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LogDepartmentByIdError(id, ex);
            return StatusCode(500, "An error occurred while retrieving the department.");
        }
    }

    /// <summary>
    /// Obtém hierarquia completa do departamento (subordinados e departamento pai)
    /// </summary>
    /// <param name="id">ID do departamento</param>
    /// <returns>Hierarquia completa do departamento</returns>
    [HttpGet("{id:guid}/hierarchy")]
    [Authorize(Roles = "Manager,HR,Admin")]
    public async Task<ActionResult<DepartmentHierarchyDto>> GetDepartmentHierarchy([Required] Guid id)
    {
        LogDepartmentHierarchyRequest(id);

        try
        {
            var query = new GetDepartmentHierarchyQuery(id);
            var result = await _mediator.Send(query);

            LogDepartmentHierarchySuccess(id);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            LogDepartmentNotFound(id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            LogDepartmentHierarchyError(id, ex);
            return StatusCode(500, "An error occurred while retrieving the department hierarchy.");
        }
    }

    /// <summary>
    /// Cria novo departamento com validações de hierarquia (apenas HR/Admin)
    /// </summary>
    /// <param name="request">Dados do departamento a ser criado</param>
    /// <returns>Dados do departamento criado</returns>
    [HttpPost]
    [Authorize(Roles = "HR,Admin")]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody, Required] CreateDepartmentRequest request)
    {
        LogCreateDepartmentRequest(request.Name, request.Code);

        try
        {
            var command = new CreateDepartmentCommand(request);
            var result = await _mediator.Send(command);

            LogCreateDepartmentSuccess(result.Id, result.Name);
            return CreatedAtAction(nameof(GetDepartmentById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            LogCreateDepartmentConflict(request.Code, ex);
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            LogCreateDepartmentValidation(ex);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogCreateDepartmentError(request.Name, ex);
            return StatusCode(500, "An error occurred while creating the department.");
        }
    }

    /// <summary>
    /// Atualiza departamento existente com validações de negócio (apenas HR/Admin)
    /// </summary>
    /// <param name="id">ID do departamento</param>
    /// <param name="request">Dados de atualização do departamento</param>
    /// <returns>Dados do departamento atualizado</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "HR,Admin")]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment([Required] Guid id, [FromBody, Required] UpdateDepartmentRequest request)
    {
        LogUpdateDepartmentRequest(id, request.Name, request.Code);

        try
        {
            var command = new UpdateDepartmentCommand(id, request);
            var result = await _mediator.Send(command);

            LogUpdateDepartmentSuccess(id, result.Name);
            return Ok(result);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("not found"))
        {
            LogDepartmentNotFound(id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            LogUpdateDepartmentConflict(id, ex);
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            LogUpdateDepartmentValidation(id, ex);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogUpdateDepartmentError(id, ex);
            return StatusCode(500, "An error occurred while updating the department.");
        }
    }

    /// <summary>
    /// Exclui departamento com validação de dependências (apenas Admin)
    /// </summary>
    /// <param name="id">ID do departamento a ser excluído</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteDepartment([Required] Guid id)
    {
        LogDeleteDepartmentRequest(id);

        try
        {
            var command = new DeleteDepartmentCommand(id);
            await _mediator.Send(command);

            LogDeleteDepartmentSuccess(id);
            return NoContent();
        }
        catch (ArgumentException ex) when (ex.Message.Contains("not found"))
        {
            LogDepartmentNotFound(id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            LogDeleteDepartmentConflict(id, ex);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            LogDeleteDepartmentError(id, ex);
            return StatusCode(500, "An error occurred while deleting the department.");
        }
    }

    #region Logging Methods

    [LoggerMessage(LogLevel.Information, "Processing request to get departments")]
    private partial void LogDepartmentRequest();

    [LoggerMessage(LogLevel.Information, "Successfully retrieved {Count} departments")]
    private partial void LogDepartmentSuccess(int count);

    [LoggerMessage(LogLevel.Error, "Error occurred while retrieving departments")]
    private partial void LogDepartmentError(Exception ex);

    [LoggerMessage(LogLevel.Information, "Processing request to get department by ID: {DepartmentId}")]
    private partial void LogDepartmentByIdRequest(Guid departmentId);

    [LoggerMessage(LogLevel.Information, "Successfully retrieved department: {DepartmentId}")]
    private partial void LogDepartmentByIdSuccess(Guid departmentId);

    [LoggerMessage(LogLevel.Warning, "Department not found: {DepartmentId}")]
    private partial void LogDepartmentNotFound(Guid departmentId);

    [LoggerMessage(LogLevel.Error, "Error occurred while retrieving department {DepartmentId}")]
    private partial void LogDepartmentByIdError(Guid departmentId, Exception ex);

    [LoggerMessage(LogLevel.Information, "Processing request to get department hierarchy: {DepartmentId}")]
    private partial void LogDepartmentHierarchyRequest(Guid departmentId);

    [LoggerMessage(LogLevel.Information, "Successfully retrieved department hierarchy: {DepartmentId}")]
    private partial void LogDepartmentHierarchySuccess(Guid departmentId);

    [LoggerMessage(LogLevel.Error, "Error occurred while retrieving department hierarchy {DepartmentId}")]
    private partial void LogDepartmentHierarchyError(Guid departmentId, Exception ex);

    [LoggerMessage(LogLevel.Information, "Processing request to create department: {Name} ({Code})")]
    private partial void LogCreateDepartmentRequest(string name, string code);

    [LoggerMessage(LogLevel.Information, "Successfully created department: {DepartmentId} - {Name}")]
    private partial void LogCreateDepartmentSuccess(Guid departmentId, string name);

    [LoggerMessage(LogLevel.Warning, "Conflict while creating department with code: {Code}")]
    private partial void LogCreateDepartmentConflict(string code, Exception ex);

    [LoggerMessage(LogLevel.Warning, "Validation error while creating department")]
    private partial void LogCreateDepartmentValidation(Exception ex);

    [LoggerMessage(LogLevel.Error, "Error occurred while creating department: {Name}")]
    private partial void LogCreateDepartmentError(string name, Exception ex);

    [LoggerMessage(LogLevel.Information, "Processing request to update department: {DepartmentId} - {Name} ({Code})")]
    private partial void LogUpdateDepartmentRequest(Guid departmentId, string name, string code);

    [LoggerMessage(LogLevel.Information, "Successfully updated department: {DepartmentId} - {Name}")]
    private partial void LogUpdateDepartmentSuccess(Guid departmentId, string name);

    [LoggerMessage(LogLevel.Warning, "Conflict while updating department: {DepartmentId}")]
    private partial void LogUpdateDepartmentConflict(Guid departmentId, Exception ex);

    [LoggerMessage(LogLevel.Warning, "Validation error while updating department: {DepartmentId}")]
    private partial void LogUpdateDepartmentValidation(Guid departmentId, Exception ex);

    [LoggerMessage(LogLevel.Error, "Error occurred while updating department: {DepartmentId}")]
    private partial void LogUpdateDepartmentError(Guid departmentId, Exception ex);

    [LoggerMessage(LogLevel.Information, "Processing request to delete department: {DepartmentId}")]
    private partial void LogDeleteDepartmentRequest(Guid departmentId);

    [LoggerMessage(LogLevel.Information, "Successfully deleted department: {DepartmentId}")]
    private partial void LogDeleteDepartmentSuccess(Guid departmentId);

    [LoggerMessage(LogLevel.Warning, "Conflict while deleting department: {DepartmentId}")]
    private partial void LogDeleteDepartmentConflict(Guid departmentId, Exception ex);

    [LoggerMessage(LogLevel.Error, "Error occurred while deleting department: {DepartmentId}")]
    private partial void LogDeleteDepartmentError(Guid departmentId, Exception ex);

    #endregion
}
