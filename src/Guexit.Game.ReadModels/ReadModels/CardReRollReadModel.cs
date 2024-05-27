namespace Guexit.Game.ReadModels.ReadModels;

public sealed class CardReRollReadModel
{
    public required CardForReRollDto[] ReservedCardsToReRoll { get; init; }
    public required CardForReRollDto[] CurrentPlayerHand { get; init; }

    public required Guid GameRoomId { get; init; }

    public sealed class CardForReRollDto
    {
        public required Guid Id { get; init; }
        public required Uri Url { get; init; }
    }
}
