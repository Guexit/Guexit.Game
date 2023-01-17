using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TryGuessIt.Game.OutboxPublisher;

public sealed class OutboxMessagePublishHeartbeat : BackgroundService
{
    private readonly ILogger<OutboxMessagePublishHeartbeat> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(5));

    public OutboxMessagePublishHeartbeat(ILogger<OutboxMessagePublishHeartbeat> logger, IServiceProvider serviceProvider)
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
                var integrationEventsPublisher = scope.ServiceProvider.GetRequiredService<OutboxMessagePublisher>();

                _logger.LogInformation("Starting ", DateTimeOffset.Now);
                await integrationEventsPublisher.PublishMessages(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration events");
            }
        }
    }
}
