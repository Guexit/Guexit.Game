using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GameRoomMarkedAsPublicProducer : IDomainEventHandler<GameRoomMarkedAsPublic>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public GameRoomMarkedAsPublicProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(GameRoomMarkedAsPublic @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new GameRoomMarkedAsPublicIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
