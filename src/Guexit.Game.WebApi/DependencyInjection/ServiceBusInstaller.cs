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
                //outboxOptions.QueryDelay = TimeSpan.FromSeconds(2);
            });
            
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(typeof(ExternalMessageHandlers.IAssemblyMarker).Assembly);
            config.UsingAzureServiceBus((context, serviceBusConfiguration) =>
            {
                serviceBusConfiguration.Host(configuration.GetConnectionString("Guexit_ServiceBus"));
                serviceBusConfiguration.ConfigureEndpoints(context);

                serviceBusConfiguration.SubscriptionEndpoint("guexit-game", "guexit-imagegeneration", 
                    endpoint => endpoint.ConfigureConsumer<ImageGeneratedHandler>(context));

                serviceBusConfiguration.UseMessageRetry(r => r.Incremental(10, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
            });
        });

        return services;
    }
}

