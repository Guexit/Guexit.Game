using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GameRoomMarkedAsPrivateProducer : IDomainEventHandler<GameRoomMarkedAsPrivate>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public GameRoomMarkedAsPrivateProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(GameRoomMarkedAsPrivate @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new GameRoomMarkedAsPrivateIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
