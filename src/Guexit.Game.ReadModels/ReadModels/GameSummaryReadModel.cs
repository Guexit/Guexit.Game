namespace Guexit.Game.ReadModels.ReadModels;

public sealed class GameSummaryReadModel
{
    public required Guid GameRoomId { get; init; }
    public required PlayerScoreDto[] Scores { get; init; }
    public required RoundSummaryDto[] RoundSummaries { get; init; }
    public required string Status { get; init; }
    public bool IsNextGameRoomLinked => NextGameRoomId != Guid.Empty;
    public Guid NextGameRoomId { get; init; }

    public sealed class RoundSummaryDto
    {
        public required Guid FinishedRoundId { get; init; }
        public required DateTimeOffset RoundFinishedAt { get; init; }
        public required StoryTellerDto StoryTeller { get; init; }
        public required RoundSummarySubmittedCardSummaryDto[] SubmittedCardSummaries { get; init; }
        public required ScoreInRoundDto[] Scores { get; init; }

        public sealed class RoundSummarySubmittedCardSummaryDto
        {
            public required Guid CardId { get; init; }
            public required Uri CardUrl { get; init; }
            public required PlayerDto[] Voters { get; init; }
            public required PlayerDto SubmittedBy { get; init; }
        }

        public sealed class ScoreInRoundDto
        {
            public required PlayerDto Player { get; init; }
            public required int Points { get; init; }
        }
    }

    public sealed class PlayerScoreDto
    {
        public required PlayerDto Player { get; init; }
        public required int Points { get; init; }
    }
}

