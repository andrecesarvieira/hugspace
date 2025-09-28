using FluentValidation;
using SynQcore.Application.Features.Feed.Queries;

namespace SynQcore.Application.Validators.Feed;

/// <summary>
/// Validator para query de feed corporativo
/// Valida parâmetros de paginação e filtros
/// </summary>
public class GetCorporateFeedQueryValidator : AbstractValidator<GetCorporateFeedQuery>
{
    private static readonly string[] ValidFeedTypes =
        { "mixed", "department", "team", "following" };

    private static readonly string[] ValidSortOptions =
        { "relevance", "date", "popularity" };

    /// <summary>
    /// Inicializa validator para query do feed corporativo.
    /// </summary>
    public GetCorporateFeedQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Número da página deve ser maior que zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Número da página não pode exceder 1000");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(100)
            .WithMessage("Tamanho da página não pode exceder 100 itens");

        RuleFor(x => x.FeedType)
            .Must(x => string.IsNullOrEmpty(x) || ValidFeedTypes.Contains(x.ToLowerInvariant()))
            .WithMessage($"Tipo de feed deve ser um dos seguintes: {string.Join(", ", ValidFeedTypes)}")
            .When(x => !string.IsNullOrEmpty(x.FeedType));

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrEmpty(x) || ValidSortOptions.Contains(x.ToLowerInvariant()))
            .WithMessage($"Ordenação deve ser uma das seguintes: {string.Join(", ", ValidSortOptions)}")
            .When(x => !string.IsNullOrEmpty(x.SortBy));
    }
}

/// <summary>
/// Validator para query de estatísticas do feed
/// </summary>
public class GetFeedStatsQueryValidator : AbstractValidator<GetFeedStatsQuery>
{
    /// <summary>
    /// Inicializa validator para query de estatísticas do feed.
    /// </summary>
    public GetFeedStatsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");
    }
}

/// <summary>
/// Validator para query de feed do departamento
/// </summary>
public class GetDepartmentFeedQueryValidator : AbstractValidator<GetDepartmentFeedQuery>
{
    /// <summary>
    /// Inicializa uma nova instância do validador de feed departamental
    /// </summary>
    public GetDepartmentFeedQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("ID do departamento é obrigatório");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Número da página deve ser maior que zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Número da página não pode exceder 1000");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(100)
            .WithMessage("Tamanho da página não pode exceder 100 itens");
    }
}

/// <summary>
/// Validator para query de conteúdo em alta
/// </summary>
public class GetTrendingContentQueryValidator : AbstractValidator<GetTrendingContentQuery>
{
    /// <summary>
    /// Inicializa uma nova instância do validador de conteúdo em alta
    /// </summary>
    public GetTrendingContentQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Número da página deve ser maior que zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Número da página não pode exceder 1000");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(50)
            .WithMessage("Tamanho da página não pode exceder 50 itens para conteúdo em alta");

        RuleFor(x => x.TimeWindow)
            .Must(timeWindow => IsValidTimeWindow(timeWindow))
            .WithMessage("Janela de tempo deve ser '24h', '7d' ou '30d'");

        RuleFor(x => x.Department)
            .MaximumLength(100)
            .WithMessage("Nome do departamento não pode exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Department));
    }

    private static bool IsValidTimeWindow(string? timeWindow)
    {
        if (string.IsNullOrEmpty(timeWindow)) return true;
        return timeWindow is "24h" or "7d" or "30d";
    }
}

/// <summary>
/// Validator para query de conteúdo recomendado
/// </summary>
public class GetRecommendedContentQueryValidator : AbstractValidator<GetRecommendedContentQuery>
{
    /// <summary>
    /// Inicializa uma nova instância do validador de conteúdo recomendado
    /// </summary>
    public GetRecommendedContentQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Número da página deve ser maior que zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Número da página não pode exceder 1000");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(50)
            .WithMessage("Tamanho da página não pode exceder 50 itens para recomendações");

        RuleFor(x => x.MinRelevanceScore)
            .InclusiveBetween(0.0, 1.0)
            .WithMessage("Pontuação mínima de relevância deve estar entre 0.0 e 1.0");
    }
}

/// <summary>
/// Validator para query de interesses do usuário
/// </summary>
public class GetUserInterestsQueryValidator : AbstractValidator<GetUserInterestsQuery>
{
    /// <summary>
    /// Inicializa uma nova instância do validador de interesses do usuário
    /// </summary>
    public GetUserInterestsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("ID do usuário é obrigatório");
    }
}
