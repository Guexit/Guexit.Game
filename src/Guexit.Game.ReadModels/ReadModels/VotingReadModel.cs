namespace Guexit.Game.ReadModels.ReadModels;

public sealed class VotingReadModel
{
    public required SubmittedCardDto[] Cards { get; init; }
    public required VotingPlayerDto[] GuessingPlayers { get; init; }
    public required bool IsCurrentUserStoryTeller { get; init; }
    public required bool CurrentUserHasAlreadyVoted { get; init; }
    public required StoryTellerDto CurrentStoryTeller { get; init; }
    public required VotedCardDto? CurrentUserVotedCard { get; init; }

    public sealed class SubmittedCardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
        public required bool WasSubmittedByQueryingPlayer { get; init; }
    }
    
    public sealed class VotedCardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
    }
    
    public sealed class VotingPlayerDto
    {
        public required string PlayerId { get; init; }
        public required string Username { get; init; }
        public required string Nickname { get; init; }
        public required bool HasVotedAlready { get; init; }
    }
}
