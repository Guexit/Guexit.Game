namespace TryGuessIt.Game.Domain;

/// <summary>
/// Entity base class
/// </summary>
/// <typeparam name="TId">Type of Id of the entity</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId? Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity() 
    {
        // Entity Framework required parameterless ctor
    }

    protected Entity(TId id) => Id = id;

    protected void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        if (other.GetType() != GetType())
            return false;

        return Equals((Entity<TId>)other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
