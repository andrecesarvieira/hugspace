using FluentValidation;
using SynQcore.Application.Features.Feed.Commands;

namespace SynQcore.Application.Validators.Feed;

/// <summary>
/// Validator para comando de regeneração de feed
/// Valida parâmetros de entrada e regras corporativas
/// </summary>
public class RegenerateFeedCommandValidator : AbstractValidator<RegenerateFeedCommand>
{
    public RegenerateFeedCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.DaysToInclude)
            .GreaterThan(0)
            .WithMessage("Dias para incluir deve ser maior que zero")
            .LessThanOrEqualTo(365)
            .WithMessage("Não é possível incluir mais de 365 dias");

        RuleFor(x => x.MaxItems)
            .GreaterThan(0)
            .WithMessage("Máximo de itens deve ser maior que zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Máximo de itens não pode exceder 1000")
            .When(x => x.MaxItems.HasValue);
    }
}

/// <summary>
/// Validator para comando de marcar item como lido
/// </summary>
public class MarkFeedItemAsReadCommandValidator : AbstractValidator<MarkFeedItemAsReadCommand>
{
    public MarkFeedItemAsReadCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.FeedEntryId)
            .NotEmpty()
            .WithMessage("ID do item do feed é obrigatório");
    }
}

/// <summary>
/// Validator para comando de bookmark/unbookmark
/// </summary>
public class ToggleFeedBookmarkCommandValidator : AbstractValidator<ToggleFeedBookmarkCommand>
{
    public ToggleFeedBookmarkCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.FeedEntryId)
            .NotEmpty()
            .WithMessage("ID do item do feed é obrigatório");
    }
}

/// <summary>
/// Validator para comando de ocultar item do feed
/// </summary>
public class HideFeedItemCommandValidator : AbstractValidator<HideFeedItemCommand>
{
    public HideFeedItemCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.FeedEntryId)
            .NotEmpty()
            .WithMessage("ID do item do feed é obrigatório");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Motivo não pode exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}

/// <summary>
/// Validator para comando de atualizar interesses do usuário
/// </summary>
public class UpdateUserInterestsCommandValidator : AbstractValidator<UpdateUserInterestsCommand>
{
    private static readonly string[] ValidInteractionTypes = 
        { "view", "like", "comment", "share", "bookmark" };

    public UpdateUserInterestsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage("ID do conteúdo é obrigatório");

        RuleFor(x => x.InteractionType)
            .NotEmpty()
            .WithMessage("Tipo de interação é obrigatório")
            .Must(x => ValidInteractionTypes.Contains(x.ToLowerInvariant()))
            .WithMessage($"Tipo de interação deve ser um dos seguintes: {string.Join(", ", ValidInteractionTypes)}");
    }
}

/// <summary>
/// Validator para comando de processamento em lote
/// </summary>
public class ProcessBulkFeedUpdateCommandValidator : AbstractValidator<ProcessBulkFeedUpdateCommand>
{
    private static readonly string[] ValidUpdateTypes = 
        { "new_post", "post_updated", "post_deleted" };

    public ProcessBulkFeedUpdateCommandValidator()
    {
        RuleFor(x => x.PostIds)
            .NotEmpty()
            .WithMessage("Lista de IDs de posts é obrigatória")
            .Must(x => x.Count <= 100)
            .WithMessage("Não é possível processar mais de 100 posts em lote");

        RuleForEach(x => x.PostIds)
            .NotEmpty()
            .WithMessage("ID do post não pode ser vazio");

        RuleFor(x => x.UpdateType)
            .NotEmpty()
            .WithMessage("Tipo de atualização é obrigatório")
            .Must(x => ValidUpdateTypes.Contains(x.ToLowerInvariant()))
            .WithMessage($"Tipo de atualização deve ser um dos seguintes: {string.Join(", ", ValidUpdateTypes)}");
    }
}