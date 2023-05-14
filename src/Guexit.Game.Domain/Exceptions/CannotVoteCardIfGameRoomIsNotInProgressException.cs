using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CannotVoteCardIfGameRoomIsNotInProgressException : DomainException
{
    public override string Title => "Cannot vote submitted card to a not started game";

    public CannotVoteCardIfGameRoomIsNotInProgressException(GameRoomId gameRoomId)
        : base(BuildExceptionMessage(gameRoomId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId) 
        => $"Cannot vote submitted card to game with id {gameRoomId.Value} because is not in progress";
}