using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class ApplicationInsightsInstaller
{
    public static IServiceCollection AddAppInsights(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();
        services.AddSingleton<ITelemetryInitializer, GuexitFrontendTelemetryInitializer>();
        return services;
    }
    
    public sealed class GuexitFrontendTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry) => telemetry.Context.Cloud.RoleName = "Game";
    }
}