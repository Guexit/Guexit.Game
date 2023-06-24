namespace Guexit.Game.ReadModels.ReadModels;

public sealed class RoundSummaryReadModel
{
    public required Guid GameRoomId { get; init; }
    public required Guid FinishedRoundId { get; init; }
    public required DateTimeOffset RoundFinishedAt { get; init; }
    public required StoryTellerDto StoryTeller { get; init; }
    public required SubmittedCardSummaryDto[] SubmittedCardSummaries { get; init; }
    public required ScoreDto[] Scores { get; init; }

    public sealed class SubmittedCardSummaryDto
    {
        public required Guid CardId { get; init; }
        public required Uri CardUrl { get; init; }
        public required PlayerDto[] Voters { get; init; }
        public required PlayerDto SubmittedBy { get; init; }
    }

    public sealed class ScoreDto
    {
        public required PlayerDto Player { get; init; }
        public required int Points { get; init; }
    }
}
