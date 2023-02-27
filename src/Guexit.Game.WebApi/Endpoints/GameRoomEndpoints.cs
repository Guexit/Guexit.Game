﻿using Asp.Versioning.Builder;
using Guexit.Game.Application.Commands;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using TryGuessIt.Game.ReadModels.Queries;
using TryGuessIt.Game.ReadModels.ReadModels;

namespace Guexit.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        app.MapPost("game-rooms/{gameRoomId}", CreateGameRoom)
            .WithApiVersionSet(versionSet) // TODO: find a way to add it to all and dont repeat that
            .MapToApiVersion(1);

        app.MapPost("game-rooms/{gameRoomId}/join", JoinGameRoom)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        app.MapPost("game-rooms/{gameRoomId}/start", StartGame)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        app.MapGet("game-rooms/{gameRoomId}/lobby", GetGameRoomLobby)
            .Produces<GameLobbyReadModel>()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);
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

    private static async Task<IResult> GetGameRoomLobby(
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var readModel = await sender.Send(new GameLobbyQuery(gameRoomId), cancellationToken);
        return Results.Ok(readModel);
    }
}
