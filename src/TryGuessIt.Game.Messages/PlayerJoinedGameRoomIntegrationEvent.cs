namespace TryGuessIt.Game.Messages;

public sealed class PlayerJoinedGameRoomIntegrationEvent
{
    public Guid GameRoomId { get; set; }
    public string PlayerId { get; set; }
}
