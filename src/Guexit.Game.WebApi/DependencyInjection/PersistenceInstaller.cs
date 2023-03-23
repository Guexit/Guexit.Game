using Guexit.Game.Application;
using Guexit.Game.Application.CardAssigment;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.Persistence.Npgsql;
using Guexit.Game.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class PersistenceInstaller
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddDbContextPool<GameDbContext>(options =>
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

        services.AddScoped<ILogicalShardDistributedLock, NpgsqlLogicalShardDistributedLock>();

        return services;
    }
}
