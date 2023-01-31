using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TryGuessIt.Game.Persistence;
using TryGuessIt.Game.ReadModels.Queries;
using TryGuessIt.Game.ReadModels.ReadModels;

namespace TryGuessIt.Game.ReadModels.QueryHandlers;

public sealed class GameLobbyQueryHandler : QueryHandler<GameLobbyQuery, GameLobbyReadModel>
{
    public GameLobbyQueryHandler(GameDbContext dbContext, ILogger<QueryHandler<GameLobbyQuery, GameLobbyReadModel>> logger) 
        : base(dbContext, logger)
    {
    }

    protected override async Task<GameLobbyReadModel> Process(GameLobbyQuery query, CancellationToken cancellationToken)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking().FirstAsync(x => x.Id == query.GameRoomId, cancellationToken: cancellationToken);
        var playersInGame = await DbContext.Players.AsNoTracking().Where(x => gameRoom.PlayerIds.Contains(x.Id)).ToArrayAsync(cancellationToken);

        var gameLobbyReadModel = new GameLobbyReadModel(
            gameRoom.Id.Value,
            gameRoom.RequiredMinPlayers.Count, 
            playersInGame.Select(x => new GameLobbyPlayerDto(x.Username)).ToArray()
        );
        return gameLobbyReadModel;
    }
}
