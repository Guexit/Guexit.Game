namespace Guexit.Game.Producers.TimerExpirationCancellation;

public sealed class TimerToMessageIdMap
{
    public Guid TimerId { get; init; }
    public Guid MessageId { get; init; }
}