namespace Guexit.Game.Messages;

public sealed class VotingScoresComputedIntegrationEvent
{
    public Guid GameRoomId { get; init; }


    public VotingScoresComputedIntegrationEvent()
    {
    }

    public VotingScoresComputedIntegrationEvent(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}
