using Guexit.Game.Domain.Model.GameRoomAggregate;
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
        return await _dbContext.GameRooms
            .Include(x => x.PlayerHands).ThenInclude(x => x.Cards)
            .Include(x => x.SubmittedCards).ThenInclude(x => x.Card)
            .Include(x => x.Deck)
            .Include(x => x.FinishedRounds)
            .SingleOrDefaultAsync(g => g.Id == id, ct);
    }
}
