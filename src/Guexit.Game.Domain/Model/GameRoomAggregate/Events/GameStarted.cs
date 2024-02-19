namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class GameStarted : IDomainEvent
{
    public Guid GameRoomId { get; }
   
    public GameStarted(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}