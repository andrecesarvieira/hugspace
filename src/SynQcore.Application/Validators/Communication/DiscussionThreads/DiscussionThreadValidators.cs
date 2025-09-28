using FluentValidation;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Validators.Communication.DiscussionThreads;

public class CreateDiscussionCommentCommandValidator : AbstractValidator<CreateDiscussionCommentCommand>
{
    public CreateDiscussionCommentCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("ID do post é obrigatório.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Conteúdo do comentário é obrigatório.")
            .MaximumLength(2000)
            .WithMessage("Conteúdo não pode exceder 2000 caracteres.")
            .MinimumLength(1)
            .WithMessage("Comentário deve ter pelo menos 1 caractere.");

        RuleFor(x => x.Type)
            .Must(BeValidCommentType)
            .WithMessage("Tipo de comentário inválido.");

        RuleFor(x => x.Visibility)
            .Must(BeValidVisibility)
            .WithMessage("Visibilidade inválida.");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority)
            .WithMessage("Prioridade inválida.");

        When(x => x.Mentions != null && x.Mentions.Count > 0, () =>
        {
            RuleFor(x => x.Mentions)
                .Must(HaveValidMentions)
                .WithMessage("Menções contêm dados inválidos.");
        });
    }

    private static bool BeValidCommentType(string type)
    {
        return Enum.TryParse<CommentType>(type, true, out _);
    }

    private static bool BeValidVisibility(string visibility)
    {
        return Enum.TryParse<CommentVisibility>(visibility, true, out _);
    }

    private static bool BeValidPriority(string priority)
    {
        return Enum.TryParse<CommentPriority>(priority, true, out _);
    }

    private static bool HaveValidMentions(List<CreateCommentMentionDto>? mentions)
    {
        if (mentions == null) return true;

        return mentions.All(m =>
            m.MentionedEmployeeId != Guid.Empty &&
            !string.IsNullOrWhiteSpace(m.MentionText) &&
            m.StartPosition >= 0 &&
            m.Length > 0);
    }
}

public class UpdateDiscussionCommentCommandValidator : AbstractValidator<UpdateDiscussionCommentCommand>
{
    public UpdateDiscussionCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("ID do comentário é obrigatório.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Conteúdo do comentário é obrigatório.")
            .MaximumLength(2000)
            .WithMessage("Conteúdo não pode exceder 2000 caracteres.")
            .MinimumLength(1)
            .WithMessage("Comentário deve ter pelo menos 1 caractere.");

        RuleFor(x => x.Type)
            .Must(BeValidCommentType)
            .WithMessage("Tipo de comentário inválido.");

        RuleFor(x => x.Visibility)
            .Must(BeValidVisibility)
            .WithMessage("Visibilidade inválida.");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority)
            .WithMessage("Prioridade inválida.");
    }

    private static bool BeValidCommentType(string type)
    {
        return Enum.TryParse<CommentType>(type, true, out _);
    }

    private static bool BeValidVisibility(string visibility)
    {
        return Enum.TryParse<CommentVisibility>(visibility, true, out _);
    }

    private static bool BeValidPriority(string priority)
    {
        return Enum.TryParse<CommentPriority>(priority, true, out _);
    }
}

public class ModerateDiscussionCommentCommandValidator : AbstractValidator<ModerateDiscussionCommentCommand>
{
    public ModerateDiscussionCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("ID do comentário é obrigatório.");

        RuleFor(x => x.ModerationStatus)
            .NotEmpty()
            .WithMessage("Status de moderação é obrigatório.")
            .Must(BeValidModerationStatus)
            .WithMessage("Status de moderação inválido.");

        When(x => x.ModerationStatus is "Flagged" or "Hidden" or "Rejected", () =>
        {
            RuleFor(x => x.ModerationReason)
                .NotEmpty()
                .WithMessage("Motivo da moderação é obrigatório para este status.")
                .MaximumLength(500)
                .WithMessage("Motivo não pode exceder 500 caracteres.");
        });
    }

    private static bool BeValidModerationStatus(string status)
    {
        return Enum.TryParse<ModerationStatus>(status, true, out _);
    }
}

public class ResolveDiscussionCommentCommandValidator : AbstractValidator<ResolveDiscussionCommentCommand>
{
    public ResolveDiscussionCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("ID do comentário é obrigatório.");

        When(x => !string.IsNullOrWhiteSpace(x.ResolutionNote), () =>
        {
            RuleFor(x => x.ResolutionNote)
                .MaximumLength(1000)
                .WithMessage("Nota de resolução não pode exceder 1000 caracteres.");
        });
    }
}
