using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Persistence;

namespace TryGuessIt.Game.Component.IntegrationTests.DataCleaners;

public sealed class PersistenceDataCleaner : ITestDataCleaner
{
    public async ValueTask Clean(GameWebApplicationFactory webApplicationFactory)
    {
        var scopeFactory = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
        await using var scope = scopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        var allPlayers = await dbContext.Players.ToArrayAsync();
        var allGameRooms = await dbContext.GameRooms.ToArrayAsync();
        dbContext.Players.RemoveRange(allPlayers);
        dbContext.GameRooms.RemoveRange(allGameRooms);
        await dbContext.SaveChangesAsync();
    }
}