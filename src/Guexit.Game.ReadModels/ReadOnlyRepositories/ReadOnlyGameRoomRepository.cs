using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.ReadModels.Extensions;
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

    public async Task<PaginatedCollection<GameRoom>> GetAvailable(PaginationSettings paginationSettings, CancellationToken ct = default)
    {
        var availableGameRooms = await _dbContext.GameRooms.AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Status == GameStatus.NotStarted && x.IsPublic && x.PlayerIds.Count < GameRoom.MaximumPlayers)
            .PaginateAsync(paginationSettings, ct);
        
        return availableGameRooms;
    }
}