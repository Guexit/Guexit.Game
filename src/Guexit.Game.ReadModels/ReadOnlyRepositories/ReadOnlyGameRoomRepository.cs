using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.ReadModels.ReadOnlyRepositories;

public sealed class ReadOnlyGameRoomRepository
{
    private readonly GameDbContext _dbContext;

    public ReadOnlyGameRoomRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GameRoom?> GetBy(GameRoomId id, CancellationToken ct = default)
    {
        var gameRoom = await _dbContext.GameRooms.AsNoTracking()
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Id == id, ct);
        return gameRoom;
    }
}