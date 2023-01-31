using System.Text.Json;
using Guexit.Game.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TryGuessIt.Game.Persistence;
using IAssemblyMarker = Guexit.Game.Messages.IAssemblyMarker;

namespace Guexit.Game.OutboxPublisher;

public interface IOutboxMessagePublisher
{
    Task PublishMessages(CancellationToken cancellationToken = default);
}

public sealed class OutboxMessagePublisher : IOutboxMessagePublisher
{
    private const int BatchSize = 400;
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

        var messages = await _dbContext.OutboxMessages
            .Where(x => x.PublishedAt == null)
            .OrderBy(x => x.CreatedAt)
            .Take(BatchSize)
            .ToArrayAsync(ct);
        _logger.LogInformation("Publishing {integrationEvents.Length} messages...", messages.Length);

        foreach (var message in messages)
        {
            try
            {
                var messageType = typeof(IAssemblyMarker).Assembly.GetType(message.FullyQualifiedTypeName);
                if (messageType is null)
                    throw new InvalidOperationException($"{message.FullyQualifiedTypeName} type cannot be resolved.");

                var deserializedMessage = JsonSerializer.Deserialize(message.SerializedData, messageType)!;
                await _bus.Publish(deserializedMessage, messageType, ct);
                message.MarkAsPublished(_clock.UtcNow);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error publishing message with id: {outboxMessageId}", message.Id);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}