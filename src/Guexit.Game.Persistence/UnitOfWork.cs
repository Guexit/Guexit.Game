using Guexit.Game.Application;

namespace TryGuessIt.Game.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly GameDbContext _dbContext;

    public UnitOfWork(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
