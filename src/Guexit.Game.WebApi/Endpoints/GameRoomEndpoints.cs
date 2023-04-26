using Asp.Versioning.Builder;
using Guexit.Game.Application.Commands;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.ReadModels.QueryHandlers;

namespace Guexit.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        app.MapPost("game-rooms/{gameRoomId}", CreateGameRoom)
            .WithApiVersionSet(versionSet).MapToApiVersion(1);

        app.MapPost("game-rooms/{gameRoomId}/join", JoinGameRoom)
            .WithApiVersionSet(versionSet).MapToApiVersion(1);

        app.MapPost("game-rooms/{gameRoomId}/start", StartGame)
            .WithApiVersionSet(versionSet).MapToApiVersion(1);

        app.MapGet("game-rooms/{gameRoomId}/lobby", GetLobby)
            .WithApiVersionSet(versionSet).MapToApiVersion(1)
            .Produces<GameLobbyReadModel>();

        app.MapGet("game-rooms/{gameRoomId}/board", GetBoard)
            .WithApiVersionSet(versionSet).MapToApiVersion(1)
            .Produces<GameBoardReadModel>();
    }

    private static async Task<IResult> CreateGameRoom(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateGameRoomCommand(gameRoomId, userId), cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> JoinGameRoom(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new JoinGameRoomCommand(userId, gameRoomId), cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> StartGame(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new StartGameCommand(gameRoomId, userId), cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> GetLobby(
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var readModel = await sender.Send(new GameLobbyQuery(gameRoomId), cancellationToken);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetBoard(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var readModel = await sender.Send(new GameBoardQuery(gameRoomId, userId), cancellationToken);
        return Results.Ok(readModel);
    }
}
