namespace Guexit.Game.Messages;

public sealed class CardReRollCompletedIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    public string PlayerId { get; init; } = default!;
    public Guid OldCardId { get; init; }
    public Guid NewCardId { get; init; }


    public CardReRollCompletedIntegrationEvent()
    {
    }

    public CardReRollCompletedIntegrationEvent(Guid gameRoomId, string playerId, Guid oldCardId, Guid newCardId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
        OldCardId = oldCardId;
        NewCardId = newCardId;
    }
}