using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGameRoomMarkedAsPrivate
{
    private readonly GameRoomMarkedAsPrivateProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingGameRoomMarkedAsPrivate()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new GameRoomMarkedAsPrivateProducer(_publishEndpoint);
    }

    [Fact]
    public async Task GameRoomMarkedAsPrivateIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameFinished = new GameRoomMarkedAsPrivate(gameRoomId);

        await _eventHandler.Handle(gameFinished);

        await _publishEndpoint.Received(1).Publish(Arg.Is<GameRoomMarkedAsPrivateIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}
