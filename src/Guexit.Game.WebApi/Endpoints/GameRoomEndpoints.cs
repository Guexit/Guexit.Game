using Asp.Versioning.Builder;
using Guexit.Game.Application.Commands;
using Guexit.Game.ReadModels.QueryHandlers;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.WebApi.Contracts.Requests;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Guexit.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app.MapGroup("game-rooms/{gameRoomId:guid}")
            .WithApiVersionSet(versionSet).MapToApiVersion(1);

        group.MapPost("", CreateGameRoom);
        group.MapPost("/join", JoinGameRoom);
        group.MapPost("/start", StartGame);
        group.MapPost("/storyteller/submit-card-story", SubmitStoryTellerCardStory);
        group.MapPost("/guessing-player/submit-card", SubmitGuessingPlayerCard);
        group.MapPost("/submitted-cards/{cardId:guid}/vote", VoteCard);
        
        group.MapGet("/lobby", GetLobby).Produces<LobbyReadModel>();
        group.MapGet("/board", GetBoard).Produces<BoardReadModel>();
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

    private static async Task<IResult> SubmitStoryTellerCardStory(
       [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
       [FromRoute] Guid gameRoomId,
       [FromBody] SubmitStoryTellerCardStoryRequest request,
       [FromServices] ISender sender,
       CancellationToken ct)
    {
        await sender.Send(new SubmitStoryTellerCardStoryCommand(userId, gameRoomId, request.CardId, request.Story), ct);
        return Results.Ok();
    }

    private static async Task<IResult> SubmitGuessingPlayerCard(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId,
        [FromBody] SubmitCardForGuessingPlayerRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new SubmitGuessingPlayerCardCommand(userId, gameRoomId, request.CardId), ct);
        return Results.Ok();
    }
    
    private static async Task<IResult> VoteCard(
        [FromHeader(Name = GuexitHttpHeaders.UserId)] string userId,
        [FromRoute] Guid gameRoomId,
        [FromRoute] Guid cardId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new VoteCardCommand(userId, gameRoomId, cardId), ct);
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
