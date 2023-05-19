using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public class JoinStartedGameException : DomainException
{
    public override string Title => "Cannot join started game exception";

    public JoinStartedGameException(PlayerId playerId, GameRoomId gameRoomId)
        : base(BuildExceptionMessage(playerId, gameRoomId))
    {
    }

    private static string BuildExceptionMessage(PlayerId playerId, GameRoomId gameRoomId) 
        => $"Player with id {playerId.Value} cannot join game with id {gameRoomId.Value} because game already started";
}
