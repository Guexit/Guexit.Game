using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class GameStartPermissionDeniedException : DomainException
{
    public override string Title => "Game Start Permission Denied";

    public GameStartPermissionDeniedException(GameRoomId gameRoomId, PlayerId playerId) 
        : base($"Player {playerId.Value} is not authorized to start the game in room {gameRoomId.Value}. Only the game creator can start the game.")
    { }
}