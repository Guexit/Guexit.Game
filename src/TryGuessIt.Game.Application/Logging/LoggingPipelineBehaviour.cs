using Mediator;
using Microsoft.Extensions.Logging;

namespace TryGuessIt.Game.Application.Logging;

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
        try
        {
            _logger.LogInformation("Handling {requestType}", typeof(TRequest).Name);
            var response = await next(message, cancellationToken);
            _logger.LogInformation("{requestType} handled successfully", typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling {requestType}", typeof(TRequest).Name);
            throw;
        }
    }
}
