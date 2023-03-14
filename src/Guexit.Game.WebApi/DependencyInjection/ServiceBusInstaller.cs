using System;
using System.Reflection;
using Guexit.Game.ExternalMessageHandlers;
using Guexit.Game.Sagas;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
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

            config.AddSagaStateMachine<DeckAssignmentSaga, DeckAssignmentState>()
                .EntityFrameworkRepository(cfg =>
                {
                    cfg.ConcurrencyMode = ConcurrencyMode.Pessimistic;

                    cfg.AddDbContext<DbContext, DeckAssignmentSagaDbContext>((serviceProvider, builder) =>
                    {
                        builder.UseNpgsql(configuration.GetConnectionString("Guexit_Game_MasstransitJobServiceDb"), m =>
                        {
                            m.MigrationsAssembly(typeof(Sagas.IAssemblyMarker).Assembly.GetName().Name);
                            m.MigrationsHistoryTable($"__{nameof(JobServiceSagaDbContext)}");
                        });
                    });
                })
                ;

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

public class DeckAssignmentSagaDbContextFactory : IDesignTimeDbContextFactory<DeckAssignmentSagaDbContext>
{
    public DeckAssignmentSagaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DeckAssignmentSagaDbContext>();
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__Guexit_Game_MasstransitJobServiceDb"), m =>
        {
            m.MigrationsAssembly(typeof(Sagas.IAssemblyMarker).Assembly.GetName().Name);
            m.MigrationsHistoryTable($"__{nameof(JobServiceSagaDbContext)}");
        });

        return new DeckAssignmentSagaDbContext(optionsBuilder.Options);
    }
}