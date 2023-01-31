namespace TryGuessIt.Game.Domain.Exceptions;

public sealed class InvalidRequiredMinPlayersException : DomainException
{
	public InvalidRequiredMinPlayersException(int count) 
		: base($"{count} is an invalid required minimum players number.")
	{

	}
}
