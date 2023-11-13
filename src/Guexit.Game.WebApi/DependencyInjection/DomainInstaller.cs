using Guexit.Game.Domain;
using Guexit.Game.WebApi.Mediator;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class DomainInstaller
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IGuidProvider, GuidProvider>();
        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddScoped<IDomainEventPublisher, MediatorDomainEventPublisher>();

        return services;
    }
}
