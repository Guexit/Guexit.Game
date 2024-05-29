using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class CardReRollReserved : IDomainEvent
{
    public Guid GameRoomId { get; }
    public string PlayerId { get; }

    public CardReRollReserved(GameRoomId gameRoomId, PlayerId playerId)
    {
        GameRoomId = gameRoomId.Value;
        PlayerId = playerId.Value;
    }
}
