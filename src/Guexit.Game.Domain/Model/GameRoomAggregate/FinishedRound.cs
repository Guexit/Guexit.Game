using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class FinishedRound : Entity<FinishedRoundId>
{
    public GameRoomId GameRoomId { get; private init; } = default!;
    public DateTimeOffset FinishedAt { get; private init; }
    public ICollection<Score> Scores { get; private init; } = default!;
    public ICollection<SubmittedCardSnapshot> SubmittedCardSnapshots { get; private init; } = default!;
    public StoryTeller StoryTeller { get; private init; } = default!;

    public FinishedRound() 
    {
        // Entity Framework required parameterless constructor
    }

    public FinishedRound(GameRoomId gameRoomId, DateTimeOffset finishedAt, IReadOnlyDictionary<PlayerId, Points> scores, IEnumerable<SubmittedCard> submittedCards, StoryTeller storyTeller)
    {
        Id = new FinishedRoundId(Guid.NewGuid());
        GameRoomId = gameRoomId;
        FinishedAt = finishedAt;
        Scores = scores.Select(x => new Score(Id, x.Key, x.Value)).ToList();
        SubmittedCardSnapshots = submittedCards.Select(x => new SubmittedCardSnapshot(x.PlayerId, x.Card, Id, x.Voters)).ToList();
        StoryTeller = storyTeller;
    }

    public Points GetScoredPointsOf(PlayerId playerId)
    {
        var score = Scores.SingleOrDefault(x => x.PlayerId == playerId);
        if (score is null)
            throw new InvalidOperationException($"Could not found score for player {playerId}");

        return score.Points;
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
