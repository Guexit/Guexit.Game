using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;
using Mediator;

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

public sealed class TimerExpiredProducer : INotificationHandler<PlayerTimerStarted>
{
    private readonly IMessageScheduler _scheduler;

    public TimerExpiredProducer(IMessageScheduler scheduler)
    {
        _scheduler = scheduler;
    }
    
    public async ValueTask Handle(PlayerTimerStarted @event, CancellationToken cancellationToken)
    {
        var scheduledMessage = await _scheduler.SchedulePublish(
            DateTimeOffset.UtcNow.DateTime + TimeSpan.FromSeconds(20), 
            new TimerExpiredIntegrationEvent(@event.TimerId), 
            cancellationToken
        );
        
        // TODO: at this point in time we need to add the lookup between the timer and the message to be canceled
        // await _scheduler.CancelScheduledPublish<TimerExpiredIntegrationEvent>(scheduledMessage.TokenId);
    }
}
