using Asp.Versioning;
using Guexit.Game.Persistence;
using Guexit.Game.WebApi;
using Guexit.Game.WebApi.DependencyInjection;
using Guexit.Game.WebApi.Endpoints;
using Guexit.Game.WebApi.ErrorHandling;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddServiceBus(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddRecurrentTasks(builder.Configuration);

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.MapExceptionsToProblemDetails();
app.UseHttpsRedirection();
app.MapGameRoomEndpoints(versionSet);

var databaseOptions = app.Services.GetRequiredService<IOptions<DatabaseOptions>>();
if (databaseOptions.Value.MigrateOnStartup)
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<GameDbContextMigrator>()
        .MigrateAsync();
}

await app.RunAsync();
