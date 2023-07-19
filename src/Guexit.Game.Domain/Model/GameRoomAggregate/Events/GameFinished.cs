namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class GameFinished : IDomainEvent
{
    public Guid GameRoomId { get; }

    public GameFinished(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
