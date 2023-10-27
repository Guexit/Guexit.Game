using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.ReadModels.Exceptions;

public sealed class CannotReadLastFinishedRoundSummaryIfHasNotAnyFinishedRound : QueryException
{
    public override string Title { get; } = "Cannot read last finished round summary because game hasn't any finished round";

    public CannotReadLastFinishedRoundSummaryIfHasNotAnyFinishedRound(GameRoomId gameRoomId)
        : base($"Cannot read last finished round summary of game {gameRoomId.Value} because game hasn't any finished round")
    {
    }
}
