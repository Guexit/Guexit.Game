using Asp.Versioning;
using Guexit.Game.WebApi.DependencyInjection;
using Guexit.Game.WebApi.Endpoints;
using Guexit.Game.WebApi.ErrorHandling;

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

app.Run();