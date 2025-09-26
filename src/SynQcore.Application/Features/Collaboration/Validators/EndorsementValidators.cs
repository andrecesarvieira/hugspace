using FluentValidation;
using SynQcore.Application.Features.Collaboration.Commands;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Validators;

/// <summary>
/// Validador para criação de endorsement corporativo
/// </summary>
public class CreateEndorsementCommandValidator : AbstractValidator<CreateEndorsementCommand>
{
    public CreateEndorsementCommandValidator()
    {
        RuleFor(x => x.Data)
            .NotNull()
            .WithMessage("Dados do endorsement são obrigatórios");

        RuleFor(x => x.EndorserId)
            .NotEmpty()
            .WithMessage("ID do endorser é obrigatório");

        // Deve especificar OU PostId OU CommentId, nunca ambos
        RuleFor(x => x.Data)
            .Must(data => (data.PostId.HasValue && !data.CommentId.HasValue) || 
                         (!data.PostId.HasValue && data.CommentId.HasValue))
            .WithMessage("Deve especificar um Post OU um Comment para endorsar, nunca ambos");

        RuleFor(x => x.Data.PostId)
            .NotEmpty()
            .When(x => x.Data.PostId.HasValue)
            .WithMessage("ID do post deve ser válido quando especificado");

        RuleFor(x => x.Data.CommentId)
            .NotEmpty()
            .When(x => x.Data.CommentId.HasValue)
            .WithMessage("ID do comment deve ser válido quando especificado");

        RuleFor(x => x.Data.Type)
            .IsInEnum()
            .WithMessage("Tipo de endorsement deve ser válido");

        RuleFor(x => x.Data.Note)
            .MaximumLength(500)
            .WithMessage("Nota do endorsement deve ter no máximo 500 caracteres");

        RuleFor(x => x.Data.Context)
            .MaximumLength(100)
            .WithMessage("Contexto deve ter no máximo 100 caracteres");
    }
}

/// <summary>
/// Validador para atualização de endorsement
/// </summary>
public class UpdateEndorsementCommandValidator : AbstractValidator<UpdateEndorsementCommand>
{
    public UpdateEndorsementCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID do endorsement é obrigatório");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.Data)
            .NotNull()
            .WithMessage("Dados para atualização são obrigatórios");

        RuleFor(x => x.Data.Type)
            .IsInEnum()
            .When(x => x.Data.Type.HasValue)
            .WithMessage("Tipo de endorsement deve ser válido");

        RuleFor(x => x.Data.Note)
            .MaximumLength(500)
            .WithMessage("Nota do endorsement deve ter no máximo 500 caracteres");

        RuleFor(x => x.Data.Context)
            .MaximumLength(100)
            .WithMessage("Contexto deve ter no máximo 100 caracteres");
    }
}

/// <summary>
/// Validador para toggle de endorsement
/// </summary>
public class ToggleEndorsementCommandValidator : AbstractValidator<ToggleEndorsementCommand>
{
    public ToggleEndorsementCommandValidator()
    {
        RuleFor(x => x.EndorserId)
            .NotEmpty()
            .WithMessage("ID do endorser é obrigatório");

        // Deve especificar OU PostId OU CommentId
        RuleFor(x => x)
            .Must(cmd => (cmd.PostId.HasValue && !cmd.CommentId.HasValue) || 
                        (!cmd.PostId.HasValue && cmd.CommentId.HasValue))
            .WithMessage("Deve especificar um Post OU um Comment para toggle, nunca ambos");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo de endorsement deve ser válido");

        RuleFor(x => x.Context)
            .MaximumLength(100)
            .WithMessage("Contexto deve ter no máximo 100 caracteres");
    }
}

/// <summary>
/// Validador para endorsement em massa
/// </summary>
public class BulkEndorsementCommandValidator : AbstractValidator<BulkEndorsementCommand>
{
    public BulkEndorsementCommandValidator()
    {
        RuleFor(x => x.EndorserId)
            .NotEmpty()
            .WithMessage("ID do endorser é obrigatório");

        // Deve especificar OU PostId OU CommentId
        RuleFor(x => x)
            .Must(cmd => (cmd.PostId.HasValue && !cmd.CommentId.HasValue) || 
                        (!cmd.PostId.HasValue && cmd.CommentId.HasValue))
            .WithMessage("Deve especificar um Post OU um Comment, nunca ambos");

        RuleFor(x => x.Types)
            .NotEmpty()
            .WithMessage("Deve especificar pelo menos um tipo de endorsement")
            .Must(types => types.Count <= 8)
            .WithMessage("Máximo de 8 tipos de endorsement por vez")
            .Must(types => types.Distinct().Count() == types.Count)
            .WithMessage("Não pode haver tipos duplicados na lista");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .WithMessage("Nota deve ter no máximo 500 caracteres");

        RuleFor(x => x.Context)
            .MaximumLength(100)
            .WithMessage("Contexto deve ter no máximo 100 caracteres");
    }
}