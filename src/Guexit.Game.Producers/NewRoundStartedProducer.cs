using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class NewRoundStartedProducer : IDomainEventHandler<NewRoundStarted>
{
    private readonly IBus _bus;

    public NewRoundStartedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(NewRoundStarted @event, CancellationToken ct = default) 
        => await _bus.Publish(new NewRoundStartedIntegrationEvent(@event.GameRoomId), ct);
}