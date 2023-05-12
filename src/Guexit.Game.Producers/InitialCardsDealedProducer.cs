using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class InitialCardsDealedProducer : IDomainEventHandler<InitialCardsDealed>
{
    private readonly IBus _bus;

    public InitialCardsDealedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(InitialCardsDealed @event, CancellationToken ct = default)
    {
        await _bus.Publish(new InitialCardsDealedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}