namespace Guexit.Game.Domain.Exceptions;

public sealed class EmptyCardStoryException : DomainException
{
    public override string Title => "Cannot submit empty story";

    public EmptyCardStoryException() : base("Submitted story value was empty. It must be a non-empty text")
    { }
}