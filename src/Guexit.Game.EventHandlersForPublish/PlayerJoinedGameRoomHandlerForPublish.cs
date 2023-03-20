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

public class GameStartedHandlerForPublish : IDomainEventHandler<GameStarted>
{
    private readonly IBus _bus;

    public GameStartedHandlerForPublish(IBus bus) => _bus = bus;

    public async ValueTask Handle(GameStarted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new AssignDeckCommand
        {
            GameRoomId = @event.GameRoomId
        }, ct);
    }
}