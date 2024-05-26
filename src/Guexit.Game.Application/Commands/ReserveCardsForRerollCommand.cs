using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class ReserveCardsForReRollCommand : IGameRoomCommand
{
    public PlayerId PlayerId { get; }
    public GameRoomId GameRoomId { get; }

    public ReserveCardsForReRollCommand(string playerId, Guid gameRoomId)
    {
        PlayerId = new PlayerId(playerId);
        GameRoomId = new GameRoomId(gameRoomId);
    }
}