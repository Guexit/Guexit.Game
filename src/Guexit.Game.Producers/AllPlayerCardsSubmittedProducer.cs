using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class AllPlayerCardsSubmittedProducer : IDomainEventHandler<AllPlayerCardsSubmitted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public AllPlayerCardsSubmittedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(AllPlayerCardsSubmitted @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new AllPlayerCardsSubmittedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}