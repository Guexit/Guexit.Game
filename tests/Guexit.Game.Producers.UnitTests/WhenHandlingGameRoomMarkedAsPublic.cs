using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGameRoomMarkedAsPublic
{
    private readonly GameRoomMarkedAsPublicProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingGameRoomMarkedAsPublic()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new GameRoomMarkedAsPublicProducer(_publishEndpoint);
    }

    [Fact]
    public async Task GameRoomMarkedAsPublicIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameFinished = new GameRoomMarkedAsPublic(gameRoomId);

        await _eventHandler.Handle(gameFinished);

        await _publishEndpoint.Received(1).Publish(Arg.Is<GameRoomMarkedAsPublicIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}
