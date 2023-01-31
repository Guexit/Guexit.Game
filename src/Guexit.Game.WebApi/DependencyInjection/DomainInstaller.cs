using Guexit.Game.Domain;

namespace TryGuessIt.Game.WebApi.DependencyInjection;

public static class DomainInstaller
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IGuidProvider, GuidProvider>();
        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        return services;
    }
}
