namespace TryGuessIt.Game.Domain.Model.PlayerAggregate;

public sealed class Player : Entity<PlayerId>, IAggregateRoot
{
    public uint Version { get; private set; }

    public string Username { get; private set; } = default!;

    public Player()
    {
        // Entity Framework required parameterless ctor
    }

    public Player(PlayerId id, string username) : base(id)
    {
        Username = username;
    }
}