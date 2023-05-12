using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Producers;
using Guexit.Game.Messages;
using MassTransit;
using NSubstitute;

namespace Guexit.Game.Producers.UnitTests;

public sealed class WhenHandlingPlayerJoinedGameRoom
{
    private readonly PlayerJoinedProducer _eventHandler;
    private readonly IBus _bus;

    public WhenHandlingPlayerJoinedGameRoom()
    {
        _bus = Substitute.For<IBus>();
        _eventHandler = new PlayerJoinedProducer(_bus);
    }

    [Fact]
    public async Task PlayerJoinedGameRoomIntegrationEventIsPublished()
    {
        var gameRoomId = new GameRoomId(Guid.Parse("0FF76CA8-1A45-4DC7-B426-8AFC0B1F0AE3"));
        var playerId = new PlayerId("F91981DC-017D-47A9-B7F6-CD8563118627");
        var playerJoined = new PlayerJoined(gameRoomId, playerId);

        await _eventHandler.Handle(playerJoined);

        await _bus.Received(1).Publish(Arg.Is<PlayerJoinedGameRoomIntegrationEvent>(e =>
            e.GameRoomId == playerJoined.GameRoomId && e.PlayerId == playerJoined.PlayerId));
    }
}