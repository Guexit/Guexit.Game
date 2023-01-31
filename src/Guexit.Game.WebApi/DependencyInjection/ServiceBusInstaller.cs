﻿using MassTransit;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class ServiceBusInstaller
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(typeof(Guexit.Game.ExternalMessageHandlers.IAssemblyMarker).Assembly);
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
