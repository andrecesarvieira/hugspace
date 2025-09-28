using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SynQcore.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    private static readonly Action<ILogger, string, object, Exception?> _logHandling =
        LoggerMessage.Define<string, object>(
            LogLevel.Information,
            new EventId(1, nameof(Handle)),
            "Handling {RequestName} {@Request}");

    private static readonly Action<ILogger, string, long, Exception?> _logHandled =
        LoggerMessage.Define<string, long>(
            LogLevel.Information,
            new EventId(2, nameof(Handle)),
            "Handled {RequestName} in {ElapsedMs}ms");

    private static readonly Action<ILogger, string, long, Exception?> _logError =
        LoggerMessage.Define<string, long>(
            LogLevel.Error,
            new EventId(3, nameof(Handle)),
            "Error handling {RequestName} in {ElapsedMs}ms");

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

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
