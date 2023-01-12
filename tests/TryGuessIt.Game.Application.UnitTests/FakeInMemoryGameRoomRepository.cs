using TryGuessIt.Game.Domain.Model.GameRoomAggregate;

namespace TryGuessIt.Game.Application.UnitTests;

public sealed class FakeInMemoryGameRoomRepository : IGameRoomRepository
{
    private readonly List<GameRoom> _gameRooms = new();

    public ValueTask Add(GameRoom gameRoom, CancellationToken ct = default)
    {
        _gameRooms.Add(gameRoom);
        return ValueTask.CompletedTask;
    }

    public ValueTask<GameRoom?> GetBy(GameRoomId id, CancellationToken ct = default)
    {
        return ValueTask.FromResult(_gameRooms.FirstOrDefault(r => r.Id == id));
    }
}

