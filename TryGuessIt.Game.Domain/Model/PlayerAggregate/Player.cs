namespace TryGuessIt.Game.Domain.Model.PlayerAggregate;

public sealed class Player : Entity<string>
{
    public Player(string id) : base(id)
    {
    }
}
