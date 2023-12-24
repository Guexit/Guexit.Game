using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.DataCleaners;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

[CollectionDefinition(nameof(ComponentTestCollectionDefinition))]
public sealed class ComponentTestCollectionDefinition : ICollectionFixture<GameWebApplicationFactory>;

[Collection(nameof(ComponentTestCollectionDefinition))]
public abstract class ComponentTest : IAsyncLifetime
{
    private static readonly ITestDataCleaner[] _testDataCleaners = { new PersistenceDataCleaner() };
    
    protected GameWebApplicationFactory WebApplicationFactory { get; }

    protected ComponentTest(GameWebApplicationFactory webApplicationFactory)
    {
        WebApplicationFactory = webApplicationFactory;
    }

    protected async Task ConsumeMessage<TMessage>(TMessage message)
        where TMessage : class
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();
            await harness.PublishAndWaitUntilConsumed(message);
        }
        finally
        {
            await harness.Stop();
        }
    }

    protected async Task Save<TAggregateRoot>(params TAggregateRoot[] aggregateRoots) where TAggregateRoot : class, IAggregateRoot
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        await dbContext.Set<TAggregateRoot>().AddRangeAsync(aggregateRoots);
        await dbContext.SaveChangesAsync();
    }

    protected async Task<HttpResponseMessage> Send(HttpMethod httpMethod, string requestUri, HttpContent content, 
        PlayerId authenticatedPlayerId)
    {
        using var request = new HttpRequestMessage(httpMethod, requestUri);
        request.Content = content;
        request.AddPlayerIdHeader(authenticatedPlayerId);
        return await Send(request);
    }

    protected async Task<HttpResponseMessage> Send(HttpMethod httpMethod, string requestUri, PlayerId authenticatedPlayerId)
    {
        using var request = new HttpRequestMessage(httpMethod, requestUri);
        request.AddPlayerIdHeader(authenticatedPlayerId);
        return await Send(request);
    }

    private async Task<HttpResponseMessage> Send(HttpRequestMessage request)
    {
        using var client = WebApplicationFactory.CreateClient();
        return await client.SendAsync(request);
    }

    public Task InitializeAsync() => CleanUp();
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task CleanUp()
    {
        foreach (var cleaner in _testDataCleaners)
            await cleaner.Clean(WebApplicationFactory);
    }
}
