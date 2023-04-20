using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.Queries;
using Guexit.Game.ReadModels.ReadModels;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameLobbyQueryHandler : QueryHandler<GameLobbyQuery, GameLobbyReadModel>
{
    public GameLobbyQueryHandler(GameDbContext dbContext, ILogger<QueryHandler<GameLobbyQuery, GameLobbyReadModel>> logger) 
        : base(dbContext, logger)
    {
    }

    protected override async Task<GameLobbyReadModel> Process(GameLobbyQuery query, CancellationToken ct)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking().SingleAsync(x => x.Id == query.GameRoomId, cancellationToken: ct);
        var playersInGame = await DbContext.Players.AsNoTracking().Where(x => gameRoom.PlayerIds.Contains(x.Id)).ToArrayAsync(ct);

        return new GameLobbyReadModel
        {
            GameRoomId = gameRoom.Id.Value,
            Players = playersInGame.Select(x => new GameLobbyPlayerDto { Username = x.Username }).ToArray(),
            RequiredMinPlayers = gameRoom.RequiredMinPlayers.Count,
            CanStartGame = gameRoom.RequiredMinPlayers.Count < playersInGame.Length,
            GameStatus = gameRoom.Status.Value
        };
    }
}
