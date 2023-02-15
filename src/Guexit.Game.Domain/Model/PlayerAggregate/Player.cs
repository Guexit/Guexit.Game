namespace Guexit.Game.Domain.Model.PlayerAggregate;

public sealed class Player : AggregateRoot<PlayerId>
{
    public uint Version { get; private set; }

    public string Username { get; private set; } = default!;

    public Player()
    {
        // Entity Framework required parameterless ctor
    }

    public Player(PlayerId id, string username)
    {
        Id = id;
        Username = username;
    }
}