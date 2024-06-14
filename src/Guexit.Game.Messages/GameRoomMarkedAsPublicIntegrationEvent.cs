namespace Guexit.Game.Messages;

public sealed class GameRoomMarkedAsPublicIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    
    public GameRoomMarkedAsPublicIntegrationEvent()
    {
    }

    public GameRoomMarkedAsPublicIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
