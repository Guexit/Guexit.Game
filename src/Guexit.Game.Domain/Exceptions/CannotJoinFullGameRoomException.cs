using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CannotJoinFullGameRoomException : DomainException
{
    public override string Title => "Cannot join full game room";

    public CannotJoinFullGameRoomException(PlayerId playerId, GameRoomId gameRoomId)
        : base($"Player with id {playerId.Value} cannot join game room with id {gameRoomId.Value} because it is already full")
    {
    }
}