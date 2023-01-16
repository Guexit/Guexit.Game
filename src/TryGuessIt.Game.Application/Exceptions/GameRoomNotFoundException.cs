using TryGuessIt.Game.Domain.Model.GameRoomAggregate;

namespace TryGuessIt.Game.Application.Exceptions;

public sealed class GameRoomNotFoundException : AggregateNotFoundException
{
    public GameRoomNotFoundException(GameRoomId gameRoomId)
        : base($"Game room with id {gameRoomId.Value} not found.")
    {
    }
}
