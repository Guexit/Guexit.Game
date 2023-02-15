using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.EventHandlersForPublish;

public class PlayerJoinedGameRoomHandlerForPublish : IDomainEventHandler<PlayerJoinedGameRoom>
{
    private readonly IBus _bus;

    public PlayerJoinedGameRoomHandlerForPublish(IBus bus) => _bus = bus;

    public async ValueTask Handle(PlayerJoinedGameRoom @event, CancellationToken ct = default)
    {
        await _bus.Publish(new PlayerJoinedGameRoomIntegrationEvent
        {
            PlayerId = @event.PlayerId,
            GameRoomId = @event.GameRoomId
        }, ct);
    }
}