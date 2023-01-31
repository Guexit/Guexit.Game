using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using TryGuessIt.Game.Persistence;

namespace TryGuessIt.Game.ReadModels.IntegrationTests;

public sealed class ReadModelsIntegrationTestFixture
{
    public IConfigurationRoot Configuration { get; }

    public ReadModelsIntegrationTestFixture()
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
public sealed class MappingIntegrationTestCollectionDefinition : ICollectionFixture<ReadModelsIntegrationTestFixture>
{
}

[Collection(nameof(MappingIntegrationTestCollectionDefinition))]
public abstract class ReadModelsIntegrationTestBase : IAsyncLifetime
{
    private readonly IDbContextTransaction _transactionToDiscardChanges;
    protected readonly GameDbContext DbContext;

    protected ReadModelsIntegrationTestBase(ReadModelsIntegrationTestFixture fixture)
    {
        var dbContextOptions = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql(fixture.Configuration.GetConnectionString("Guexit_Game_GameDb"))
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