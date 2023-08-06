namespace Guexit.Game.Messages;

public sealed class InitialCardsDealtIntegrationEvent
{
    public Guid GameRoomId { get; init; }

    public InitialCardsDealtIntegrationEvent()
    {
    }

    public InitialCardsDealtIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
