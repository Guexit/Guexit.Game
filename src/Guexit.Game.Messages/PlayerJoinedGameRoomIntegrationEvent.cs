namespace Guexit.Game.Messages;

public sealed class PlayerJoinedGameRoomIntegrationEvent
{
    public Guid GameRoomId { get; set; }
    public string PlayerId { get; set; } = default!;

    public PlayerJoinedGameRoomIntegrationEvent()
    {
    }

    public PlayerJoinedGameRoomIntegrationEvent(Guid gameRoomId, string playerId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
    }
}
