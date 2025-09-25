using MediatR;
using Microsoft.AspNetCore.Identity;
using SynQcore.Application.Commands.Auth;
using SynQcore.Application.DTOs.Auth;
using SynQcore.Infrastructure.Identity;
using SynQcore.Infrastructure.Services.Auth;

namespace SynQcore.Api.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Verificar se email já existe
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return new AuthResponse
            {
                Success = false,
                Message = "Credenciais inválidas" // Invalid credentials
            };

        // Verificar se password está correto
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
            return new AuthResponse
            {
                Success = false,
                Message = "Credenciais inválidas" // Invalid credentials
            };

        // Gerar token JWT
        var token = _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "Login realizado com sucesso" // Login successful
        };
    }
}