using System.Collections.Frozen;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class PlayerTimer : Entity<PlayerTimerId>
{
    public PlayerId PlayerId { get; private init; } = default!;
    public DateTimeOffset StartedAt { get; private init; }
    public TimeSpan Duration { get; private init; }
    public TimedAction Action { get; private init; } = default!;
    public bool WasMet { get; private set; }

    public PlayerTimer()
    {
        // Entity Framework required parameterless ctor
    }

    public PlayerTimer(PlayerTimerId timerId, PlayerId playerId, DateTimeOffset startedAt, TimeSpan duration, TimedAction action)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

        Id = timerId;
        PlayerId = playerId;
        StartedAt = startedAt;
        Duration = duration;
        Action = action;
    }

    public bool IsExpiredAt(DateTimeOffset date)
    {
        return date > StartedAt + Duration;
    }

    public void MarkAsMet()
    {
        WasMet = true;
    }
}

public sealed class PlayerTimerId : ValueObject
{
    public Guid Value { get; private init; }


    public PlayerTimerId(Guid value)
    {
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public sealed class TimedAction : ValueObject
{
    public static readonly TimedAction Vote = new("Vote");
    public static readonly TimedAction SubmitStory = new("SubmitStory");
    public static readonly TimedAction SubmitGuessingCard = new("SubmitGuessingCard");
    private static readonly FrozenDictionary<string, TimedAction> AllByValue = new[]
    {
        KeyValuePair.Create(Vote.Value, Vote), 
        KeyValuePair.Create(SubmitStory.Value, SubmitStory), 
        KeyValuePair.Create(SubmitGuessingCard.Value, SubmitGuessingCard)
    }.ToFrozenDictionary();

    public string Value { get; }

    private TimedAction(string value)
    {
        Value = value;
    }

    public static TimedAction From(string value)
    {
        if (AllByValue.TryGetValue(value, out var status))
            return status;

        throw new ArgumentOutOfRangeException(nameof(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}