using Guexit.Game.Application;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Persistence;
using TryGuessIt.Game.Persistence.Outbox;
using TryGuessIt.Game.Persistence.Repositories;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class PersistenceInstaller
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GameDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Guexit_Game_GameDb"), b =>
            {
                b.MigrationsAssembly(typeof(Guexit.Game.Persistence.Npgsql.IAssemblyMarker).Assembly.FullName);
            });
        });

        services.AddSingleton<OutboxMessageFactory>();
        services.AddScoped<IGameRoomRepository, GameRoomRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
