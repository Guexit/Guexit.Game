using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class CardReRollReservedProducer : IDomainEventHandler<CardReRollReserved>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CardReRollReservedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(CardReRollReserved @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new CardReRollReservedIntegrationEvent
        {
            GameRoomId = @event.GameRoomId,
            PlayerId = @event.PlayerId
        }, ct);
    }
}
