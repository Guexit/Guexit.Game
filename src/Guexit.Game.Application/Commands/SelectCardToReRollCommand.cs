using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class SelectCardToReRollCommand : IGameRoomCommand
{
    public PlayerId PlayerId { get; }
    public GameRoomId GameRoomId { get; }
    public CardId CardToReRollId { get; }
    public CardId NewCardId { get; }

    public SelectCardToReRollCommand(string playerId, Guid gameRoomId, Guid cardToReRollId, Guid newCardId)
    {
        PlayerId = new(playerId);
        GameRoomId = new(gameRoomId);
        CardToReRollId = new(cardToReRollId);
        NewCardId = new(newCardId);
    }
}