using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class StartGameCommand : ICommand
{
    public GameRoomId GameRoomId { get; }

	public StartGameCommand(Guid gameRoomId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
    }
}
