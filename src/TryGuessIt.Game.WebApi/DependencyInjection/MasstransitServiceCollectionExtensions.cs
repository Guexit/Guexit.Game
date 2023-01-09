using MassTransit;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class MasstransitServiceCollectionExtensions
{
    public static IServiceCollection AddMasstransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(typeof(ExternalMessageHandlers.IAssemblyMarker).Assembly);
            config.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Incremental(10, TimeSpan.Zero, TimeSpan.FromSeconds(1)));

                cfg.Host(configuration.GetConnectionString("TryGuessIt_ServiceBus"));
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
