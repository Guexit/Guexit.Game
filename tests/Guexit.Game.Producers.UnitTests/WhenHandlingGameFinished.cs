using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGameFinished
{
    private readonly GameFinishedProducer _eventHandler;
    private readonly IBus _bus;

    public WhenHandlingGameFinished()
    {
        _bus = Substitute.For<IBus>();
        _eventHandler = new GameFinishedProducer(_bus);
    }

    [Fact]
    public async Task GameFinishedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameFinished = new GameFinished(gameRoomId);

        await _eventHandler.Handle(gameFinished);

        await _bus.Received(1).Publish(Arg.Is<GameFinishedIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}
