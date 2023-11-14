using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingAllPlayerCardsSubmitted
{
    private readonly AllPlayerCardsSubmittedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingAllPlayerCardsSubmitted()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new AllPlayerCardsSubmittedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task AllPlayerCardsSubmittedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameStarted = new AllPlayerCardsSubmitted(gameRoomId);

        await _eventHandler.Handle(gameStarted);

        await _publishEndpoint.Received(1).Publish(Arg.Is<AllPlayerCardsSubmittedIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}