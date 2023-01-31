using Guexit.Game.Component.IntegrationTests.DataCleaners;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

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

    public async Task InitializeAsync()
    {
        foreach (var cleaner in _testDataCleaners)
        {
            await cleaner.Clean(WebApplicationFactory);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
