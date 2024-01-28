namespace Guexit.Game.Domain.Exceptions;

public sealed class InvalidNicknameException : DomainException
{
    public override string Title => "Invalid nickname";
    
    public InvalidNicknameException() 
        : base("Nickname is invalid because it's empty or composed only by whitespaces")
    {
    }
}