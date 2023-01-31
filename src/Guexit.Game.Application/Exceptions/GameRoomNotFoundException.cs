using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.Exceptions;

public sealed class GameRoomNotFoundException : AggregateNotFoundException
{
    public GameRoomNotFoundException(GameRoomId gameRoomId)
        : base($"Game room with id {gameRoomId.Value} not found.")
    {
    }
}
