using Guexit.Game.Consumers;
using Guexit.Game.Messages;
using Guexit.Game.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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

            config.AddConsumers(typeof(Consumers.IAssemblyMarker).Assembly);

            EndpointConvention.Map<GenerateImagesCommand>(new Uri("queue:guexit-cron-generate-image-command"));
            EndpointConvention.Map<GenerateImagesCommandWithStyles>(new Uri("queue:guexit-cron-generate-image-command"));

            config.UsingAzureServiceBus((context, serviceBusConfiguration) =>
            {
                serviceBusConfiguration.Host(configuration.GetConnectionString("Guexit_ServiceBus"));
                serviceBusConfiguration.ConfigureEndpoints(context);
                
                serviceBusConfiguration.SubscriptionEndpoint("guexit-game", "guexit-imagegeneration", 
                    endpoint => endpoint.ConfigureConsumer<ImageGeneratedConsumer>(context));

                serviceBusConfiguration.UseMessageRetry(r => r.Incremental(10, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
            });
        });

        return services;
    }
}