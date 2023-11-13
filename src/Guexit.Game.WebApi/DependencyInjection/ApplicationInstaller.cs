using Guexit.Game.Application.Services;
using Guexit.Game.WebApi.Logging;
using Guexit.Game.WebApi.Mediator;
using Mediator;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class ApplicationInstaller
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkCommandPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GameRoomOptimisticConcurrencyCheckPipelineBehaviour<,>));

        return services.AddScoped<IPlayerManagementService, PlayerManagementService>()
            .AddScoped<IImageManagementService, ImageManagementService>();
    }
}
