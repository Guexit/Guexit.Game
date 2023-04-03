namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class InitialCardsDealed : IDomainEvent
{
    public Guid GameRoomId { get; }

    public InitialCardsDealed(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
