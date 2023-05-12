using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class AllPlayerCardsSubmittedProducer : IDomainEventHandler<AllPlayerCardsSubmitted>
{
    private readonly IBus _bus;

    public AllPlayerCardsSubmittedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(AllPlayerCardsSubmitted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new AllPlayerCardsSubmittedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}