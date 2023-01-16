using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.Commands;

public sealed class JoinGameRoomCommand : ICommand
{
    public PlayerId PlayerId { get; }
    public GameRoomId GameRoomId { get; }

    public JoinGameRoomCommand(string playerId, Guid gameRoomId)
    {
        PlayerId = new PlayerId(playerId);
        GameRoomId = new GameRoomId(gameRoomId);
    }
}
