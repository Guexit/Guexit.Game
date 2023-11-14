using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GameStartedProducer : IDomainEventHandler<GameStarted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public GameStartedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(GameStarted @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new GameStartedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}
