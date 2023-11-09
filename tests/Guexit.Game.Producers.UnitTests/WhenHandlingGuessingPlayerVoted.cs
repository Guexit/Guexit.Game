using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGuessingPlayerVoted
{
    private readonly GuessingPlayerVotedProducer _eventHandler;
    private readonly IBus _bus;

    public WhenHandlingGuessingPlayerVoted()
    {
        _bus = Substitute.For<IBus>();
        _eventHandler = new GuessingPlayerVotedProducer(_bus);
    }

    [Fact]
    public async Task GuessingPlayerVotedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.Parse("0FF76CA8-1A45-4DC7-B426-8AFC0B1F0AE3"));
        var playerId = new PlayerId("F91981DC-017D-47A9-B7F6-CD8563118627");
        var cardId = new CardId(Guid.Parse("F91981DC-017D-47A9-B7F6-CD8563118626"));
        var @event = new GuessingPlayerVoted(gameRoomId, playerId, cardId);

        await _eventHandler.Handle(@event);

        await _bus.Received(1).Publish(Arg.Is<GuessingPlayerVotedIntegrationEvent>(e =>
            e.GameRoomId == @event.GameRoomId && 
            e.PlayerId == @event.PlayerId && 
            e.SelectedCardId == @event.SelectedCardId));
    }
}