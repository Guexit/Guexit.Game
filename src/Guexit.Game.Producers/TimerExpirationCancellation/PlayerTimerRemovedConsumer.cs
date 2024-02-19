using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers.TimerExpirationCancellation;

public sealed class PlayerTimerRemovedConsumer : IDomainEventHandler<PlayerTimerRemoved>
{
    private readonly ITimerExpirationMessageIdLookup _messageIdLookup;
    private readonly IMessageScheduler _messageScheduler;

    public PlayerTimerRemovedConsumer(ITimerExpirationMessageIdLookup messageIdLookup, IMessageScheduler messageScheduler)
    {
        _messageIdLookup = messageIdLookup;
        _messageScheduler = messageScheduler;
    }
    
    public async ValueTask Handle(PlayerTimerRemoved @event, CancellationToken ct = default)
    {
        // TODO:
        // Maybe this has to be after that unit of work because it may cancel things and later fail committing the transaction?
        // potential solution: publish an integration event saying the timer was removed, and handle that message from the bus
        // to cancel the publish of the expiration event
        var messageId = await _messageIdLookup.GetMessageIdOf(@event.TimerId, ct);
        await _messageScheduler.CancelScheduledPublish<PlayerTimerExpiredIntegrationEvent>(messageId);
        
        await _messageIdLookup.Remove(@event.TimerId, ct);
    }
}