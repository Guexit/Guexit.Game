using Guexit.Game.Persistence;
using Guexit.Game.WebApi.DependencyInjection;
using Guexit.Game.WebApi.Endpoints;
using Guexit.Game.WebApi.ErrorHandling;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddServiceBus(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapExceptionsToProblemDetails();
app.UseHttpsRedirection();
app.MapGameRoomEndpoints();
app.MapPlayerEndpoints();

var databaseOptions = app.Services.GetRequiredService<IOptions<DatabaseOptions>>();
if (databaseOptions.Value.MigrateOnStartup)
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<GameDbContextMigrator>().MigrateAsync();
}

await app.RunAsync();
