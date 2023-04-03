using Guexit.Game.Component.IntegrationTests.DataCleaners;
using Guexit.Game.Domain;
using Guexit.Game.Persistence;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

// TODO: This should be implementing ICollectionFixture, need to spend some investigate why tests that uses ITestHarness are failing
[CollectionDefinition(nameof(ComponentTestCollectionDefinition))]
public sealed class ComponentTestCollectionDefinition : IClassFixture<GameWebApplicationFactory>
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

            await harness.Published.Any<TMessage>();
            await harness.Consumed.Any<TMessage>();
        }
        finally
        {
            await harness.Stop();
        }
    }

    protected async Task Save<TAggregate>(params TAggregate[] aggregate) where TAggregate : class, IAggregateRoot
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await dbContext.Set<TAggregate>().AddRangeAsync(aggregate);
        await dbContext.SaveChangesAsync();
    }

    protected TAggregate GetSingle<TAggregate>(Func<TAggregate, bool> predicate) where TAggregate : class, IAggregateRoot
    {
        using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    
        return dbContext.Set<TAggregate>().Single(predicate);
    }

    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var cleaner in _testDataCleaners)
            await cleaner.Clean(WebApplicationFactory);
    }
}
