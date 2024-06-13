namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class GameRoomMarkedAsPublic : IDomainEvent
{
    public Guid GameRoomId { get; }

    public GameRoomMarkedAsPublic(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}

public sealed class GameRoomMarkedAsPrivate : IDomainEvent
{
    public Guid GameRoomId { get; }

    public GameRoomMarkedAsPrivate(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
