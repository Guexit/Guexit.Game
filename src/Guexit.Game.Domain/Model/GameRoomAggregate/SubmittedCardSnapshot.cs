using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class SubmittedCardSnapshot : Entity<SubmittedCardSnapshotId>
{
    public PlayerId PlayerId { get; private set; } = default!;
    public Card Card { get; private set; } = default!;
    public FinishedRoundId FinishedRoundId { get; private set; } = default!;
    public ICollection<PlayerId> Voters { get; private set; } = new List<PlayerId>();

    private SubmittedCardSnapshot()
    {
        // Entity Framework required parameterless ctor 
    }

    public SubmittedCardSnapshot(PlayerId playerId, Card cards, FinishedRoundId finishedRoundId, IEnumerable<PlayerId> voters)
    {
        Id = new SubmittedCardSnapshotId(Guid.NewGuid());
        PlayerId = playerId;
        Card = cards;
        FinishedRoundId = finishedRoundId;
        Voters = voters.ToArray();
    }
}

public sealed class SubmittedCardSnapshotId : ValueObject
{
    public static readonly SubmittedCardSnapshotId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public SubmittedCardSnapshotId(Guid value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator SubmittedCardSnapshotId(Guid value) => new(value);
    public static implicit operator Guid(SubmittedCardSnapshotId value) => value.Value;
}