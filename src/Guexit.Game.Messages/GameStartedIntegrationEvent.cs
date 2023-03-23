namespace Guexit.Game.Messages;

public sealed class GameStartedIntegrationEvent
{
    public Guid GameRoomId { get; init; }


    public GameStartedIntegrationEvent()
    {
    }

    public GameStartedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}