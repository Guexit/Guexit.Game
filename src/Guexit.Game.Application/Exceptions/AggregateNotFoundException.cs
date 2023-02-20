namespace Guexit.Game.Application.Exceptions;

public abstract class AggregateNotFoundException : Exception
{
    protected AggregateNotFoundException(string message) : base(message)
    {
    }
}