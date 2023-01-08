using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Persistence.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly DbContext _dbContext;

    public PlayerRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Add(Player player)
    {
        throw new NotImplementedException();
    }

    public Task<Player?> GetById(string playerId)
    {
        throw new NotImplementedException();
    }
}
