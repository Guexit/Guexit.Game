using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TryGuessIt.Game.OutboxPublisher;
using TryGuessIt.Game.Persistence;

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
        options.UseNpgsql(builderContext.Configuration.GetConnectionString("TryGuessIt_Game_GameDb"), b =>
        {
            b.MigrationsAssembly(typeof(TryGuessIt.Game.Persistence.Npgsql.IAssemblyMarker).Assembly.FullName);
        });
    });

    services.AddScoped<IOutboxMessagePublisher, OutboxMessagePublisher>();
    services.AddHostedService<OutboxMessagePublishHeartbeat>();
});

var host = builder.Build();

await host.RunAsync();