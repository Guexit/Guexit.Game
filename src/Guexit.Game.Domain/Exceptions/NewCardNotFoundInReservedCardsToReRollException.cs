using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class NewCardNotFoundInReservedCardsToReRollException : DomainException
{
    public override string Title => "Card was not found in player's reserved cards for re-roll";

    public NewCardNotFoundInReservedCardsToReRollException(GameRoomId gameRoomId, PlayerId playerId, CardId cardId)
        : base($"Player with id {playerId.Value} cannot select a card to re-roll in game room with id {gameRoomId.Value} because card with id {cardId.Value} is not found in theirs reserved cards for re-roll.")
    {
    }
}