using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using TryGuessIt.Game.Component.IntegrationTests.DataCleaners;

namespace TryGuessIt.Game.Component.IntegrationTests;

[CollectionDefinition(nameof(ComponentTestCollectionDefinition))]
public sealed class ComponentTestCollectionDefinition : ICollectionFixture<GameWebApplicationFactory>
{
}

[Collection(nameof(ComponentTestCollectionDefinition))]
public abstract class ComponentTestBase : IAsyncLifetime
{
    private readonly ITestDataCleaner[] _testDataCleaners;
    protected GameWebApplicationFactory WebApplicationFactory { get; }

    public ComponentTestBase(GameWebApplicationFactory webApplicationFactory)
    {
        WebApplicationFactory = webApplicationFactory;
        _ = WebApplicationFactory.CreateClient();

        _testDataCleaners = new[] { new PersistenceDataCleaner() };
    }

    protected async Task PublishAndWaitUntilConsumed<TMessage>(TMessage message)
        where TMessage : class
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();
            await harness.Bus.Publish<TMessage>(message);

            await harness.Consumed.Any<TMessage>();
        }
        finally
        {
            await harness.Stop();
        }
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach(var cleaner in _testDataCleaners)
        {
            await cleaner.Clean(WebApplicationFactory);
        }
    }

}
