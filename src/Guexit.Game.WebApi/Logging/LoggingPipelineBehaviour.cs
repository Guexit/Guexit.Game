using System.Diagnostics;
using Mediator;

namespace Guexit.Game.WebApi.Logging;

public sealed class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var timestamp = Stopwatch.GetTimestamp();

        try
        {
            _logger.LogRequestHandlingStart(typeof(TRequest).Name);
            var response = await next(message, cancellationToken);
            _logger.LogRequestHandlingCompleted(typeof(TRequest).Name, Stopwatch.GetElapsedTime(timestamp));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling {requestType}. It took {elapsed}", typeof(TRequest).Name, Stopwatch.GetElapsedTime(timestamp));
            throw;
        }
    }
}

public static partial class LoggingPipelineLogExtensions
{
    [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "Handling {requestTypeName}")]
    public static partial void LogRequestHandlingStart(this ILogger logger, string requestTypeName);


    [LoggerMessage(EventId = 22, Level = LogLevel.Information, Message = "{requestTypeName} handled successfully in {elapsed}")]
    public static partial void LogRequestHandlingCompleted(this ILogger logger, string requestTypeName, TimeSpan elapsed);
}