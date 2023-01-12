namespace TryGuessIt.Game.WebApi.Endpoints;

public static class GameRoomEndpoints
{
    public static void MapGameRoomEndpoints(this IEndpointRouteBuilder app)
    {
        // TODO add versioning
        app.MapPost("v1/game-rooms", CreateGameRoom);
        app.MapPost("v1/game-rooms/{gameRoomId}/join", JoinGameRoom);
    }

    private static IResult CreateGameRoom()
    {
        return Results.BadRequest();
    }

    private static IResult JoinGameRoom(string gameRoomId)
    {
        return Results.BadRequest();
    }
}
