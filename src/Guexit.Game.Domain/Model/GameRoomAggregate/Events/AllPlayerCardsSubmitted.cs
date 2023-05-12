namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class AllPlayerCardsSubmitted : IDomainEvent
{
    public Guid GameRoomId { get; }

    public AllPlayerCardsSubmitted(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
