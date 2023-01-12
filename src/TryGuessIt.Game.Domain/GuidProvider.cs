namespace TryGuessIt.Game.Domain;

public interface IGuidProvider
{
    Guid NewGuid();
}

public sealed class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}