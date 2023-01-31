using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class PlayerJoinedGameRoom : IDomainEvent
{
    public Guid GameRoomId { get; }
    public string PlayerId { get; }

    public PlayerJoinedGameRoom(GameRoomId gameRoomId, PlayerId playerId)
    {
        GameRoomId = gameRoomId.Value;
        PlayerId = playerId.Value;
    }
}
