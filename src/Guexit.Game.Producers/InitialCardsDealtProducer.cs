using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class InitialCardsDealtProducer : IDomainEventHandler<InitialCardsDealt>
{
    private readonly IBus _bus;

    public InitialCardsDealtProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(InitialCardsDealt @event, CancellationToken ct = default)
    {
        await _bus.Publish(new InitialCardsDealtIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
