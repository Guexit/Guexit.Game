using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CardReRollNotReservedException : DomainException
{
    public override string Title => "Card re-roll not reserved";

    public CardReRollNotReservedException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} cannot select a card to re-roll in game room with id {gameRoomId.Value} because they haven't reserved any cards to re-roll.")
    {
    }
}
