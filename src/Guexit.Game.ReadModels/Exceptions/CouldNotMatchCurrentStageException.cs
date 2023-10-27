using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.ReadModels.Exceptions;

public sealed class CouldNotMatchCurrentStageException : Exception
{
    public CouldNotMatchCurrentStageException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Cannot match an stage for game room with id {gameRoomId.Value} to player with id {playerId.Value}")
    {
    }
}