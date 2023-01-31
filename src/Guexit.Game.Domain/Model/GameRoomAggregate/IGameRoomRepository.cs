namespace TryGuessIt.Game.Domain.Model.GameRoomAggregate;

public interface IGameRoomRepository
{
    ValueTask<GameRoom?> GetBy(GameRoomId id, CancellationToken cancellationToken = default);
    ValueTask Add(GameRoom gameRoom, CancellationToken cancellationToken = default);
}
