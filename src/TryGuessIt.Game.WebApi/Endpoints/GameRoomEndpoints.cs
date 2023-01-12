using Mediator;
using Microsoft.AspNetCore.Mvc;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.WebApi.Dtos.Request;

namespace TryGuessIt.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("v1/game-rooms", CreateGameRoom)
            .Produces<CreateGameRoomResponseDto>(StatusCodes.Status200OK);
        app.MapPost("v1/game-rooms/{gameRoomId}/join", JoinGameRoom);
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
