namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class NewRoundStarted : IDomainEvent
{
    public Guid GameRoomId { get; }

    public NewRoundStarted(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}