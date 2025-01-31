﻿using Guexit.Game.Component.IntegrationTests.Extensions;
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
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class GameWebApplicationFactory : WebApplicationFactory<Game.WebApi.IAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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
