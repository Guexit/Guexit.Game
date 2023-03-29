using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.EventHandlersForPublish;

public sealed class DeckAssignedForPublish : IDomainEventHandler<DeckAssigned>
{
    private readonly IBus _bus;

    public DeckAssignedForPublish(IBus bus) => _bus = bus;

    public async ValueTask Handle(DeckAssigned @event, CancellationToken ct = default)
    {
        await _bus.Publish(new DeckAssignedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}