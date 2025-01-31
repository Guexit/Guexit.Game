using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class CreateNextGameRoomCommand : IGameRoomCommand<GameRoomId>
{
    public PlayerId PlayerId { get; }
    public GameRoomId GameRoomId { get; }

    public CreateNextGameRoomCommand(string playerId, Guid gameRoomId)
    {
        PlayerId = new PlayerId(playerId);
        GameRoomId = new GameRoomId(gameRoomId);
    }
}