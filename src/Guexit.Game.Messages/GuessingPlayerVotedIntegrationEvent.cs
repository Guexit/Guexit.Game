namespace Guexit.Game.Messages;

public sealed class GuessingPlayerVotedIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    public string PlayerId { get; init; } = default!;
    public Guid SelectedCardId { get; init; } 

    public GuessingPlayerVotedIntegrationEvent()
    {
    }

    public GuessingPlayerVotedIntegrationEvent(Guid gameRoomId, string playerId, Guid selectedCardId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
        SelectedCardId = selectedCardId;
    }
}