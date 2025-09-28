using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.DTOs.Auth;
using SynQcore.Application.DTOs.Admin;
using SynQcore.Application.Commands.Admin;
using SynQcore.Application.Queries.Admin;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para funcionalidades administrativas do sistema corporativo
/// Fornece endpoints para gerenciamento de usuários e configurações (apenas Admin)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Listar todos os usuários do sistema com paginação e busca
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 10)</param>
    /// <param name="searchTerm">Termo para busca por nome ou email</param>
    /// <returns>Lista paginada de usuários do sistema</returns>
    [HttpGet("users")]
    public async Task<ActionResult<UsersListResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        // Validar e ajustar parâmetros de paginação
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Limitar tamanho máximo da página

        var query = new GetAllUsersQuery(page, pageSize, searchTerm);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Criar novo usuário com papel específico (apenas administradores)
    /// </summary>
    /// <param name="request">Dados do novo usuário incluindo papel</param>
    /// <returns>Usuário criado com confirmação</returns>
    [HttpPost("users")]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.Role);

        var response = await _mediator.Send(command);

        if (!response.Success)
            return BadRequest(new { message = response.Message });

        return CreatedAtAction(
            nameof(GetUsers), 
            new { searchTerm = response.Email }, 
            response);
    }

    /// <summary>
    /// Listar papéis disponíveis no sistema
    /// </summary>
    /// <returns>Lista de papéis corporativos disponíveis</returns>
    [HttpGet("roles")]
    /// <summary>
    /// Método para operação do sistema
    /// </summary>
    public ActionResult<IEnumerable<string>> GetAvailableRoles()
    {
        var roles = new[] { "Employee", "Manager", "HR", "Admin" };
        return Ok(roles);
    }
}