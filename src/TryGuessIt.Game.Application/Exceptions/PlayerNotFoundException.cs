using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.Exceptions;

public sealed class PlayerNotFoundException : AggregateNotFoundException
{
    public PlayerNotFoundException(PlayerId playerId) 
        : base($"Player with id {playerId.Value} not found")
    {
    }
}

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException(string message) : base(message)
    {
    }
}