using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingCardReRollCompleted
{
    private readonly CardReRollCompletedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingCardReRollCompleted()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new CardReRollCompletedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task CardReRollCompletedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var playerId = new PlayerId("player-123");
        var oldCardId = new CardId(Guid.NewGuid());
        var newCardId = new CardId(Guid.NewGuid());
        var cardReRollCompleted = new CardReRollCompleted(gameRoomId, playerId, oldCardId, newCardId);

        await _eventHandler.Handle(cardReRollCompleted);

        await _publishEndpoint.Received(1).Publish(Arg.Is<CardReRollCompletedIntegrationEvent>(e =>
            e.GameRoomId == gameRoomId.Value &&
            e.PlayerId == playerId.Value &&
            e.OldCardId == oldCardId.Value &&
            e.NewCardId == newCardId.Value));
    }
}