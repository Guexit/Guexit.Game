using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGameFinished
{
    private readonly GameFinishedProducer _eventHandler;
    private readonly IPublishEndpoint _publishEndpoint;

    public WhenHandlingGameFinished()
    {
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _eventHandler = new GameFinishedProducer(_publishEndpoint);
    }

    [Fact]
    public async Task GameFinishedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameFinished = new GameFinished(gameRoomId);

        await _eventHandler.Handle(gameFinished);

        await _publishEndpoint.Received(1).Publish(Arg.Is<GameFinishedIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}
