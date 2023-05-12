using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GuessingPlayerCardSubmittedProducer : IDomainEventHandler<GuessingPlayerCardSubmitted>
{
    private readonly IBus _bus;

    public GuessingPlayerCardSubmittedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(GuessingPlayerCardSubmitted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new GuessingPlayerCardSubmittedIntegrationEvent 
        { 
            GameRoomId = @event.GameRoomId,
            CardId = @event.SelectedCardId,
            PlayerId = @event.PlayerId
        }, ct);
    }
}
