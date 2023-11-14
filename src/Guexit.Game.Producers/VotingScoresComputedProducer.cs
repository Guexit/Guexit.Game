using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class VotingScoresComputedProducer : IDomainEventHandler<VotingScoresComputed>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public VotingScoresComputedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(VotingScoresComputed @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new VotingScoresComputedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
