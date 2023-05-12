namespace Guexit.Game.Messages;

public sealed class InitialCardsDealedIntegrationEvent
{
    public Guid GameRoomId { get; init; }


    public InitialCardsDealedIntegrationEvent()
    {
    }

    public InitialCardsDealedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
