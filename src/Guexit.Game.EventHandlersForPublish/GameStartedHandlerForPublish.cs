using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.EventHandlersForPublish;

public class GameStartedHandlerForPublish : IDomainEventHandler<GameStarted>
{
    private readonly IBus _bus;

    public GameStartedHandlerForPublish(IBus bus) => _bus = bus;

    public async ValueTask Handle(GameStarted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new AssignDeckCommand { GameRoomId = @event.GameRoomId }, ct);
    }
}