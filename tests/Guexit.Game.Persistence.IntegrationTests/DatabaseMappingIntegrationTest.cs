using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace TryGuessIt.Game.Persistence.IntegrationTests;

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
public sealed class MappingIntegrationTestCollectionDefinition : ICollectionFixture<IntegrationTestFixture>
{
}

[Collection(nameof(MappingIntegrationTestCollectionDefinition))]
public abstract class DatabaseMappingIntegrationTest : IAsyncLifetime
{
    private readonly IDbContextTransaction _transactionToDiscardChanges;
    protected readonly GameDbContext DbContext;

    protected DatabaseMappingIntegrationTest(IntegrationTestFixture fixture)
    {
        var dbContextOptions = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql(fixture.Configuration.GetConnectionString("TryGuessIt_Game_GameDb"))
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new GameDbContext(dbContextOptions);
        _transactionToDiscardChanges = DbContext.Database.BeginTransaction();
    }

    protected async Task SaveChangesAndClearChangeTracking()
    {
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _transactionToDiscardChanges.RollbackAsync();
        await _transactionToDiscardChanges.DisposeAsync();
        await DbContext.DisposeAsync();
    }
}