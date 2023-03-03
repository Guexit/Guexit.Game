using Guexit.Game.Application;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Persistence;
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
                b.MigrationsAssembly(typeof(Persistence.Npgsql.IAssemblyMarker).Assembly.FullName);
            });
        });

        services.AddScoped<IGameRoomRepository, GameRoomRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
