using Guexit.Game.Application.Services;
using Guexit.Game.WebApi.Logging;
using Mediator;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class ApplicationInstaller
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

        return services.AddScoped<IPlayerManagementService, PlayerManagementService>()
            .AddScoped<IImageManagementService, ImageManagementService>()
            .AddScoped<IDeckAssignmentService, DeckAssignmentService>();
    }
}
