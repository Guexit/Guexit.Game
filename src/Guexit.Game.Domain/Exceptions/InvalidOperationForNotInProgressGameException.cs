using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class InvalidOperationForNotInProgressGameException : DomainException
{
    public override string Title => "Invalid operation for a game room that is not in progress";

    public InvalidOperationForNotInProgressGameException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} cannot perform this action game is not in progress in game room with id {gameRoomId.Value}.")
    {
    }
}