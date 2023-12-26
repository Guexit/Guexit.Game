using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class NextGameRoomLinkedProducer : IDomainEventHandler<NextGameRoomLinked>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public NextGameRoomLinkedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(NextGameRoomLinked @event, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(new NextGameRoomLinkedIntegrationEvent
        {
            FinishedGameRoomId = @event.FinishedGameRoomId,
            NextGameRoomId = @event.NextGameRoomId
        }, ct);
    }
}