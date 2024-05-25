namespace Guexit.Game.Domain;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : notnull
{
    private List<IDomainEvent>? _domainEvents;
    
    public IReadOnlyList<IDomainEvent> DomainEvents
    {
        get
        {
            if (_domainEvents is null)
                return Array.Empty<IDomainEvent>();
            
            return _domainEvents.ToArray().AsReadOnly();
        }
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    protected void AddDomainEvents(params IDomainEvent[] domainEvents)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.AddRange(domainEvents);
    }

    public void ClearDomainEvents() => _domainEvents?.Clear();
}
