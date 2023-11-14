using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public class PlayerJoinedProducer : IDomainEventHandler<PlayerJoined>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PlayerJoinedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(PlayerJoined @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new PlayerJoinedGameRoomIntegrationEvent
        {
            PlayerId = @event.PlayerId,
            GameRoomId = @event.GameRoomId
        }, ct);
    }
}