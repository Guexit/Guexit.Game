namespace Guexit.Game.Messages;

public sealed class CardReRollReservedIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    public string PlayerId { get; init; } = default!;

    public CardReRollReservedIntegrationEvent()
    {
    }

    public CardReRollReservedIntegrationEvent(Guid gameRoomId, string playerId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
    }
}
