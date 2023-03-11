using Guexit.Game.ExternalMessageHandlers;
using MassTransit;
using TryGuessIt.Game.Persistence;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class ServiceBusInstaller
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.AddEntityFrameworkOutbox<GameDbContext>(outboxOptions =>
            {
                outboxOptions.UsePostgres();
                outboxOptions.UseBusOutbox();
            });
            
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(typeof(ExternalMessageHandlers.IAssemblyMarker).Assembly);
            config.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("Guexit_ServiceBus"));
                cfg.ConfigureEndpoints(context);
                cfg.SubscriptionEndpoint("guexit-game", "guexit-imagegeneration", e => e.ConfigureConsumer<ImageGeneratedHandler>(context));

                cfg.UseMessageRetry(r => r.Incremental(10, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
            });
        });

        return services;
    }
}

