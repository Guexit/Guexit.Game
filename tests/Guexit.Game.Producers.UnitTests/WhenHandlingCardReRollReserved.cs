using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingCardReRollReserved
{
    private readonly CardReRollReservedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingCardReRollReserved()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new CardReRollReservedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task CardReRollReservedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("player-123");
        var cardReRollReserved = new CardReRollReserved(gameRoomId, playerId);

        await _eventHandler.Handle(cardReRollReserved);

        await _publishEndpoint.Received(1).Publish(Arg.Is<CardReRollReservedIntegrationEvent>(e =>
            e.GameRoomId == gameRoomId.Value &&
            e.PlayerId == playerId.Value));
    }
}
