using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class PlayerCannotVoteTheirOwnSubmittedCardException : DomainException
{
    public override string Title => "Players are not allowed to vote for their own submitted cards";

    public PlayerCannotVoteTheirOwnSubmittedCardException(GameRoomId gameRoomId, PlayerId votingPlayerId, CardId cardId)
        : base(BuildExceptionMessage(gameRoomId, votingPlayerId, cardId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, PlayerId votingPlayerId, CardId cardId)
        => $"Player with id {votingPlayerId.Value} in game room with id {gameRoomId.Value} is not allowed to vote for their own submitted card with id {cardId.Value}";
}