using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Guexit.Game.Component.IntegrationTests.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceAllWithSingleton<TService, TImplementation>(this IServiceCollection services) 
        where TImplementation : class, TService 
        where TService : class
    {
        services.RemoveAll<TService>();
        services.AddSingleton<TImplementation>();
        return services.AddSingleton<TService, TImplementation>(sp => sp.GetRequiredService<TImplementation>());
    }
    
    public static IServiceCollection ReplaceAllWithSingleton<TService, TImplementation>(this IServiceCollection services, 
        TImplementation instance) 
        where TImplementation : class, TService 
        where TService : class
    {
        services.RemoveAll<TService>();
        services.AddSingleton<TImplementation>(instance);
        return services.AddSingleton<TService, TImplementation>(sp => sp.GetRequiredService<TImplementation>());
    }
    
    public static IServiceCollection ReplaceAllWithSingleton<TService, TImplementation>(this IServiceCollection services, 
        Func<IServiceProvider, TImplementation> implementationFactory) 
        where TImplementation : class, TService 
        where TService : class
    {
        services.RemoveAll<TService>();
        services.AddSingleton<TImplementation>(implementationFactory);
        return services.AddSingleton<TService, TImplementation>(sp => sp.GetRequiredService<TImplementation>());
    }
}