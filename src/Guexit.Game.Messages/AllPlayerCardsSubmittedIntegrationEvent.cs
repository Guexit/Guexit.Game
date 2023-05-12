namespace Guexit.Game.Messages;

public sealed class AllPlayerCardsSubmittedIntegrationEvent
{
    public Guid GameRoomId { get; init; }


    public AllPlayerCardsSubmittedIntegrationEvent()
    {
    }

    public AllPlayerCardsSubmittedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}