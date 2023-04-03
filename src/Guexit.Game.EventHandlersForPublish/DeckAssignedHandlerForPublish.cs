using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.EventHandlersForPublish;

public sealed class InitialCardsDealedHandlerForPublish : IDomainEventHandler<InitialCardsDealed>
{
    private readonly IBus _bus;

    public InitialCardsDealedHandlerForPublish(IBus bus) => _bus = bus;

    public async ValueTask Handle(InitialCardsDealed @event, CancellationToken ct = default)
    {
        await _bus.Publish(new DeckAssignedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}