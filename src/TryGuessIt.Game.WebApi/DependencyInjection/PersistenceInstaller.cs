using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Application;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;
using TryGuessIt.Game.Persistence;
using TryGuessIt.Game.Persistence.Repositories;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class PersistenceInstaller
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IHostEnvironment environment)
    {
        services.AddDbContext<GameDbContext>(options =>
        {
            //if (environment.IsDevelopment())
            //{
            //    options.EnableSensitiveDataLogging();
            //    options.EnableDetailedErrors();
            //}

            options.UseNpgsql(configuration.GetConnectionString("TryGuessIt_Game_GameDb"), b =>
            {
                b.MigrationsAssembly(typeof(Persistence.Npgsql.IAssemblyMarker).Assembly.FullName);
            });
        });

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
