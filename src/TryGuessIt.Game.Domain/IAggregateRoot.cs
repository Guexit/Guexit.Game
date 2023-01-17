namespace TryGuessIt.Game.Domain;

public interface IAggregateRoot
{
    uint Version { get; }
}
