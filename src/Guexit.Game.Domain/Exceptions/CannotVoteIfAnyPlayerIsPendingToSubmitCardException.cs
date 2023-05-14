using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CannotVoteIfAnyPlayerIsPendingToSubmitCardException : DomainException
{
    public override string Title => "Cannot vote until all guessing players have submit their card";

    public CannotVoteIfAnyPlayerIsPendingToSubmitCardException(GameRoomId gameRoomId)
        : base(BuildExceptionMessage(gameRoomId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId) 
        => $"Cannot vote in game room with id {gameRoomId.Value} until all guessing players have submit their card";
}