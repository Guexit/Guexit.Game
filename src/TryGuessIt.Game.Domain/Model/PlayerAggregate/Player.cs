namespace TryGuessIt.Game.Domain.Model.PlayerAggregate;

public sealed class Player : Entity<PlayerId>, IAggregateRoot
{
    public string Username { get; private set; }



    public Player(PlayerId id, string username) : base(id)
    {
        Username = username;
    }
}