namespace Guexit.Game.Messages;

public sealed class GameRoomMarkedAsPrivateIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    
    public GameRoomMarkedAsPrivateIntegrationEvent()
    {
    }

    public GameRoomMarkedAsPrivateIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
