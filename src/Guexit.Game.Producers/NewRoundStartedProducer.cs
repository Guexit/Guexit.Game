using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Producers;

public sealed class NewRoundStartedProducer : IDomainEventHandler<NewRoundStarted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public NewRoundStartedProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async ValueTask Handle(NewRoundStarted @event, CancellationToken ct = default) 
        => await _publishEndpoint.Publish(new NewRoundStartedIntegrationEvent(@event.GameRoomId), ct);
}