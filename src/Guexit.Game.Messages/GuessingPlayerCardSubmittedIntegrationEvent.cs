namespace Guexit.Game.Messages;

public sealed class GuessingPlayerCardSubmittedIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    public string PlayerId { get; init; } = default!;
    public Guid CardId { get; init; } 

    public GuessingPlayerCardSubmittedIntegrationEvent()
    {
    }

    public GuessingPlayerCardSubmittedIntegrationEvent(Guid gameRoomId, string playerId, Guid selectedCardId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
        CardId = selectedCardId;
    }
}
