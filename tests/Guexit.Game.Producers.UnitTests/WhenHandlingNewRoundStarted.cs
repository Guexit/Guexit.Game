using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingNewRoundStarted
{
    private readonly NewRoundStartedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingNewRoundStarted()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new NewRoundStartedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task NewRoundStartedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var newRoundStarted = new NewRoundStarted(gameRoomId);

        await _eventHandler.Handle(newRoundStarted);

        await _publishEndpoint.Received(1).Publish(Arg.Is<NewRoundStartedIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}