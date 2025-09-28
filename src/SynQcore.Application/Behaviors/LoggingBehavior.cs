using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SynQcore.Application.Behaviors;

/// <summary>
/// Behavior do pipeline MediatR para logging automático de performance e execução.
/// Registra tempo de execução, início/fim e erros de todos os handlers.
/// </summary>
/// <typeparam name="TRequest">Tipo do request sendo processado.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta do handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// LoggerMessage delegate otimizado para log de início de processamento.
    /// </summary>
    private static readonly Action<ILogger, string, object, Exception?> _logHandling =
        LoggerMessage.Define<string, object>(
            LogLevel.Information,
            new EventId(1, nameof(Handle)),
            "Handling {RequestName} {@Request}");

    /// <summary>
    /// LoggerMessage delegate otimizado para log de conclusão com sucesso.
    /// </summary>
    private static readonly Action<ILogger, string, long, Exception?> _logHandled =
        LoggerMessage.Define<string, long>(
            LogLevel.Information,
            new EventId(2, nameof(Handle)),
            "Handled {RequestName} in {ElapsedMs}ms");

    /// <summary>
    /// LoggerMessage delegate otimizado para log de erros.
    /// </summary>
    private static readonly Action<ILogger, string, long, Exception?> _logError =
        LoggerMessage.Define<string, long>(
            LogLevel.Error,
            new EventId(3, nameof(Handle)),
            "Error handling {RequestName} in {ElapsedMs}ms");

    /// <summary>
    /// Inicializa o behavior com o logger apropriado.
    /// </summary>
    /// <param name="logger">Logger para registrar eventos de execução.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executa o handler com logging de performance e tratamento de erros.
    /// </summary>
    /// <param name="request">Request sendo processado.</param>
    /// <param name="next">Próximo handler no pipeline.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resposta do handler com logs de performance registrados.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logHandling(_logger, requestName, request, null);

        try
        {
            var response = await next();

            stopwatch.Stop();
            _logHandled(_logger, requestName, stopwatch.ElapsedMilliseconds, null);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logError(_logger, requestName, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }
}
