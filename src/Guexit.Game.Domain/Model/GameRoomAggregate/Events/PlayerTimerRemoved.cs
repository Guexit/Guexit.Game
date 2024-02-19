using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class PlayerTimerRemoved : IDomainEvent
{
    public Guid GameRoomId { get; }
    public Guid TimerId { get; }
    public string PlayerId { get; }
    public string Action { get; }
    public DateTimeOffset StartedAt { get; }
    public TimeSpan Duration { get; }

    public PlayerTimerRemoved(GameRoomId gameRoomId, PlayerTimerId timerId, PlayerId playerId, TimedAction action, DateTimeOffset startedAt, TimeSpan duration)
    {
        GameRoomId = gameRoomId.Value;
        TimerId = timerId.Value;
        PlayerId = playerId.Value;
        Action = action.Value;
        StartedAt = startedAt;
        Duration = duration;
    }
}