using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class CardReRollCompletedProducer : IDomainEventHandler<CardReRollCompleted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CardReRollCompletedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(CardReRollCompleted @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new CardReRollCompletedIntegrationEvent
        {
            GameRoomId = @event.GameRoomId,
            PlayerId = @event.PlayerId,
            OldCardId = @event.OldCardId,
            NewCardId = @event.NewCardId
        }, ct);
    }
}
