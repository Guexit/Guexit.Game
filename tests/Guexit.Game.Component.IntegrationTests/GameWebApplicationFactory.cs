using Guexit.Game.WebApi;
using Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class GameWebApplicationFactory : WebApplicationFactory<IAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddMassTransitTestHarness();

            var imageGenerationBackgroundService = services.Single(d => d.ImplementationType == typeof(ImageGenerationBackgroundService));
            services.Remove(imageGenerationBackgroundService);
        });
    }
}
