using Asp.Versioning;
using Guexit.Game.WebApi.OpenApi;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Guexit.Game.WebApi.DependencyInjection;

public static class SwaggerInstaller
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

        return services;
    }
}
