using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Exceptions;

public sealed class PlayerNotFoundException : AggregateNotFoundException
{
    public PlayerNotFoundException(PlayerId playerId) 
        : base($"Player with id {playerId.Value} not found.")
    {
    }
}
