using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class GamePermissionDeniedException : DomainException
{
    public override string Title => "Game Permission Denied";

    public GamePermissionDeniedException(GameRoomId gameRoomId, PlayerId playerId) 
        : base($"Player {playerId.Value} is not authorized to do this action in room {gameRoomId.Value} because it's restricted only to the game room creator.")
    { }
}