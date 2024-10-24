using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Component.IntegrationTests.TestDoubles;
using Guexit.Game.Consumers;
using Guexit.Game.Domain;
using Guexit.Game.Persistence;
using Guexit.Game.Tests.Common;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class GameWebApplicationFactory : WebApplicationFactory<Game.WebApi.IAssemblyMarker>, IAsyncLifetime
{
    private IContainer? _postgreSqlContainer;
    
    public async Task InitializeAsync()
    {
        _postgreSqlContainer = new ContainerBuilder()
            .WithName("guexit_game_database_test")
            .WithImage("postgres:16.4")
            .WithEnvironment("POSTGRES_USER", "postgres")
            .WithEnvironment("POSTGRES_PASSWORD", "postgres")
            .WithEnvironment("POSTGRES_DB", "guexit_game_database_test")
            .WithPortBinding(5555, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
        
        await _postgreSqlContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_postgreSqlContainer is not null) 
            await _postgreSqlContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var dbConnectionString =
                $"User ID=postgres;Password=postgres;Host=localhost;Port={_postgreSqlContainer!.GetMappedPublicPort(5432)};Database=guexit_game_database_test;";
            
            var configurationOverrides = new Dictionary<string, string?>
            {
                { "ConnectionStrings:Guexit_Game_GameDb", dbConnectionString },
                { "Database:MigrateOnStartup", "true" }
            };
            configurationBuilder.AddInMemoryCollection(configurationOverrides);
        });
        
        builder.ConfigureTestServices(services =>
        {
            services.AddMassTransitTestHarness();
            var outboxBackgroundService = services.First(x => x.ImplementationType == typeof(BusOutboxDeliveryService<GameDbContext>));
            services.Remove(outboxBackgroundService);

            var consumerContext = services.Single(d => d.ImplementationType == typeof(ImageGeneratedConsumerDefinition));
            services.Remove(consumerContext);
            services.AddSingleton<ImageGeneratedConsumerDefinition, DummyImageGeneratedConsumerDefinition>();

            services.ReplaceAllWithSingleton<IGuidProvider, FakeGuidProvider>();
        });
    }
}
