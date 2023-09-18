using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GameFinishedProducer : IDomainEventHandler<GameFinished>
{
    private readonly IBus _bus;

    public GameFinishedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(GameFinished @event, CancellationToken ct = default)
    {
        await _bus.Publish(new GameFinishedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}

