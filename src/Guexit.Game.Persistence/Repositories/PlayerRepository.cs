using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence.Repositories;

public sealed class PlayerRepository : IPlayerRepository
{
    private readonly GameDbContext _dbContext;

    public PlayerRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Player player, CancellationToken ct = default)
    {
        await _dbContext.Players.AddAsync(player, ct);
    }

    public async Task<Player?> GetBy(PlayerId id, CancellationToken ct = default)
    {
        return await _dbContext.Players.FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
