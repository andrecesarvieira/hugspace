using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Commands.Auth;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para autenticação corporativa
/// Gerencia registro e login de funcionários
/// </summary>
[ApiController]
[Route("api/[controller]")]
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registrar novo funcionário no sistema corporativo
    /// </summary>
    /// <param name="request">Dados de registro do funcionário</param>
    /// <returns>Resposta com token JWT se sucesso</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.PhoneNumber);

        var response = await _mediator.Send(command);

        if (!response.Success)
            return BadRequest(new { message = response.Message });

        return Ok(response);
    }

    /// <summary>
    /// Autenticar funcionário e gerar token JWT
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Resposta com token JWT se autenticação válida</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        
        var response = await _mediator.Send(command);

        if (!response.Success)
            return Unauthorized(new { message = response.Message });

        return Ok(response);
    }
}