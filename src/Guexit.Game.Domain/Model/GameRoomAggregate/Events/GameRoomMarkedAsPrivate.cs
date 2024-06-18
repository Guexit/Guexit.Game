namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class GameRoomMarkedAsPrivate : IDomainEvent
{
    public Guid GameRoomId { get; }

    public GameRoomMarkedAsPrivate(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}