using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadOnlyRepositories;

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

public sealed class GameLobbyQueryHandler : IQueryHandler<GameLobbyQuery, LobbyReadModel>
{
    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;
    private readonly ReadOnlyPlayersRepository _playersRepository;

    public GameLobbyQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository, ReadOnlyPlayersRepository playersRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _playersRepository = playersRepository;
    }

    public async ValueTask<LobbyReadModel> Handle(GameLobbyQuery query, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(query.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var playersInGame = (await _playersRepository.GetBy(gameRoom.PlayerIds, ct)).ToDictionary(x => x.Id);
        var creator = playersInGame[gameRoom.CreatedBy];

        return new LobbyReadModel
        {
            GameRoomId = gameRoom.Id.Value,
            Players = playersInGame.Select(x => new LobbyPlayerDto { Username = x.Value.Username, Id = x.Key.Value, Nickname = x.Value.Nickname.Value }).ToArray(),
            RequiredMinPlayers = gameRoom.RequiredMinPlayers.Count,
            CanStartGame = gameRoom.RequiredMinPlayers.Count <= playersInGame.Count && gameRoom.CreatedBy == query.PlayerId,
            Creator = new LobbyPlayerDto { Id = creator.Id, Username = creator.Username, Nickname = creator.Nickname.Value },
            GameStatus = gameRoom.Status.Value,
            IsPublic = gameRoom.IsPublic
        };
    }
}
