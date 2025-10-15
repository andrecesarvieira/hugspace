using MediatR;
using Microsoft.AspNetCore.Identity;
using SynQcore.Application.Commands.Auth;
using SynQcore.Application.DTOs.Auth;
using SynQcore.Infrastructure.EventHandlers;
using SynQcore.Infrastructure.Identity;
using SynQcore.Infrastructure.Services.Auth;

namespace SynQcore.Api.Handlers.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly IJwtService _jwtService;
    private readonly UserRegistrationService _userRegistrationService;

    public RegisterCommandHandler(
        UserManager<ApplicationUserEntity> userManager,
        IJwtService jwtService,
        UserRegistrationService userRegistrationService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _userRegistrationService = userRegistrationService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Verificar se email já existe
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return new AuthResponse { Success = false, Message = "Email already exists" };

        // Criar novo usuário
        var user = new ApplicationUserEntity
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true // Para desenvolvimento
        };

        // Tentar criar o usuário
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse { Success = false, Message = $"Registration failed: {errors}" };
        }

        // Adicionar role padrão "Employee" para novos usuários
        await _userManager.AddToRoleAsync(user, "Employee");

        // Criar registro do funcionário automaticamente
        await _userRegistrationService.HandleUserRegisteredAsync(user);

        // Gerar token JWT
        var token = _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "Registration successful"
        };
    }
}
