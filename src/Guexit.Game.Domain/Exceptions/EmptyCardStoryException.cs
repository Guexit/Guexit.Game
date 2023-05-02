using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class EmptyCardStoryException : DomainException
{
    public override string Title { get; } = "Cannot submit empty story";

    public EmptyCardStoryException
        () : base("Submitted story value was empty. Come on tell us something good cabron :P")
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, PlayerId playerId) 
        => $"Cannot submit card story to game room with id {gameRoomId.Value} because player with id {playerId.Value} is not the current story teller";
}
