using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class PlayerNotInGameRoomException : DomainException
{
    public override string Title => "Player not in game room";

    public PlayerNotInGameRoomException(GameRoomId gameRoomId, PlayerId playerId)
    : base($"Cannot interact with game room with id {gameRoomId.Value} because player with id {playerId.Value} is not inside the room.")
    {

    }
}
