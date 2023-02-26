namespace Guexit.Game.Domain.Exceptions;

public abstract class DomainException : Exception
{
	public abstract string Title { get; }

	public DomainException(string message) : base(message)
	{
	}
}
