using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameLobbyQuery : IQuery<LobbyReadModel>
{
    public GameRoomId GameRoomId { get; }

    public GameLobbyQuery(Guid gameRoomId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
    }
}

public sealed class GameLobbyQueryHandler : QueryHandler<GameLobbyQuery, LobbyReadModel>
{
    public GameLobbyQueryHandler(GameDbContext dbContext, ILogger<QueryHandler<GameLobbyQuery, LobbyReadModel>> logger) 
        : base(dbContext, logger)
    {
    }

    protected override async Task<LobbyReadModel> Process(GameLobbyQuery query, CancellationToken ct)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking().SingleAsync(x => x.Id == query.GameRoomId, cancellationToken: ct);
        var playersInGame = await DbContext.Players.AsNoTracking().Where(x => gameRoom.PlayerIds.Contains(x.Id)).ToArrayAsync(ct);

        return new LobbyReadModel
        {
            GameRoomId = gameRoom.Id.Value,
            Players = playersInGame.Select(x => new LobbyPlayerDto { Username = x.Username }).ToArray(),
            RequiredMinPlayers = gameRoom.RequiredMinPlayers.Count,
            CanStartGame = gameRoom.RequiredMinPlayers.Count <= playersInGame.Length,
            GameStatus = gameRoom.Status.Value
        };
    }
}
