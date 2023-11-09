using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate.Events;

public sealed class GuessingPlayerVoted : IDomainEvent
{
    public Guid GameRoomId { get; }
    public string PlayerId { get; }
    public Guid SelectedCardId { get; }

    public GuessingPlayerVoted(GameRoomId gameRoomId, PlayerId playerId, CardId cardId)
    {
        GameRoomId = gameRoomId.Value;
        PlayerId = playerId.Value;
        SelectedCardId = cardId.Value;
    }
}