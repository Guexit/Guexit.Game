namespace Guexit.Game.Producers.TimerExpirationCancellation;

public interface ITimerExpirationMessageIdLookup
{
    Task Add(TimerToMessageIdMap timerToMessageIdMap, CancellationToken cancellationToken = default);
    Task<Guid> GetMessageIdOf(Guid timerId, CancellationToken cancellationToken = default);
    Task Remove(Guid timerId, CancellationToken cancellationToken = default);
}