using TryGuessIt.Game.WebApi.DependencyInjection;
using TryGuessIt.Game.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddServiceBus(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGameRoomEndpoints();

app.Run();