using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameLobbyQuery : IQuery<LobbyReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public GameLobbyQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
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
        var gameRoom = await DbContext.GameRooms.AsNoTracking().AsSplitQuery().SingleOrDefaultAsync(x => x.Id == query.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var playersInGame = await DbContext.Players.AsNoTracking().Where(x => gameRoom.PlayerIds.Contains(x.Id)).ToArrayAsync(ct);

        return new LobbyReadModel
        {
            GameRoomId = gameRoom.Id.Value,
            Players = playersInGame.Select(x => new LobbyPlayerDto { Username = x.Username, Id = x.Id.Value }).ToArray(),
            RequiredMinPlayers = gameRoom.RequiredMinPlayers.Count,
            CanStartGame = gameRoom.RequiredMinPlayers.Count <= playersInGame.Length && gameRoom.CreatedBy == query.PlayerId,
            CreatedBy = gameRoom.CreatedBy,
            GameStatus = gameRoom.Status.Value
        };
    }
}
