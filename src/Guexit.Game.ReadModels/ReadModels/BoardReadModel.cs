namespace Guexit.Game.ReadModels.ReadModels;

public sealed class BoardReadModel
{
    public required Guid GameRoomId { get; init; }
    public required StoryTellerDto CurrentStoryTeller { get; init; }
    public required CardDto[] PlayerHand { get; init; }
    public required GuessingPlayersDto[] GuessingPlayers { get; init; }
    public CardDto? CurrentUserSubmittedCard { get; init; }
    public required bool IsCurrentUserStoryTeller { get; init; }
    public required PlayerDto CurrentPlayer { get; init; }

    public sealed class CardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
    }

    public sealed class GuessingPlayersDto
    {
        public required string PlayerId { get; init; }
        public required string Username { get; init; }
        public required string Nickname { get; init; }
        public required bool HasSubmittedCardAlready { get; init; }
    }

}
