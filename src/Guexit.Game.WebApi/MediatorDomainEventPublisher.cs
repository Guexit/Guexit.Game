using Guexit.Game.Domain;
using Mediator;

namespace Guexit.Game.WebApi;

public sealed class MediatorDomainEventPublisher : IDomainEventPublisher
{
    private readonly IPublisher _publisher;

    public MediatorDomainEventPublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async ValueTask Publish(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
            await _publisher.Publish(domainEvent, ct);
    }

    public async ValueTask Publish(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        await _publisher.Publish(domainEvent, ct);
    }
}
