using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class Points : ValueObject
{
    public static readonly Points Zero = new(0);

    public int Value { get; }

    public Points(int points)
    {
        if (points < 0)
            throw new ArgumentOutOfRangeException(nameof(points));

        Value = points;
    }

    public Points Sum(Points other) => new(Value + other.Value);

    public static Points operator +(Points points1, Points points2) => points1.Sum(points2);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public sealed class FinishedRound : Entity<FinishedRoundId>
{
    public GameRoomId GameRoomId { get; private init; } = default!;
    public DateTimeOffset FinishedAt { get; private init; }
    public IReadOnlyDictionary<PlayerId, Points> Scores { get; private init; } = default!;
    public IReadOnlyDictionary<PlayerId, Card> SubmittedCards { get; private init; } = default!;

    public FinishedRound()
    {
        // EF required parameterless ctor
    }

    public FinishedRound(GameRoomId gameRoomId, DateTimeOffset finishedAt, IDictionary<PlayerId, Points> scores, IDictionary<PlayerId, Card> submittedCards)
    {
        GameRoomId = gameRoomId;
        FinishedAt = finishedAt;
        Scores = scores.AsReadOnly();
        SubmittedCards = submittedCards.AsReadOnly();
    }
}


public sealed class FinishedRoundId : ValueObject
{
    public static readonly FinishedRoundId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public FinishedRoundId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator FinishedRoundId(Guid value) => new(value);
    public static implicit operator Guid(FinishedRoundId value) => value.Value;
}
