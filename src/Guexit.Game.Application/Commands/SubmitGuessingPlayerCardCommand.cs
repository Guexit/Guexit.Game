using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class SubmitGuessingPlayerCardCommand : IGameRoomCommand
{
    public PlayerId PlayerId { get; }
    public GameRoomId GameRoomId { get; }
    public CardId CardId { get; }

    public SubmitGuessingPlayerCardCommand(string playerId, Guid gameRoomId, Guid cardId)
    {
        PlayerId = new PlayerId(playerId);
        GameRoomId = new GameRoomId(gameRoomId);
        CardId = cardId;
    }
}