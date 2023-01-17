using Mediator;
using TryGuessIt.Game.Application.Services;
using TryGuessIt.Game.WebApi.Logging;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class ApplicationInstaller
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

        services.AddScoped<IPlayerManagementService, PlayerManagementService>();
        return services;
    }
}
