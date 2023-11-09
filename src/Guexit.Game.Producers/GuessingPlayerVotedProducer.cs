using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GuessingPlayerVotedProducer : IDomainEventHandler<GuessingPlayerVoted>
{
    private readonly IBus _bus;

    public GuessingPlayerVotedProducer(IBus bus) => _bus = bus;

    public async ValueTask Handle(GuessingPlayerVoted @event, CancellationToken ct = default)
    {
        await _bus.Publish(new GuessingPlayerVotedIntegrationEvent 
        { 
            GameRoomId = @event.GameRoomId,
            SelectedCardId = @event.SelectedCardId,
            PlayerId = @event.PlayerId
        }, ct);
    }
}