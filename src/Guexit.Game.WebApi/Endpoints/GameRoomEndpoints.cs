using Asp.Versioning.Builder;
using Guexit.Game.Application.Commands;
using Guexit.Game.ReadModels.QueryHandlers;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.WebApi.Requests;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Guexit.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app.MapGroup("game-rooms").WithApiVersionSet(versionSet).MapToApiVersion(1);

        group.MapPost("/{gameRoomId}", CreateGameRoom);
        group.MapPost("/{gameRoomId}/join", JoinGameRoom);
        group.MapPost("/{gameRoomId}/start", StartGame);
        group.MapPost("/{gameRoomId}/submit-card-story", SubmitCardStory);
        group.MapGet("/{gameRoomId}/lobby", GetLobby).Produces<GameLobbyReadModel>();
        group.MapGet("/{gameRoomId}/board", GetBoard).Produces<GameBoardReadModel>();
    }

    private static async Task<IResult> CreateGameRoom(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new CreateGameRoomCommand(gameRoomId, userId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> JoinGameRoom(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new JoinGameRoomCommand(userId, gameRoomId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> StartGame(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new StartGameCommand(gameRoomId, userId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> SubmitCardStory(
       [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
       [FromRoute] Guid gameRoomId,
       [FromBody] SubmitCardStoryRequest body,
       [FromServices] ISender sender,
       CancellationToken ct)
    {
        await sender.Send(new SubmitCardStoryCommand(userId, gameRoomId, body.CardId, body.Story), ct);
        return Results.Ok();
    }

    private static async Task<IResult> GetLobby(
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new GameLobbyQuery(gameRoomId), ct);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetBoard(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new GameBoardQuery(gameRoomId, userId), ct);
        return Results.Ok(readModel);
    }
}
