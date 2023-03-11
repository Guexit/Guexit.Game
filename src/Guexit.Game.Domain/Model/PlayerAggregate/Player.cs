namespace Guexit.Game.Domain.Model.PlayerAggregate;

public sealed class Player : AggregateRoot<PlayerId>
{
    public string Username { get; private set; } = default!;

    private Player()
    {
        // Entity Framework required parameterless ctor
    }

    public Player(PlayerId id, string username)
    {
        Id = id;
        Username = username;
    }
}