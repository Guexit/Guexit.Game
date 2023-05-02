using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CannotSubmitCardStoryIfGameRoomIsNotInProgressException : DomainException
{
    public override string Title { get; } = "Cannot submit card story to a not started game";

    public CannotSubmitCardStoryIfGameRoomIsNotInProgressException(GameRoomId gameRoomId)
        : base(BuildExceptionMessage(gameRoomId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId) 
        => $"Cannot submit card story to game with id {gameRoomId.Value} because is not in progress";
}
