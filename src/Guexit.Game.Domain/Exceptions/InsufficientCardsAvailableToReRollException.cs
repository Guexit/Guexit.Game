using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public class InsufficientCardsAvailableToReRollException : DomainException
{
    public override string Title => "Insufficient images to reserve cards for re-roll";

    public InsufficientCardsAvailableToReRollException(int actualAvailableImages, GameRoomId gameRoomId, PlayerId playerId)
        : base(BuildExceptionMessage(actualAvailableImages, gameRoomId, playerId))
    {
    }

    private static string BuildExceptionMessage(int actualAvailableCards, GameRoomId gameRoomId, PlayerId playerId) 
        => $"{actualAvailableCards} are less images than required to reserve cards to player with id {playerId.Value} for rerolling in game room with id {gameRoomId.Value}";
}
