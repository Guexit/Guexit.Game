using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests.DataCleaners;

public sealed class PersistenceDataCleaner : ITestDataCleaner
{
    public async ValueTask Clean(GameWebApplicationFactory webApplicationFactory)
    {
        await using var scope = webApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        await dbContext.Cards.ExecuteDeleteAsync();
        await dbContext.PlayerHands.ExecuteDeleteAsync();
        await dbContext.GameRooms.ExecuteDeleteAsync();
        await dbContext.Players.ExecuteDeleteAsync();
        await dbContext.Images.ExecuteDeleteAsync();
    }
}