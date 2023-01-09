using TryGuessIt.Game.Application;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
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
