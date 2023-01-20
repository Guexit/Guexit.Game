using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TryGuessIt.Game.Messages;
using TryGuessIt.Game.Persistence;
using TryGuessIt.Game.Persistence.Outbox;

namespace TryGuessIt.Game.OutboxPublisher.UnitTests;

public sealed class WhenPublishingOutboxMessages
{
    private readonly ILogger<OutboxMessagePublisher> _logger;
    private readonly IBus _bus;
    private readonly GameDbContext _dbContext;
    private readonly OutboxMessagePublisher _outboxMessagePublisher;

    public WhenPublishingOutboxMessages()
    {
        _dbContext = new GameDbContext(new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        _bus = Substitute.For<IBus>();
        _logger = Substitute.For<ILogger<OutboxMessagePublisher>>();

        _outboxMessagePublisher = new OutboxMessagePublisher(_dbContext, _bus, _logger);

    }

    [Fact]
    public async Task EventsArePublishedInBus()
    {
        var event1 = new PlayerJoinedGameRoomIntegrationEvent(Guid.NewGuid(), "1");
        var messageId1 = Guid.Parse("11116740-70CC-40F4-94A7-AD409CAA43F7");
        var typeName1 = event1.GetType().FullName!;
        var serializedData1 = JsonSerializer.Serialize(event1);
        var storedAt1 = new DateTimeOffset(2023, 1, 1, 1, 1, 1, TimeSpan.Zero);

        var event2 = new PlayerJoinedGameRoomIntegrationEvent(Guid.NewGuid(), "2");
        var messageId2 = Guid.Parse("22226740-70CC-40F4-94A7-AD409CAA43F7");
        var typeName2 = event2.GetType().FullName!;
        var serializedData2 = JsonSerializer.Serialize(event2);
        var storedAt2 = new DateTimeOffset(2023, 2, 2, 2, 2, 2, TimeSpan.Zero);

        var storedMessages = new OutboxMessage[]
        {
            new OutboxMessage(
                messageId1,
                typeName1,
                serializedData1,
                storedAt1
            ),
            new OutboxMessage(
                messageId2,
                typeName2,
                serializedData2,
                storedAt2
            )
        };

        _dbContext.OutboxMessages.AddRange(storedMessages);
        await _dbContext.SaveChangesAsync();

        await _outboxMessagePublisher.PublishMessages();

        await _bus.Received(1).Publish(
            Arg.Is<object>(x => ((PlayerJoinedGameRoomIntegrationEvent)x).PlayerId == event1.PlayerId && 
                                ((PlayerJoinedGameRoomIntegrationEvent)x).GameRoomId == event1.GameRoomId), 
            Arg.Is<Type>(t => t == typeof(PlayerJoinedGameRoomIntegrationEvent)),
            Arg.Any<CancellationToken>()
        );
        await _bus.Received(1).Publish(
            Arg.Is<object>(x => ((PlayerJoinedGameRoomIntegrationEvent)x).PlayerId == event2.PlayerId && 
                                ((PlayerJoinedGameRoomIntegrationEvent)x).GameRoomId == event2.GameRoomId),
            Arg.Is<Type>(t => t == typeof(PlayerJoinedGameRoomIntegrationEvent)),
            Arg.Any<CancellationToken>()
        );
    }
}
