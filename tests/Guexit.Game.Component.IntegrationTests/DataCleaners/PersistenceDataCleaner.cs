using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Persistence;

namespace Guexit.Game.Component.IntegrationTests.DataCleaners;

public sealed class PersistenceDataCleaner : ITestDataCleaner
{
    public async ValueTask Clean(GameWebApplicationFactory webApplicationFactory)
    {
        await using var scope = webApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        var allGameRooms = await dbContext.GameRooms.ToArrayAsync();
        var allPlayers = await dbContext.Players.ToArrayAsync();
        dbContext.GameRooms.RemoveRange(allGameRooms);
        dbContext.Players.RemoveRange(allPlayers);
        await dbContext.SaveChangesAsync();
    }
}