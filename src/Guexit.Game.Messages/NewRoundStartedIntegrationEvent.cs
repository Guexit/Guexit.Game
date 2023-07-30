namespace Guexit.Game.Messages;

public sealed class NewRoundStartedIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    
    public NewRoundStartedIntegrationEvent()
    {
    }

    public NewRoundStartedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}