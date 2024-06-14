using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class InvalidOperationForStartedGameException : DomainException
{
    public override string Title => "Invalid operation for a game room that has already started";

    public InvalidOperationForStartedGameException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} cannot perform this action because game room with id {gameRoomId.Value} has already started.")
    {
    }
}