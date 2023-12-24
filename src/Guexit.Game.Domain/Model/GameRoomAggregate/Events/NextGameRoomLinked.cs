namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class NextGameRoomLinked : IDomainEvent
{
    public Guid FinishedGameRoomId { get; }
    public Guid NextGameRoomId { get; }

    public NextGameRoomLinked(GameRoomId finished, GameRoomId next)
    {
        FinishedGameRoomId = finished.Value;
        NextGameRoomId = next.Value;
    }
}