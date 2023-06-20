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
    }

    public sealed class StoryTellerDto
    {
        public required string Username { get; init; }
        public required string PlayerId { get; init; }
        public required string? Story { get; init; }
    }

    public sealed class PlayerDto
    {
        public required string PlayerId { get; init; }
        public required string Username { get; init; }
    }
}