using Guexit.Game.Domain;
using Guexit.Game.OutboxPublisher;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TryGuessIt.Game.Persistence;
using IAssemblyMarker = Guexit.Game.Persistence.Npgsql.IAssemblyMarker;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((builderContext, services) =>
{
    services.AddMassTransit(config =>
    {
        config.SetKebabCaseEndpointNameFormatter();

        config.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(builderContext.Configuration.GetConnectionString("TryGuessIt_ServiceBus"));
            cfg.ConfigureEndpoints(context);
        });
    });

    services.AddDbContext<GameDbContext>(options =>
    {
        options.UseNpgsql(builderContext.Configuration.GetConnectionString("Guexit_Game_GameDb"), b =>
        {
            b.MigrationsAssembly(typeof(IAssemblyMarker).Assembly.FullName);
        });
    });

    services.AddSingleton<ISystemClock, SystemClock>();
    services.AddScoped<IOutboxMessagePublisher, OutboxMessagePublisher>();
    services.AddHostedService<OutboxMessagePublisherHeartbeat>();
});

var host = builder.Build();

await host.RunAsync();