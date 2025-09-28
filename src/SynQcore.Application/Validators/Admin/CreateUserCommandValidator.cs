using FluentValidation;
using SynQcore.Application.Commands.Admin;

namespace SynQcore.Application.Validators.Admin;

/// <summary>
/// Validator para CreateUserCommand usando FluentValidation.
/// Define regras de validação rigorosas para criação administrativa de usuários.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    /// <summary>
    /// Papéis válidos no sistema.
    /// </summary>
    private static readonly string[] ValidRoles = { "Employee", "Manager", "HR", "Admin" };

    /// <summary>
    /// Inicializa as regras de validação para criação de usuário pelo administrador.
    /// </summary>
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Nome de usuário é obrigatório")
            .Length(3, 50).WithMessage("Nome deve ter entre 3 e 50 caracteres")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Nome pode conter apenas letras, números, pontos, hífens e underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
            .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Senha deve conter pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Número de telefone deve ter formato válido")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Papel é obrigatório")
            .Must(role => ValidRoles.Contains(role))
            .WithMessage($"Papel deve ser um dos seguintes: {string.Join(", ", ValidRoles)}");
    }
}
