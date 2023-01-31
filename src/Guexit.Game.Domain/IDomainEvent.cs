using Mediator;

namespace TryGuessIt.Game.Domain;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent : INotification
{
}

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
}
