using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public class InsufficientImagesToAssignDeckException : DomainException
{
    public override string Title { get; } = "Insufficient images to assign deck";

    public InsufficientImagesToAssignDeckException(int actualAvailableImages, GameRoomId gameRoomId)
        : base(BuildExceptionMessage(actualAvailableImages, gameRoomId))
    {
    }

    private static string BuildExceptionMessage(int actualAvailableCards, GameRoomId gameRoomId) 
        => $"{actualAvailableCards} are less images than required to assign a deck to game room with id {gameRoomId.Value}";
}
