namespace Guexit.Game.ReadModels.ReadModels;

public sealed class VotingReadModel
{
    public required CardDto[] Cards { get; init; }
    public required PlayerDto[] PlayersWhoHaveAlreadyVoted { get; init; }
    public required bool IsCurrentUserStoryTeller { get; init; }
    public required bool CurrentUserHasAlreadyVoted { get; init; }

    public sealed class CardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
    }

    public sealed class PlayerDto
    {
        public required string PlayerId { get; init; }
        public required string Username { get; init; }
    }
}