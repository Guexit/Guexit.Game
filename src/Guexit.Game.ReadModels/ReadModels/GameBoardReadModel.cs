namespace Guexit.Game.ReadModels.ReadModels;

public sealed class GameBoardReadModel
{
    public required Guid GameRoomId { get; init; }
    public required StoryTellerDto CurrentStoryTeller { get; init; }
    public required CardDto[] PlayerHand { get; init; }
    public CardDto? SelectedCard { get; init; }
    public bool IsCurrentUserStoryTeller { get; init; }

    public sealed class StoryTellerDto
    {
        public required string PlayerId { get; init; }
        public required string? Story { get; init; }
    }

    public sealed class CardDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
    }
}
