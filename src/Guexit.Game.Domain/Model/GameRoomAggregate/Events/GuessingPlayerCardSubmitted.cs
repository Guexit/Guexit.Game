using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class GuessingPlayerCardSubmitted : IDomainEvent
{
    public Guid GameRoomId { get; }
    public string PlayerId { get; }
    public Guid SelectedCardId { get; }

    public GuessingPlayerCardSubmitted(GameRoomId gameRoomId, PlayerId playerId, CardId cardId)
    {
        GameRoomId = gameRoomId.Value;
        PlayerId = playerId.Value;
        SelectedCardId = cardId.Value;
    }
}
