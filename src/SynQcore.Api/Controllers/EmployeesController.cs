using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Employees.Commands;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Application.Features.Employees.Queries;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de funcionários
/// Fornece endpoints para consulta, criação e manutenção de perfis de funcionários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public partial class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly ILogger<EmployeesController> _logger;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public EmployeesController(IMediator mediator, UserManager<ApplicationUserEntity> userManager, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Buscar funcionários com filtros e paginação
    /// </summary>
    /// <param name="request">Parâmetros de busca e filtros</param>
    /// <returns>Lista paginada de funcionários</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetEmployees([FromQuery] EmployeeSearchRequest request)
    {
        var result = await _mediator.Send(new GetEmployeesQuery(request));
        return Ok(result);
    }

    /// <summary>
    /// Obter funcionário específico por ID
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <returns>Dados completos do funcionário</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Criar novo funcionário (apenas HR/Admin)
    /// </summary>
    /// <param name="request">Dados para criação do funcionário</param>
    /// <returns>Funcionário criado com ID gerado</returns>
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

    /// <summary>
    /// Atualizar dados do funcionário (apenas HR/Admin)
    /// </summary>
    /// <param name="id">ID do funcionário a atualizar</param>
    /// <param name="request">Novos dados do funcionário</param>
    /// <returns>Funcionário atualizado</returns>
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

    /// <summary>
    /// Faz upload de avatar do funcionário com validações de segurança
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <param name="avatar">Arquivo de imagem do avatar</param>
    /// <returns>URL do avatar atualizado</returns>
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

    /// <summary>
    /// Obtém hierarquia organizacional do funcionário (subordinados e gerente)
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <returns>Hierarquia organizacional do funcionário</returns>
    [HttpGet("{id:guid}/hierarchy")]
    [ProducesResponseType(typeof(EmployeeHierarchyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeHierarchyDto>> GetEmployeeHierarchy(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeHierarchyQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Busca funcionários por nome, email ou departamento
    /// </summary>
    /// <param name="q">Termo de busca</param>
    /// <returns>Lista de funcionários encontrados</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<EmployeeDto>>> SearchEmployees([FromQuery] string q)
    {
        var result = await _mediator.Send(new SearchEmployeesQuery(q));
        return Ok(result);
    }

    /// <summary>
    /// Desliga funcionário (soft delete + bloqueio de acesso) - apenas HR/Admin
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <returns>Confirmação do desligamento</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "HR,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> TerminateEmployee(
        [FromRoute] Guid id)
    {
        try
        {
            LogProcessingTermination(_logger, id);

            // 1. Executar soft delete do Employee (lógica de negócio existente)
            await _mediator.Send(new DeleteEmployeeCommand(id));

            // 2. Buscar e bloquear usuário associado
            await BlockAssociatedUser(id);

            LogEmployeeTerminated(_logger, id);
            return NoContent();
        }
        catch (Exception ex)
        {
            LogEmployeeTerminationError(_logger, id, ex);
            throw;
        }
    }

    // Bloquear usuário associado ao Employee desligado
    private async Task BlockAssociatedUser(Guid employeeId)
    {
        LogSearchingUser(_logger, employeeId);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);

        if (user != null)
        {
            LogBlockingUser(_logger, user.Id, user.Email ?? "N/A");

            // Bloquear acesso indefinidamente
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            // Forçar logout de todas as sessões ativas
            await _userManager.UpdateSecurityStampAsync(user);

            LogUserBlocked(_logger, user.Id);
        }
        else
        {
            LogUserNotFound(_logger, employeeId);
        }
    }

    // LoggerMessage delegates para performance otimizada
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Iniciando processo de desligamento do funcionário: {EmployeeId}")]
    private static partial void LogProcessingTermination(ILogger logger, Guid employeeId);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Funcionário {EmployeeId} desligado com sucesso")]
    private static partial void LogEmployeeTerminated(ILogger logger, Guid employeeId);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Error,
        Message = "Erro durante desligamento do funcionário {EmployeeId}")]
    private static partial void LogEmployeeTerminationError(ILogger logger, Guid employeeId, Exception exception);

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Information,
        Message = "Buscando usuário associado ao funcionário: {EmployeeId}")]
    private static partial void LogSearchingUser(ILogger logger, Guid employeeId);

    [LoggerMessage(
        EventId = 2005,
        Level = LogLevel.Information,
        Message = "Bloqueando acesso do usuário: {UserId} - {Email}")]
    private static partial void LogBlockingUser(ILogger logger, Guid userId, string email);

    [LoggerMessage(
        EventId = 2006,
        Level = LogLevel.Information,
        Message = "Usuário {UserId} bloqueado com sucesso")]
    private static partial void LogUserBlocked(ILogger logger, Guid userId);

    [LoggerMessage(
        EventId = 2007,
        Level = LogLevel.Warning,
        Message = "Nenhum usuário encontrado associado ao funcionário: {EmployeeId}")]
    private static partial void LogUserNotFound(ILogger logger, Guid employeeId);
}
