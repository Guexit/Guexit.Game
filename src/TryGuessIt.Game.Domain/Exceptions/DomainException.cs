using TryGuessIt.Game.Domain.Model.GameRoomAggregate;

namespace TryGuessIt.Game.Domain.Exceptions;

public sealed class InvalidRequiredMinPlayersException : DomainException
{
	public InvalidRequiredMinPlayersException(int count) 
		: base($"{count} is an invalid required minimum players number")
	{

	}
}
public abstract class DomainException : Exception
{
	public DomainException(string message) : base(message)
	{
	}
}
