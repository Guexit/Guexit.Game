namespace Guexit.Game.Domain.Exceptions;

public sealed class EmptyUsernameException : DomainException
{
    public override string Title => "Invalid empty username";
    
    public EmptyUsernameException() : base("Empty or whitespace usernames are invalid")
    {
    }
}