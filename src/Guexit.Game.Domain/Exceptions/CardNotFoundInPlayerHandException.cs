using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CardNotFoundInPlayerHandException : DomainException
{
    public override string Title { get; } = "Card not found in player's hand";

    public CardNotFoundInPlayerHandException(PlayerId playerId, CardId cardId)
        : base(BuildExceptionMessage(playerId, cardId))
    {
    }

    private static string BuildExceptionMessage(PlayerId playerId, CardId cardId)
        => $"Cannot submit card with id {cardId.Value} for player with id {playerId.Value} because it's not found in player's hand";
}
