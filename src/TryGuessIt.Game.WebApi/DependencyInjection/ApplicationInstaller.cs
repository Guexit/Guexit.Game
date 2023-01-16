using TryGuessIt.Game.Application.Services;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class ApplicationInstaller
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddScoped<IPlayerManagementService, PlayerManagementService>();
        return services;
    }
}
