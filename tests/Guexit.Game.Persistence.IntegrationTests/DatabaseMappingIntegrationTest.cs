using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Guexit.Game.Persistence.IntegrationTests;

public sealed class IntegrationTestFixture
{
    public IConfigurationRoot Configuration { get; }

    public IntegrationTestFixture()
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
        Configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: false)
           .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();
    }
}

[CollectionDefinition(nameof(MappingIntegrationTestCollectionDefinition))]
public sealed class MappingIntegrationTestCollectionDefinition : ICollectionFixture<IntegrationTestFixture>;

[Collection(nameof(MappingIntegrationTestCollectionDefinition))]
public abstract class DatabaseMappingIntegrationTest : IAsyncLifetime
{
    protected DbContextOptions<GameDbContext> DbContextOptions { get; }
    protected GameDbContext DbContext { get; }


    protected DatabaseMappingIntegrationTest(IntegrationTestFixture fixture, ITestOutputHelper testOutput)
    {
        DbContextOptions = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql(fixture.Configuration.GetConnectionString("Guexit_Game_GameDb"))
            .EnableSensitiveDataLogging()
            .LogTo(testOutput.WriteLine, LogLevel.Information)
            .Options;

        DbContext = new GameDbContext(DbContextOptions);
    }

    protected async Task SaveChanges()
    {
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await DbContext.Cards.ExecuteDeleteAsync();
        await DbContext.PlayerHands.ExecuteDeleteAsync();
        await DbContext.GameRooms.ExecuteDeleteAsync();
        await DbContext.Players.ExecuteDeleteAsync();
        await DbContext.Images.ExecuteDeleteAsync();
        
        await DbContext.DisposeAsync();
    }
}