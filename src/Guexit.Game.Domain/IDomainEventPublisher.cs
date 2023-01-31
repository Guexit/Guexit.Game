namespace Guexit.Game.Domain;

public interface IDomainEventPublisher
{
    ValueTask Publish(IDomainEvent domainEvent, CancellationToken ct = default);
    ValueTask Publish(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);
}