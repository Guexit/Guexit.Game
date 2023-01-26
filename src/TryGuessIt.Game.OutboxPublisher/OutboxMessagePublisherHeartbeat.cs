using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TryGuessIt.Game.OutboxPublisher;

public sealed class OutboxMessagePublisherHeartbeat : BackgroundService
{
    private readonly ILogger<OutboxMessagePublisherHeartbeat> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(1));

    public OutboxMessagePublisherHeartbeat(ILogger<OutboxMessagePublisherHeartbeat> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (await _periodicTimer.WaitForNextTickAsync(ct) && !ct.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var publisher = scope.ServiceProvider.GetRequiredService<IOutboxMessagePublisher>();
                await publisher.PublishMessages(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration events");
            }
        }
    }
}
