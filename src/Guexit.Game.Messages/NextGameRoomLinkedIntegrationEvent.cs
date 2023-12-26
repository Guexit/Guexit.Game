namespace Guexit.Game.Messages;

public sealed class NextGameRoomLinkedIntegrationEvent
{
    public Guid FinishedGameRoomId { get; init; }
    public Guid NextGameRoomId { get; init; }

    public NextGameRoomLinkedIntegrationEvent()
    {
    }

    public NextGameRoomLinkedIntegrationEvent(Guid finishedGameRoomId, Guid nextGameRoomId)
    {
        FinishedGameRoomId = finishedGameRoomId;
        NextGameRoomId = nextGameRoomId;
    }
}