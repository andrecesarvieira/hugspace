using FluentValidation;
using SynQcore.Application.Features.Feed.DTOs;
using System.Text.RegularExpressions;

namespace SynQcore.Application.Features.Feed.Validators;

/// <summary>
/// Validador para atualização de posts do feed corporativo
/// </summary>
public partial class UpdateFeedPostRequestValidator : AbstractValidator<UpdateFeedPostRequest>
{
    [GeneratedRegex(@"^[a-zA-Z0-9\-_]+$", RegexOptions.Compiled)]
    private static partial Regex TagValidationRegex();
    public UpdateFeedPostRequestValidator()
    {
        // Validação do conteúdo (opcional em updates)
        RuleFor(x => x.Content)
            .MaximumLength(5000)
            .WithMessage("O conteúdo deve ter no máximo 5000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Content));

        // Validação de URL da imagem (opcional)
        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl)
            .WithMessage("A URL da imagem deve ser válida")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        // Validação das tags (opcional)
        RuleFor(x => x.Tags)
            .Must(HaveValidTags)
            .WithMessage("As tags devem ter entre 2 e 50 caracteres cada, sem espaços")
            .When(x => x.Tags != null && x.Tags.Length > 0);

        RuleFor(x => x.Tags)
            .Must(x => x == null || x.Length <= 10)
            .WithMessage("Máximo de 10 tags permitidas");

        // Validação de visibilidade (opcional)
        RuleFor(x => x.IsPublic)
            .NotNull()
            .WithMessage("A visibilidade deve ser especificada")
            .When(x => x.IsPublic.HasValue);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true; // URL vazia é válida para campos opcionais

        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    private static bool HaveValidTags(string[]? tags)
    {
        if (tags == null || tags.Length == 0)
            return true;

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            var trimmedTag = tag.Trim();
            
            // Tag deve ter entre 2 e 50 caracteres
            if (trimmedTag.Length < 2 || trimmedTag.Length > 50)
                return false;

            // Tag não deve conter espaços
            if (trimmedTag.Contains(' '))
                return false;

            // Tag deve conter apenas letras, números, hífen e underscore
            if (!TagValidationRegex().IsMatch(trimmedTag))
                return false;
        }

        return true;
    }
}