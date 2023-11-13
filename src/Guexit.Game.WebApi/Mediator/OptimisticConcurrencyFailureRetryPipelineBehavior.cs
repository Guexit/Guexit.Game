using Mediator;
using Microsoft.EntityFrameworkCore;
using Polly;
using ICommand = Guexit.Game.Application.ICommand;

namespace Guexit.Game.WebApi.Mediator;

public sealed class OptimisticConcurrencyFailureRetryPipelineBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    private const int MaxRetries = 5;
    
    private readonly ILogger<OptimisticConcurrencyFailureRetryPipelineBehavior<TCommand, TResponse>> _logger;

    public OptimisticConcurrencyFailureRetryPipelineBehavior(ILogger<OptimisticConcurrencyFailureRetryPipelineBehavior<TCommand, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<TResponse> Handle(TCommand command, CancellationToken ct, MessageHandlerDelegate<TCommand, TResponse> next)
    {
        var retryPolicy = Policy.Handle<DbUpdateConcurrencyException>()
            .WaitAndRetryAsync(
                retryCount: MaxRetries, 
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)) / 2, 
                onRetry: (exception, _, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Optimistic concurrency violation occurred. Retry {RetryCount} of {MaxRetries}", retryCount,
                        MaxRetries);
                });
        
        var response = await retryPolicy
            .ExecuteAsync(async () => await next.Invoke(command, ct))
            .WaitAsync(ct);
        
        return response;
    }
}