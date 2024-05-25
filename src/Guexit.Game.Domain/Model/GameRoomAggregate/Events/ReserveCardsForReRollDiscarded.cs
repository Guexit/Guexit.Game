namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class ReserveCardsForReRollDiscarded : IDomainEvent
{
    public Guid GameRoomId { get; }
    public Uri[] UnusedCardImageUrls { get; }

    public ReserveCardsForReRollDiscarded(GameRoomId gameRoomId, Uri[] unusedCardImageUrls)
    {
        GameRoomId = gameRoomId.Value;
        UnusedCardImageUrls = unusedCardImageUrls;
    }
}