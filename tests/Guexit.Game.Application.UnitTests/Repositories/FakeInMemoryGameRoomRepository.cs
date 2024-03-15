using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class FakeInMemoryGameRoomRepository : IGameRoomRepository
{
    private readonly Dictionary<GameRoomId, GameRoom> _gameRooms = new();

    public ValueTask Add(GameRoom gameRoom, CancellationToken ct = default)
    {
        _gameRooms.Add(gameRoom.Id, gameRoom);
        return ValueTask.CompletedTask;
    }

    public ValueTask<GameRoom?> GetBy(GameRoomId id, CancellationToken ct = default)
    {
        return ValueTask.FromResult(_gameRooms.GetValueOrDefault(id));
    }
}

