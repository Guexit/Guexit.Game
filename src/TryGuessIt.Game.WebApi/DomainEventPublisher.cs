using Mediator;
using TryGuessIt.Game.Domain;

namespace TryGuessIt.Game.WebApi;

public sealed class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IPublisher _publisher;

    public DomainEventPublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async ValueTask Publish(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        if (!domainEvents.Any())
            return;

        foreach (var domainEvent in domainEvents)
            await _publisher.Publish(domainEvent, ct);
    }

    public async ValueTask Publish(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        await _publisher.Publish(domainEvent, ct);
    }
}
