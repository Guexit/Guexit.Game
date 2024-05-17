using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class InvalidOperationForInProgressGame : DomainException
{
    public override string Title => "Invalid operation for a game room in progress";

    public InvalidOperationForInProgressGame(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} cannot perform this action because it's not valid for in progress game room with id {gameRoomId.Value}.")
    {
    }
}