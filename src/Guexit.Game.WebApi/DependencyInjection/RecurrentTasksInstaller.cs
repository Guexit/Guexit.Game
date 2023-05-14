using Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class RecurrentTasksInstaller
{
    public static IServiceCollection AddRecurrentTasks(this IServiceCollection services)
    {
        services.AddScoped<ImageGenerationService>()
            .AddHostedService<ImageGenerationRecurrentTask>();

        services.AddOptions<ImageGenerationBackgroundServiceOptions>()
            .BindConfiguration(ImageGenerationBackgroundServiceOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
