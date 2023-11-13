using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class CreateGameRoomCommand : IGameRoomCommand
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

	public CreateGameRoomCommand(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}
