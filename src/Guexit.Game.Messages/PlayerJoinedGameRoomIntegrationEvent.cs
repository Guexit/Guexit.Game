namespace Guexit.Game.Messages;

public sealed class PlayerJoinedGameRoomIntegrationEvent
{
    public Guid GameRoomId { get; init; }
    public string PlayerId { get; init; } = default!;

    public PlayerJoinedGameRoomIntegrationEvent()
    {
    }

    public PlayerJoinedGameRoomIntegrationEvent(Guid gameRoomId, string playerId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
    }
}