using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingGameStarted
{
    private readonly GameStartedProducer _eventHandler;
    private readonly IBus _bus;

    public WhenHandlingGameStarted()
    {
        _bus = Substitute.For<IBus>();
        _eventHandler = new GameStartedProducer(_bus);
    }

    [Fact]
    public async Task GameStartedIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var gameStarted = new GameStarted(gameRoomId);

        await _eventHandler.Handle(gameStarted);

        await _bus.Received(1).Publish(Arg.Is<GameStartedIntegrationEvent>(e => e.GameRoomId == gameRoomId));
    }
}
