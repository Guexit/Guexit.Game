using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class Score : ValueObject
{
    public FinishedRoundId FinishedRoundId { get; private init; } = default!;
    public PlayerId PlayerId { get; private init; } = default!;
    public Points Points { get; private init; } = default!;

    private Score()
    {
        // EF required parameterless ctor
    }

    public Score(FinishedRoundId finishedRoundId, PlayerId playerId, Points points)
    {
        PlayerId = playerId;
        Points = points;
        FinishedRoundId = finishedRoundId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FinishedRoundId;
        yield return PlayerId;
        yield return Points;
    }
}
