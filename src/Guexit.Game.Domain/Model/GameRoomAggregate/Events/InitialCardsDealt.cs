namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class InitialCardsDealt : IDomainEvent
{
    public Guid GameRoomId { get; }

    public InitialCardsDealt(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
