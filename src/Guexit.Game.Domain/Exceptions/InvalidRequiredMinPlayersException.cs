namespace Guexit.Game.Domain.Exceptions;

public sealed class InvalidRequiredMinPlayersException : DomainException
{
    public override string Title { get; } = "Invalid minimum number of required players.";

	public InvalidRequiredMinPlayersException(int count) 
		: base($"{count} is an invalid required minimum players.")
	{

	}
}
