using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CardNotFoundInPlayerHandException : DomainException
{
    public override string Title { get; } = "Card not found in player's hand";

    public CardNotFoundInPlayerHandException(GameRoomId gameRoomId, PlayerId playerId, CardId cardId)
        : base(BuildExceptionMessage(gameRoomId, playerId, cardId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, PlayerId playerId, CardId cardId)
        => $"Cannot submit card with id {cardId.Value} for player with id {playerId.Value} in game room with id {gameRoomId.Value} because it's not found in player's hand";
}
