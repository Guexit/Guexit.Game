using Asp.Versioning.Builder;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.WebApi.Dtos.Request;

namespace TryGuessIt.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        app.MapPost("game-rooms", CreateGameRoom)
            .Produces<CreateGameRoomResponseDto>(StatusCodes.Status200OK)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        app.MapPost("game-rooms/{gameRoomId}/join", JoinGameRoom)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);
    }

    private static async Task<IResult> CreateGameRoom(
        [FromHeader(Name = TryGuessItHttpHeaders.UserId)] string userId, 
        [FromServices] ISender sender)
    {
        var completion = await sender.Send(new CreateGameRoomCommand(userId));
        return Results.Ok(CreateGameRoomResponseDto.From(completion));
    }

    private static IResult JoinGameRoom(string gameRoomId)
    {
        return Results.BadRequest();
    }
}
