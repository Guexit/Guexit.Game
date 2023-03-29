namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class DeckAssigned : IDomainEvent
{
    public Guid GameRoomId { get; }

    public DeckAssigned(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
