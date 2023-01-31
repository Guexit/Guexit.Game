using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Domain.Exceptions;

public sealed class PlayerIsAlreadyInGameRoomException : DomainException
{
    public PlayerIsAlreadyInGameRoomException(PlayerId playerId)
        : base($"Player with id {playerId.Value} is already in game room.")
    {

    }
}
