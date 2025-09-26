using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Commands.Auth;
using SynQcore.Application.DTOs.Auth;

namespace SynQcore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Registrar novo funcionário no sistema corporativo
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

    // Autenticar funcionário e gerar token JWT
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