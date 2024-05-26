using Guexit.Game.Application.Commands;
using Guexit.Game.ReadModels.QueryHandlers;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.WebApi.Contracts.Requests;
using Guexit.Game.WebApi.Contracts.Responses;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Guexit.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("game-rooms/{gameRoomId:guid}").WithOpenApi();

        group.MapPost("", CreateGameRoom);
        group.MapPost("/join", JoinGameRoom);
        group.MapPost("/start", StartGame);
        group.MapPost("/storyteller/submit-card-story", SubmitStoryTellerCardStory);
        group.MapPost("/guessing-player/submit-card", SubmitGuessingPlayerCard);
        group.MapPost("/submitted-cards/{cardId:guid}/vote", VoteCard);
        group.MapPost("/create-next", CreateNext).Produces<CreateNextGameRoomResponse>();
        group.MapPost("/reserve-cards-for-re-roll", ReserveCardsForReRoll);
        group.MapPost("/player-hand/{cardToReRollId}/swap-with/{selectedNewCardId}", SelectCardToReRoll);

        group.MapGet("/lobby", GetLobby).Produces<LobbyReadModel>();
        group.MapGet("/board", GetBoard).Produces<BoardReadModel>();
        group.MapGet("/voting", GetVoting).Produces<VotingReadModel>();
        group.MapGet("/round-summaries/last", GetLastRoundSummary).Produces<RoundSummaryReadModel>();
        group.MapGet("/summary", GetSummary).Produces<GameSummaryReadModel>();
        group.MapGet("/stages/current", GetStage).Produces<GameStageReadModel>();
    }

    private static async Task<IResult> CreateGameRoom(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new CreateGameRoomCommand(gameRoomId, authenticatedUserId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> JoinGameRoom(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new JoinGameRoomCommand(authenticatedUserId, gameRoomId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> StartGame(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new StartGameCommand(gameRoomId, authenticatedUserId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> SubmitStoryTellerCardStory(
       [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
       [FromRoute] Guid gameRoomId,
       [FromBody] SubmitStoryTellerCardStoryRequest request,
       [FromServices] ISender sender,
       CancellationToken ct)
    {
        await sender.Send(new SubmitStoryTellerCardStoryCommand(authenticatedUserId, gameRoomId, request.CardId, request.Story), ct);
        return Results.Ok();
    }

    private static async Task<IResult> SubmitGuessingPlayerCard(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromBody] SubmitCardForGuessingPlayerRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new SubmitGuessingPlayerCardCommand(authenticatedUserId, gameRoomId, request.CardId), ct);
        return Results.Ok();
    }
    
    private static async Task<IResult> VoteCard(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromRoute] Guid cardId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new VoteCardCommand(authenticatedUserId, gameRoomId, cardId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> CreateNext(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var nextGameRoomId = await sender.Send(new CreateNextGameRoomCommand(authenticatedUserId, gameRoomId), ct);
        return Results.Ok(new CreateNextGameRoomResponse(nextGameRoomId));
    }
    
    private static async Task<IResult> ReserveCardsForReRoll(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new ReserveCardsForReRollCommand(authenticatedUserId, gameRoomId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> SelectCardToReRoll(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromRoute] Guid cardToReRollId,
        [FromRoute] Guid selectedNewCardId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new SelectCardToReRollCommand(authenticatedUserId, gameRoomId, cardToReRollId, selectedNewCardId), ct);
        return Results.Ok();
    }

    private static async Task<IResult> GetLobby(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new GameLobbyQuery(gameRoomId, authenticatedUserId), ct);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetBoard(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new GameBoardQuery(gameRoomId, authenticatedUserId), ct);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetVoting(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new GameVotingQuery(gameRoomId, authenticatedUserId), ct);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetLastRoundSummary(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new LastRoundSummaryQuery(gameRoomId, authenticatedUserId), ct);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetSummary(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new GameRoomSummaryQuery(gameRoomId, authenticatedUserId), ct);
        return Results.Ok(readModel);
    }

    private static async Task<IResult> GetStage(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] Guid gameRoomId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var readModel = await sender.Send(new CurrentStageQuery(gameRoomId, authenticatedUserId), ct);
        return Results.Ok(readModel);
    }
}
