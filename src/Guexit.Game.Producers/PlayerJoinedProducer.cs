using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public class PlayerJoinedProducer : IDomainEventHandler<PlayerJoined>
{
    private readonly IBus _bus;

    public PlayerJoinedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(PlayerJoined @event, CancellationToken ct = default)
    {
        await _bus.Publish(new PlayerJoinedGameRoomIntegrationEvent
        {
            PlayerId = @event.PlayerId,
            GameRoomId = @event.GameRoomId
        }, ct);
    }
}