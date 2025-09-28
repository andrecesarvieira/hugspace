using FluentValidation;
using MediatR;

namespace SynQcore.Application.Behaviors;

/// <summary>
/// Behavior do pipeline MediatR para validação automática de requests.
/// Executa todos os validators registrados antes do handler principal.
/// </summary>
/// <typeparam name="TRequest">Tipo do request a ser validado.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta do handler.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Inicializa o behavior com os validators disponíveis.
    /// </summary>
    /// <param name="validators">Coleção de validators para o tipo de request.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Executa validação do request antes de chamar o próximo handler no pipeline.
    /// </summary>
    /// <param name="request">Request a ser validado.</param>
    /// <param name="next">Próximo handler no pipeline.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resposta do handler ou lança ValidationException se inválido.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
