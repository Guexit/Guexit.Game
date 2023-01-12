using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;

namespace TryGuessIt.Game.Persistence.Repositories;

public sealed class GameRoomRepository : IGameRoomRepository
{
    private readonly GameDbContext _dbContext;

    public GameRoomRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask Add(GameRoom gameRoom, CancellationToken ct = default)
    {
        await _dbContext.GameRooms.AddAsync(gameRoom, ct);
    }

    public async ValueTask<GameRoom?> GetBy(GameRoomId id, CancellationToken ct = default)
    {
        return await _dbContext.GameRooms.FirstOrDefaultAsync(g => g.Id == id, ct);
    }
}
