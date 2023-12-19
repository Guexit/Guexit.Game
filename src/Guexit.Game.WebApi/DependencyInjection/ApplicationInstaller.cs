using Guexit.Game.Application.Services;
using Guexit.Game.Persistence.Interceptors;
using Guexit.Game.WebApi.Logging;
using Mediator;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class ApplicationInstaller
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GameRoomDistributedLockPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkCommandPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GameRoomOptimisticConcurrencyCheckPipelineBehaviour<,>));

        services.AddScoped<IPlayerManagementService, PlayerManagementService>();
        services.AddScoped<IImageManagementService, ImageManagementService>();
        
        return services;
    }
}
