namespace TryGuessIt.Game.Domain.Model.PlayerAggregate;

public sealed class Player : Entity<string>
{
    public string Username { get; private set; }

    public Player(string id, string username) : base(id)
    {
        Username = username;
    }
}