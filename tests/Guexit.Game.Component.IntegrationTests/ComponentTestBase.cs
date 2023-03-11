using Guexit.Game.Component.IntegrationTests.DataCleaners;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Persistence;

namespace Guexit.Game.Component.IntegrationTests;

[CollectionDefinition(nameof(ComponentTestCollectionDefinition))]
public sealed class ComponentTestCollectionDefinition : ICollectionFixture<GameWebApplicationFactory>
{
}

[Collection(nameof(ComponentTestCollectionDefinition))]
public abstract class ComponentTestBase : IAsyncLifetime
{
    private readonly ITestDataCleaner[] _testDataCleaners;
    protected GameWebApplicationFactory WebApplicationFactory { get; }

    protected ComponentTestBase(GameWebApplicationFactory webApplicationFactory)
    {
        WebApplicationFactory = webApplicationFactory;
        _ = WebApplicationFactory.CreateClient();

        _testDataCleaners = new[] { new PersistenceDataCleaner() };
    }

    protected async Task ConsumeMessage<TMessage>(TMessage message)
        where TMessage : class
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();
            await harness.Bus.Publish(message);

            await harness.Consumed.Any<TMessage>();
        }
        finally
        {
            await harness.Stop();
        }
    }

    protected async Task AssumeExistingPlayer(Player player)
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();
    }

    protected async Task AssumeExistingGameRoom(GameRoom gameRoom)
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        await dbContext.GameRooms.AddAsync(gameRoom);
        await dbContext.SaveChangesAsync();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var cleaner in _testDataCleaners)
            await cleaner.Clean(WebApplicationFactory);
    }
}
