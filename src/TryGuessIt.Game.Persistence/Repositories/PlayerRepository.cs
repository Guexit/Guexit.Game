using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Persistence.Repositories;

public sealed class PlayerRepository : IPlayerRepository
{
    private readonly GameDbContext _dbContext;

    public PlayerRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Player player)
    {
        await _dbContext.AddAsync(player);
    }

    public async Task<Player?> GetById(string playerId)
    {
        return await _dbContext.Players.FirstOrDefaultAsync(p => p.Id == playerId);
    }
}
