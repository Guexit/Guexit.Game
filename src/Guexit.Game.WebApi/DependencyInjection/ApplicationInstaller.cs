using Guexit.Game.Application.CardAssigment;
using Guexit.Game.Application.Services;
using Guexit.Game.WebApi.Logging;
using Mediator;

namespace Guexit.Game.WebApi.DependencyInjection;

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
        services.AddScoped<IImageManagementService, ImageManagementService>();
        services.AddScoped<ILogicalShardProvider, LogicalShardProvider>();
        services.AddScoped<IDeckAssignmentService, DeckAssignmentService>();

        services.AddOptions<CardAssignmentOptions>()
            .BindConfiguration(CardAssignmentOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
