using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingNextGameRoomLinked
{
    private readonly NextGameRoomLinkedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingNextGameRoomLinked()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new NextGameRoomLinkedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task GameStartedIntegrationEventIsPublished()
    {
        var finishedGameRoomId = new GameRoomId(Guid.NewGuid());
        var nextGameRoomId = new GameRoomId(Guid.NewGuid());
        var gameStarted = new NextGameRoomLinked(finishedGameRoomId, nextGameRoomId);

        await _eventHandler.Handle(gameStarted);

        await _publishEndpoint.Received(1).Publish(Arg.Is<NextGameRoomLinkedIntegrationEvent>(e =>
            e.FinishedGameRoomId == finishedGameRoomId && e.NextGameRoomId == nextGameRoomId));
    }
}