using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence.Repositories;

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
