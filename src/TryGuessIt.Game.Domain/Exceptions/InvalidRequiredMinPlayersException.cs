using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Domain.Exceptions;

public sealed class InvalidRequiredMinPlayersException : DomainException
{
	public InvalidRequiredMinPlayersException(int count) 
		: base($"{count} is an invalid required minimum players number.")
	{

	}
}

public sealed class PlayerIsAlreadyInGameRoomException : DomainException
{
    public PlayerIsAlreadyInGameRoomException(PlayerId playerId)
        : base($"Player with id {playerId.Value} is already in game room.")
    {

    }
}
