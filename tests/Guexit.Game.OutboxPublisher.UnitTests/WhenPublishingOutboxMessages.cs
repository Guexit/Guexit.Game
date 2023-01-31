using System.Text.Json;
using FluentAssertions;
using Guexit.Game.Domain;
using Guexit.Game.Messages;
using Guexit.Game.OutboxPublisher;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TryGuessIt.Game.Persistence;
using TryGuessIt.Game.Persistence.Outbox;

namespace TryGuessIt.Game.OutboxPublisher.UnitTests;

public sealed class WhenPublishingOutboxMessages
{
    private readonly IBus _bus;
    private readonly GameDbContext _dbContext;
    private readonly OutboxMessagePublisher _outboxMessagePublisher;

    public WhenPublishingOutboxMessages()
    {
        _dbContext = new GameDbContext(new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        _bus = Substitute.For<IBus>();

        _outboxMessagePublisher = new OutboxMessagePublisher(_dbContext, _bus, 
            Substitute.For<ILogger<OutboxMessagePublisher>>(), Substitute.For<ISystemClock>());
    }

    [Fact]
    public async Task AreSentToBusAndMarkedAsPublished()
    {
        var event1 = new PlayerJoinedGameRoomIntegrationEvent(Guid.NewGuid(), "1");
        var event2 = new PlayerJoinedGameRoomIntegrationEvent(Guid.NewGuid(), "2");
        await AssumeMessagesInDatabase(BuildOutboxMessage(event1), BuildOutboxMessage(event2));

        await _outboxMessagePublisher.PublishMessages();

        await AssertEventWasPublished(event1);
        await AssertEventWasPublished(event2);
        var messagesPendingToPublish = await _dbContext.OutboxMessages.Where(x => x.PublishedAt == null).ToArrayAsync();
        messagesPendingToPublish.Should().BeEmpty();
    }

    private static OutboxMessage BuildOutboxMessage(PlayerJoinedGameRoomIntegrationEvent @event) =>
        new OutboxMessage(
            Guid.NewGuid(),
            typeof(PlayerJoinedGameRoomIntegrationEvent).FullName!,
            JsonSerializer.Serialize(@event),
            new DateTimeOffset(2023, 2, 2, 2, 2, 2, TimeSpan.Zero)
        );

    private async Task AssumeMessagesInDatabase(params OutboxMessage[] outboxMessages)
    {
        _dbContext.OutboxMessages.AddRange(
            outboxMessages
        );
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
    }

    private async Task AssertEventWasPublished(PlayerJoinedGameRoomIntegrationEvent @event)
    {
        await _bus.Received(1).Publish(
            Arg.Is<object>(x => ((PlayerJoinedGameRoomIntegrationEvent)x).PlayerId == @event.PlayerId &&
                                ((PlayerJoinedGameRoomIntegrationEvent)x).GameRoomId == @event.GameRoomId),
            Arg.Is<Type>(t => t == typeof(PlayerJoinedGameRoomIntegrationEvent)),
            Arg.Any<CancellationToken>()
        );
    }
}
