using Guexit.Game.Application.Commands;
using Guexit.Game.WebApi.Contracts.Requests;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Guexit.Game.WebApi.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("players").WithOpenApi();

        group.MapPut("/{playerId}/nickname", ChangeNickname);
    }

    private static async Task<IResult> ChangeNickname(
        [FromHeader(Name = GuexitHttpHeaders.AuthenticatedUserId)] string authenticatedUserId,
        [FromRoute] string playerId,
        [FromBody] ChangePlayerNicknameRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new ChangePlayerNicknameCommand(playerId, request.Nickname), ct);
        return Results.Ok();
    }
}