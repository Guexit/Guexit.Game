namespace Guexit.Game.Messages;

public sealed class GameFinishedIntegrationEvent
{
    public Guid GameRoomId { get; init; }

    public GameFinishedIntegrationEvent()
    {
    }

    public GameFinishedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
