﻿namespace Guexit.Game.Domain;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.ToList().AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    protected void AddDomainEvents(params IDomainEvent[] domainEvents) => _domainEvents.AddRange(domainEvents);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
