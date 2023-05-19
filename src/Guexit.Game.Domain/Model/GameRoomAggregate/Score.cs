using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class Score : ValueObject
{
    public FinishedRoundId FinishedRoundId { get; private set; } = default!;
    public PlayerId PlayerId { get; private set; } = default!;
    public Points Points { get; private set; } = default!;

    public Score()
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
