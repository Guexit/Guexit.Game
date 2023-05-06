namespace Guexit.Game.ReadModels.Exceptions;

public abstract class QueryException : Exception
{
    public abstract string Title { get; }

    public QueryException(string message) : base(message)
    { }
}
