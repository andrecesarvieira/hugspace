using FluentValidation;
using SynQcore.Application.Commands.Auth;

namespace SynQcore.Application.Validators.Auth;

/// <summary>
/// Validator para LoginCommand usando FluentValidation.
/// Define regras de validação para credenciais de login.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Inicializa as regras de validação para login.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters")
            .MaximumLength(100)
            .WithMessage("Password must not exceed 100 characters");
    }
}
