using MediatR;
using Microsoft.AspNetCore.Identity;
using SynQcore.Application.Commands.Auth;
using SynQcore.Application.DTOs.Auth;
using SynQcore.Infrastructure.Services.Auth;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Infrastructure.Commands.Auth;

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
    public Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}