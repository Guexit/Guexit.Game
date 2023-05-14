using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CardNotFoundInSubmittedCardException : DomainException
{
    public override string Title => "Card not found in submitted cards";

    public CardNotFoundInSubmittedCardException(GameRoomId gameRoomId, CardId cardId)
        : base(BuildExceptionMessage(gameRoomId, cardId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, CardId cardId)
        => $"Card with id {cardId.Value} not found in submitted cards in game room with id {gameRoomId.Value}";
}