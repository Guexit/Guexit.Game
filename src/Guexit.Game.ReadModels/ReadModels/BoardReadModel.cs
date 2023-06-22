namespace Guexit.Game.ReadModels.ReadModels;

public sealed class BoardReadModel
{
    public required Guid GameRoomId { get; init; }
    public required StoryTellerDto CurrentStoryTeller { get; init; }
    public required CardDto[] PlayerHand { get; init; }
    public CardDto? CurrentUserSubmittedCard { get; init; }
    public required CardDto[] SubmittedCards { get; init; }
    public required bool IsCurrentUserStoryTeller { get; init; }

    public sealed class CardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
    }
}
