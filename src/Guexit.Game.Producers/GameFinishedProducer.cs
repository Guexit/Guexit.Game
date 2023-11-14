using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class GameFinishedProducer : IDomainEventHandler<GameFinished>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public GameFinishedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(GameFinished @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new GameFinishedIntegrationEvent { GameRoomId = @event.GameRoomId }, ct);
    }
}

