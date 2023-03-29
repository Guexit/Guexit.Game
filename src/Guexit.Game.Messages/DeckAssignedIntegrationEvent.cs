namespace Guexit.Game.Messages;

public sealed class DeckAssignedIntegrationEvent
{
    public Guid GameRoomId { get; init; }


    public DeckAssignedIntegrationEvent()
    {
    }

    public DeckAssignedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}