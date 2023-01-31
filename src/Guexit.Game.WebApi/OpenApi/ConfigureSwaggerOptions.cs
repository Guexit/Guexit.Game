using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TryGuessIt.Game.WebApi.OpenApi;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
              description.GroupName,
                new OpenApiInfo()
                {
                    Title = $"Example API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                });
        }
    }
}
