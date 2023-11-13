using Guexit.Game.Domain.Model.GameRoomAggregate;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence;

public sealed class GameRoomOptimisticConcurrencyCheckEnforcer
{
    private readonly GameDbContext _dbContext;

    public GameRoomOptimisticConcurrencyCheckEnforcer(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void ForceConcurrencyTokenCheckFor(GameRoomId gameRoomId)
    {
        var gameRoom = _dbContext.ChangeTracker.Entries<GameRoom>().FirstOrDefault(x => x.Entity.Id == gameRoomId);
        if (gameRoom is null)
            return;

        if (gameRoom.State is EntityState.Unchanged)
            gameRoom.State = EntityState.Modified;
    }
}
