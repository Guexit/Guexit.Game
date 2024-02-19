using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using Guexit.Game.Producers.TimerExpirationCancellation;
using MassTransit;
using Mediator;

namespace Guexit.Game.Producers;

public sealed class PlayerTimerExpiredProducer : INotificationHandler<PlayerTimerStarted>
{
    private readonly IMessageScheduler _scheduler;
    private readonly ITimerExpirationMessageIdLookup _expirationMessageIdLookup;
    private readonly ISystemClock _systemClock;

    public PlayerTimerExpiredProducer(IMessageScheduler scheduler)
    {
        _scheduler = scheduler;
    }
    
    public PlayerTimerExpiredProducer(IMessageScheduler scheduler, ITimerExpirationMessageIdLookup expirationMessageIdLookup, ISystemClock systemClock)
    {
        _scheduler = scheduler;
        _expirationMessageIdLookup = expirationMessageIdLookup;
        _systemClock = systemClock;
    }
    
    public async ValueTask Handle(PlayerTimerStarted @event, CancellationToken cancellationToken)
    {
        var scheduledMessage = await _scheduler.SchedulePublish(
            _systemClock.UtcNow.DateTime + TimeSpan.FromSeconds(20), 
            new PlayerTimerExpiredIntegrationEvent(@event.TimerId), 
            cancellationToken
        );

        await _expirationMessageIdLookup.Add(new TimerToMessageIdMap
        {
            TimerId = @event.TimerId,
            MessageId = scheduledMessage.TokenId
        }, cancellationToken);
    }
}