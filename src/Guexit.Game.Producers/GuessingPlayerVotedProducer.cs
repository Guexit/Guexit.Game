using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GuessingPlayerVotedProducer : IDomainEventHandler<GuessingPlayerVoted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public GuessingPlayerVotedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(GuessingPlayerVoted @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new GuessingPlayerVotedIntegrationEvent 
        { 
            GameRoomId = @event.GameRoomId,
            SelectedCardId = @event.SelectedCardId,
            PlayerId = @event.PlayerId
        }, ct);
    }
}