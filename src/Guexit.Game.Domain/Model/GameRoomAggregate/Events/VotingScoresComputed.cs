namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class VotingScoresComputed : IDomainEvent
{
    public Guid GameRoomId { get; }

    public VotingScoresComputed(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId.Value;
    }
}
