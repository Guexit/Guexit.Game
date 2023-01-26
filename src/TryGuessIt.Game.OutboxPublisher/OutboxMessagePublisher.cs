using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TryGuessIt.Game.Domain;
using TryGuessIt.Game.Persistence;

namespace TryGuessIt.Game.OutboxPublisher;

public interface IOutboxMessagePublisher
{
    Task PublishMessages(CancellationToken cancellationToken = default);
}

public sealed class OutboxMessagePublisher : IOutboxMessagePublisher
{
    private readonly GameDbContext _dbContext;
    private readonly IBus _bus;
    private readonly ILogger<OutboxMessagePublisher> _logger;
    private readonly ISystemClock _clock;

    public OutboxMessagePublisher(GameDbContext dbContext, IBus bus, ILogger<OutboxMessagePublisher> logger, ISystemClock clock)
    {
        _dbContext = dbContext;
        _bus = bus;
        _logger = logger;
        _clock = clock;
    }

    public async Task PublishMessages(CancellationToken ct = default)
    {
        var anyPendingToPublish = await _dbContext.OutboxMessages.AnyAsync(x => x.PublishedAt == null, ct);
        if (!anyPendingToPublish)
            return;

        var messagesToPublish = await _dbContext.OutboxMessages.Where(x => x.PublishedAt == null).ToArrayAsync(ct);
        _logger.LogInformation("Publishing {integrationEvents.Length} messages...", messagesToPublish.Length);

        foreach (var message in messagesToPublish)
        {
            var messageType = typeof(Messages.IAssemblyMarker).Assembly.GetType(message.FullyQualifiedTypeName);
            if (messageType is null)
                throw new InvalidOperationException($"{message.FullyQualifiedTypeName} type cannot be resolved.");

            var serializedMessage = JsonSerializer.Deserialize(message.SerializedData, messageType)!;
            await _bus.Publish(serializedMessage, messageType, ct);
            message.MarkAsPublished(DateTimeOffset.UtcNow);
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}