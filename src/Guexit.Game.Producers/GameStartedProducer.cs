using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GameStartedProducer : IDomainEventHandler<GameStarted>
{
    private readonly IBus _bus;

    public GameStartedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(GameStarted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new GameStartedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
