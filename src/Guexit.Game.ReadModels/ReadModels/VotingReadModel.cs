namespace Guexit.Game.ReadModels.ReadModels;

public sealed class VotingReadModel
{
    public required SubmittedCardDto[] Cards { get; init; }
    public required PlayerDto[] PlayersWhoHaveAlreadyVoted { get; init; }
    public required bool IsCurrentUserStoryTeller { get; init; }
    public required bool CurrentUserHasAlreadyVoted { get; init; }
    public required StoryTellerDto CurrentStoryTeller { get; init; }

    public sealed class SubmittedCardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
        public required bool WasSubmittedByQueryingPlayer { get; init; }
    }
}
