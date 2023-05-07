using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.EventHandlersForPublish;

public sealed class CardStorySubmittedForPublish : IDomainEventHandler<StoryTellerCardStorySubmitted>
{
    private readonly IBus _bus;

    public CardStorySubmittedForPublish(IBus bus) => _bus = bus;

    public async ValueTask Handle(StoryTellerCardStorySubmitted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new CardStorySubmittedIntegrationEvent(@event.GameRoomId, 
            @event.SelectedCardId, @event.StoryTellerId, @event.Story), ct);
    }
}
