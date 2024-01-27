using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.ReadModels.ReadOnlyRepositories;

public sealed class ReadOnlyPlayersRepository
{
    private readonly GameDbContext _dbContext;

    public ReadOnlyPlayersRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Player?> GetBy(PlayerId id, CancellationToken ct = default)
    {
        var gameRoom = await _dbContext.Players.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        return gameRoom;
    }
    
    public async Task<IReadOnlyList<Player>> GetBy(IEnumerable<PlayerId> ids, CancellationToken ct = default)
    {
        var players = await _dbContext.Players.AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync(ct);
        return players;
    }
}