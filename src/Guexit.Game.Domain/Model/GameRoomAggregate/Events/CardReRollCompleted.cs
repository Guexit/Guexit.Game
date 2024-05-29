using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class CardReRollCompleted : IDomainEvent
{
    public Guid GameRoomId { get; }
    public string PlayerId { get; }
    public Guid OldCardId { get; }
    public Guid NewCardId { get; }

    public CardReRollCompleted(GameRoomId gameRoomId, PlayerId playerId, CardId oldCardId, CardId newCardId)
    {
        GameRoomId = gameRoomId.Value;
        PlayerId = playerId.Value;
        OldCardId = oldCardId.Value;
        NewCardId = newCardId.Value;
    }
}