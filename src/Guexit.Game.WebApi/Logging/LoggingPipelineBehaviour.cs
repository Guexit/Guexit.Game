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
            _logger.LogInformation("Handling {requestType}", typeof(TRequest).Name);
            var response = await next(message, cancellationToken);
            _logger.LogInformation("{requestType} handled successfully in {elapsed}", typeof(TRequest).Name, Stopwatch.GetElapsedTime(timestamp));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling {requestType}. It took {elapsed}", typeof(TRequest).Name, Stopwatch.GetElapsedTime(timestamp));
            throw;
        }
    }
}
