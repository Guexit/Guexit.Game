using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class VotingScoresComputedProducer : IDomainEventHandler<VotingScoresComputed>
{
    private readonly IBus _bus;

    public VotingScoresComputedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(VotingScoresComputed @event, CancellationToken ct = default)
    {
        await _bus.Publish(new VotingScoresComputedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
