using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Persistence;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
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
        return services;
    }
}
