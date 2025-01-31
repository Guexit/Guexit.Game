﻿using Guexit.Game.Application;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.Persistence.Interceptors;
using Guexit.Game.Persistence.Repositories;
using Guexit.Game.ReadModels;
using Guexit.Game.ReadModels.ReadOnlyRepositories;
using Microsoft.EntityFrameworkCore;

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

        services.AddOptions<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<GameRoomOptimisticConcurrencyCheckEnforcer>();
        services.AddScoped<GameRoomDistributedLock>();
        
        return services.AddScoped<IGameRoomRepository, GameRoomRepository>()
            .AddScoped<IPlayerRepository, PlayerRepository>()
            .AddScoped<IImageRepository, ImageRepository>()
            .AddScoped<ReadOnlyGameRoomRepository>()
            .AddScoped<ReadOnlyPlayersRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<GameDbContextMigrator>();
    }
}
