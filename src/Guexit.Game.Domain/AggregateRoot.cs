namespace Guexit.Game.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    protected void AddDomainEvents(IDomainEvent[] domainEvents) => _domainEvents.AddRange(domainEvents);
    protected void ClearDomainEvents() => _domainEvents.Clear();
}
