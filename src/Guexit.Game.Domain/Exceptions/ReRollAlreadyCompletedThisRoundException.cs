using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class ReRollAlreadyCompletedThisRoundException : DomainException
{
    public override string Title => "Card re-roll already completed this round";

    public ReRollAlreadyCompletedThisRoundException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} cannot re-roll in game room with id {gameRoomId.Value} because they have already completed a re-roll.")
    {
    }
}
