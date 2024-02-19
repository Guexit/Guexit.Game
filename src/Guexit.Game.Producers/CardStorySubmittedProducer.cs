using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class CardStorySubmittedProducer : IDomainEventHandler<StoryTellerCardStorySubmitted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CardStorySubmittedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(StoryTellerCardStorySubmitted @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new CardStorySubmittedIntegrationEvent(@event.GameRoomId, 
            @event.SelectedCardId, @event.StoryTellerId, @event.Story), ct);
    }
}