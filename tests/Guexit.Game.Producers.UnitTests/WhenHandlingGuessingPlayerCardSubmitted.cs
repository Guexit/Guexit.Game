using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGuessingPlayerCardSubmitted
{
    private readonly GuessingPlayerCardSubmittedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingGuessingPlayerCardSubmitted()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new GuessingPlayerCardSubmittedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task GuessingPlayerCardSubmittedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.Parse("0FF76CA8-1A45-4DC7-B426-8AFC0B1F0AE3"));
        var playerId = new PlayerId("F91981DC-017D-47A9-B7F6-CD8563118627");
        var cardId = new CardId(Guid.Parse("F91981DC-017D-47A9-B7F6-CD8563118626"));
        var playerJoined = new GuessingPlayerCardSubmitted(gameRoomId, playerId, cardId);

        await _eventHandler.Handle(playerJoined);

        await _publishEndpoint.Received(1).Publish(Arg.Is<GuessingPlayerCardSubmittedIntegrationEvent>(e =>
            e.GameRoomId == playerJoined.GameRoomId && 
            e.PlayerId == playerJoined.PlayerId && 
            e.CardId == playerJoined.SelectedCardId));
    }
}